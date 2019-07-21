using System;
using Gtk;
using LibVLCSharp.Shared;
using LibVLCSharp.GTK;
using System.Threading.Tasks;

public partial class MainWindow : Gtk.Window

{
    MediaPlayer mediaPlayer; //mediaplayer
    bool mediaplay = false;  //bool to check if something is already playing
    bool mediapause = false; //bool to check if media is paused
    string stream; //file to stream
    bool scrol = false; //bool to check if user is scrolling

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Gtk.Application.Quit();
        a.RetVal = true;
    }

    //Play media
    protected void Play(object sender, EventArgs e)
    {
        if (cbox.Active == -1)
        { txt.Text = "Choose first :)"; }
        else if (mediaplay == false)
        {
            using (var libvlc = new LibVLC())
            {
                mediaPlayer = new MediaPlayer(libvlc);
                stream = cbox.ActiveText;

                // Make videoview and add 
                VideoView mediav = new VideoView { MediaPlayer = mediaPlayer };

                VLCia.MainClass.win.Add(mediav);
                VLCia.MainClass.win.ShowAll();

                //start video
                using (var media = new Media(libvlc,
                    "http://localhost:9980/renter/stream/" + stream,
                    FromType.FromLocation))
                {
                    mediaPlayer.Play(media);
                }

                do
                {
                    txt.Text = "Loading";
                }
                while (mediaPlayer.IsPlaying == false);
                                        
                mediaplay = true;
                txt.Text = "Playing";          
                scale.SetRange(0, (mediaPlayer.Length / 1000));
                scale.Sensitive = true;
                mediaPlayer.TimeChanged += MediaPlayer_TimeChanged1;
                   
            }
        }

        else if (mediaplay == true)
        {
            mediaPlayer.Pause();
            if (mediapause == false)
            { txt.Text = "Pause"; mediapause = true; }
            else
            { txt.Text = "Playing"; mediapause = false; }
        }

    }

    //Pause
    protected void Pause(object sender, EventArgs e)
    {
        if(mediaplay == true)
        {
          mediaPlayer.Pause();
          if (mediapause == false)
          { txt.Text = "Pause"; mediapause = true; }
          else
          { txt.Text = "Playing"; mediapause = false; }
        }
    }

    //Stop
    protected void Stop(object sender, EventArgs e)
    {
        if (mediaplay == true) 
        { 
            mediaPlayer.Stop();
            txt.Text = "Stopped";
            mediaplay = false;
            scale.Sensitive = false;
            scale.Value = 0;
        }
    }

    //Refresh
    protected async void Refresh(object sender, EventArgs e)
    {
        await VLCia.MainClass.RefresAsync();
        VLCia.MainClass.Process();

        foreach (string media in VLCia.MainClass.mediafiles)
        {
            cbox.AppendText(media);
        }

        refresh.Sensitive = false;
        cbox.Sensitive = true;
    }

    //Copy stream URL to Clipboard
    protected void Url(object sender, EventArgs e)
    {
        if (cbox.Active == -1)
        { }
        else
        {
            string stream = cbox.ActiveText;
            Gtk.Clipboard clipboard = Gtk.Clipboard.Get(Gdk.Atom.Intern("CLIPBOARD", false));
            clipboard.Text = "http://localhost:9980/renter/stream/" + stream;
            txt.Text = "http://localhost:9980/renter/stream/" + stream;
        }
    }

    //Combobox input change while playing
    protected void Combochanged(object sender, EventArgs e)
    {
        if (mediaplay == true)
        {
            mediaPlayer.Stop();
            mediaplay = false;
            scale.Sensitive = false;
            scale.Value = 0;
            txt.Text = "Welcome";
        }
    }

    //update scale time
    void MediaPlayer_TimeChanged1(object sender, MediaPlayerTimeChangedEventArgs e)
    {
        if (scrol == false)
        { scale.Value = mediaPlayer.Time / 1000; }       
    }

    //Change mediaplayer time if user scrolled
    protected async void OnScaleValueChanged(object sender, EventArgs e)
    {

        if (scrol == true)
        {
            await Task.Delay(2000);
            mediaPlayer.Time = (long)(scale.Value * 1000);
            scrol = false;
            play.GrabFocus();
        }
     
    }

    //Check if user starts scrolling
    protected void OnScaleFocusGrabbed(object sender, EventArgs e)
    {
        scrol = true;
    }

}




