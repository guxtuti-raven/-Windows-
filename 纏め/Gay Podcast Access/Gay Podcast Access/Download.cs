using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Gay_Podcast_Access
{
    class Download
    {
        public void Download_Portal(string web_address, string folder_address, string File_Name)
        {
            HttpStatus_Response HttpStatusResponse = new HttpStatus_Response();
            string[] Status = new string[2];

            try
            {
                Status = HttpStatusResponse.HttpStatus(web_address);
                if (Status[0] == "OK")
                {
                    RSSDownloading rssDownload = new RSSDownloading();
                    rssDownload.Download_check(web_address, folder_address, File_Name);
                }
            }
            catch (System.Net.WebException e)
            {
                Console.WriteLine("1 Exception: " + e);
            }
        }
    }
}
