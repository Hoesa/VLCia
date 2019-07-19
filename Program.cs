using System;
using Gtk;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;

namespace VLCia
{
    class MainClass
    {
      
        public static MainWindow win; 
        public static String json; //Json response string
        public static string split = "},{"; //Where to split data of sia files in the json
        public static string cutafter = "\",\"stuck\""; //cuting data not needed after this point
        public static string cutbefore = "\"siapath\":\""; //cuting data not needed before this point
        public static List<string> mediafiles = new List<string>(); //List of media files on sia available for download

        public static string[] mediaExtensions = {
       ".MOV",".MKV",".MP4",".3G2",".3GA","3GP",".AAC",".ADT",".ANX",".AVCHD","AVI",".BIK",
       ".F4V",".FLV",".GXF",".H264",".HDMOV",".M1V","M2V","M4A",".M4B",".M4V",".MKA",".MKS",
       ".MP3","..MPEG1",".MPEG4",".MPG2",".MPV",".MTS",".NSV",".NUV",".OGG",".OGV",".OMA",
       ".OPUS",".PSS",".RA",".RBS",".REC",".RM",".RMI",".RT",".SPX",".SVI",".TOD",".TRP",
       ".TS",".TTA",".VLT",".VOC","VP6","VQF","VRO",".WAV",".WEBM",".WV",".XESC",".ZAB"
};

        public static void Main(string[] args)
        {
            Application.Init();
            win = new MainWindow();
            win.ShowAll();
            Application.Run();
        }

        //Refresh, can only be done once each instance as I have no idea how to clear the array used in process
        public static async System.Threading.Tasks.Task RefresAsync()
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), "http://localhost:9980/renter/files?cached=false"))
                {
                    request.Headers.TryAddWithoutValidation("User-Agent", "Sia-Agent");           
                    HttpResponseMessage response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();                                 
                    json = await response.Content.ReadAsStringAsync();
                }
            }
        }

        //where the magic happens
        public static void Process()
        {
            mediafiles.Clear();
            string[] files = json.Split(new[] { split }, StringSplitOptions.None);

            foreach (string s in files)
            {
                bool b = s.Contains("\"available\":true");
                   
                if (b == true)
                { 
                    string path = s.Substring(0, s.LastIndexOf(cutafter) + 0);
                    path = path.Substring(path.LastIndexOf(cutbefore));
                    path = path.Replace("\"siapath\":\"", "");

                    bool c = IsMediaFile(path);

                    if (c == true)
                    {
                        mediafiles.Add(path);
                    }
                }
            }
        }

        //Check if file is a media file
        static bool IsMediaFile(string path)
        {
            return -1 != Array.IndexOf(mediaExtensions, Path.GetExtension(path).ToUpperInvariant());
        }
    }
    }





