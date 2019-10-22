using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gay_Podcast_Access
{
    class RSSDownloading
    {
        WebClient downloadClient = null;
        string episode_rss = "";
        public void Download_check(string Network_URI, string Folder_URI, string File_Name)
        {
            try
            {
                Uri uriResult;
                // URIの有効性の確認
                bool result = Uri.TryCreate(Network_URI, UriKind.Absolute, out uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                file_download(Network_URI, Folder_URI, File_Name);
            }
            catch (SystemException e)
            {
                Console.WriteLine("3 Exception: " + e);
            }
        }

        private void file_download(string RSS_URI, string Folder_URI, string File_Name)
        {
            DeleteFile(Folder_URI);
            string File_data = Folder_URI + "\\" + File_Name;
            episode_rss = File_data.Replace("\\\\", "\\");
            Uri u = new Uri(RSS_URI);

            try
            {
                //WebClientの作成
                if (downloadClient == null)
                {
                    downloadClient = new WebClient();
                    //イベントハンドラの作成
                    downloadClient.DownloadProgressChanged +=
                                            new DownloadProgressChangedEventHandler(
                                                downloadClient_DownloadProgressChanged);
                    downloadClient.DownloadFileCompleted +=
                                            new System.ComponentModel.AsyncCompletedEventHandler(
                                                downloadClient_DownloadFileCompleted);
                }
                //非同期ダウンロードを開始する
                downloadClient.DownloadFileAsync(u, File_data);
            }
            catch (SystemException e)
            {

                Console.WriteLine("4 Exception: " + e);
            }
        }

        private void downloadClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine("{0}% ({1}byte 中 {2}byte) ダウンロードが終了しました。",
                e.ProgressPercentage, e.TotalBytesToReceive, e.BytesReceived);
        }

        private void downloadClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            string Text = "", Title = "";
            MessageBoxIcon neko = MessageBoxIcon.Error;
            if (e.Cancelled)
            {
                MessageBox.Show(
                    "ダウンロードがキャンセルされました",
                    "キャンセル動作がありました",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else if (e.Error != null)
            {
                MessageBox.Show(
                    "ダウンロード中にエラーが発生しました。\nエラー内容: " + e.Error.Message,
                    "エラーが発生しました",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                DialogResult Episode = MessageBox.Show(
                    "RSSファイルのダウンロードが完了しました\nこれより、エピソードのダウンロードを開始します。",
                    "ダウンロード完了",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                if (Episode == DialogResult.OK)
                {
                    Episode_Download episode_download = new Episode_Download();
                    episode_download.episodeDownload_Portal(episode_rss);
                }
            }
            MessageBox.Show(Text, Title, MessageBoxButtons.OK, neko);

        }

        public void DeleteFile(string targetDirectoryPath)
        {
            if (!Directory.Exists(targetDirectoryPath))
            {
                return;
            }

            //ディレクトリ以外の全ファイルを削除
            string[] filePaths = Directory.GetFiles(targetDirectoryPath);
            foreach (string filePath in filePaths)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                Console.WriteLine("Delete: " + filePath);
                File.Delete(filePath);
            }
        }
    }
}
