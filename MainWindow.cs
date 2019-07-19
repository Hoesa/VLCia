using System;
using Gtk;
using LibVLCSharp.Shared;
using LibVLCSharp.GTK;

public partial class MainWindow : Gtk.Window

{
    MediaPlayer mediaPlayer; //mediaplayer
    bool mediaplay = false;  //bool to check if something is already playing
    bool mediapause = false; //bool to check if media is paused
    string stream; //file to stream

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
        if (choose.Active == -1)
        { txt.Text = "Choose first :)"; }
        else if (mediaplay == false)
        {

            using (var libvlc = new LibVLC())
            {
                mediaPlayer = new MediaPlayer(libvlc);
                stream = choose.ActiveText;

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

                mediaplay = true;
                txt.Text = "Playing";
                System.Threading.Thread.Sleep(5000);
                scale.SetRange(0, Convert.ToDouble(mediaPlayer.Length));
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
        mediaPlayer.Pause();
        if (mediapause == false)
        { txt.Text = "Pause"; mediapause = true; }
        else
        { txt.Text = "Playing"; mediapause = false; }
    }

    //Stop
    protected void Stop(object sender, EventArgs e)
    {
        mediaPlayer.Stop();
        txt.Text = "Stopped";
        mediaplay = false;
        scale.Sensitive = false;
        scale.Value = 0;
    }

    //Refresh
    protected async void Refresh(object sender, EventArgs e)
    {
        await VLCia.MainClass.RefresAsync();
        VLCia.MainClass.Process();

        foreach (string media in VLCia.MainClass.mediafiles)
        {
            choose.AppendText(media);
        }

        refresh.Sensitive = false;

    }

    //Copy stream URL to Clipboard
    protected void Url(object sender, EventArgs e)
    {
        if (choose.Active == -1)
        { }
        else
        {
            string stream = choose.ActiveText;
            Gtk.Clipboard clipboard = Gtk.Clipboard.Get(Gdk.Atom.Intern("CLIPBOARD", false));
            clipboard.Text = "http://localhost:9980/renter/stream/" + stream;
            txt.Text = "http://localhost:9980/renter/stream/" + stream;
        }
    }

    //Combobox input change while playing
    protected void Changed(object sender, EventArgs e)
    {
        if (mediaplay == true)
        {
            mediaPlayer.Stop();
            mediaplay = false;
            scale.Sensitive = false;
            scale.Value = 0;
            txt.Text = "Stopped";
        }
    }

    //Set scale value according to mediaplayer time
    void MediaPlayer_TimeChanged1(object sender, MediaPlayerTimeChangedEventArgs e)
    {
        scale.Value = mediaPlayer.Time;
    }

}


