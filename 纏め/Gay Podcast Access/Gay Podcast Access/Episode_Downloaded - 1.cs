using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Gay_Podcast_Access
{
    class Episode_Downloaded
    {
        int download_complete = 0, download_all;

        public void episodeDownload_Portal(string RSS_File, string Folder_URI)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(RSS_File);

            try
            {
                XmlElement xmlElement = xmlDocument.DocumentElement;
                XmlNodeList xmlNodeList_enclosure = xmlElement.GetElementsByTagName("enclosure");
                XmlNodeList xmlNodeList_title = xmlElement.GetElementsByTagName("title");
                string[] episode_url = new string[xmlNodeList_enclosure.Count];
                string[] episode_title = new string[xmlNodeList_title.Count];
                bool[] catch_data = new bool[2];

                // エピソードURLの取得
                if (xmlNodeList_enclosure.Count != 0)
                {
                    for (int i = 0; i < xmlNodeList_enclosure.Count; i++) // URLの取得
                    {
                        XmlElement element = (XmlElement)xmlNodeList_enclosure.Item(i);
                        episode_url[i] = element.GetAttribute("url");
                        catch_data[0] = true;
                    }
                    download_all = xmlNodeList_enclosure.Count;
                }
                else
                {
                    DialogResult choose = MessageBox.Show("\"enclosure\"タグが存在しません。RSSファイルを確認してください。\nRSSファイルを開きますか?", "タグが見つかりません", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (choose == DialogResult.Yes)
                    {
                        System.Diagnostics.Process P = System.Diagnostics.Process.Start(RSS_File);
                        bool result = P.Start();
                    }
                }

                // titleの取得
                if (xmlNodeList_title.Count != 0)
                {
                    for (int i = 0; i < xmlNodeList_title.Count; i++)
                    {
                        episode_title[i] = xmlNodeList_title.Item(i).InnerText;
                        catch_data[1] = true;
                    }
                }
                else
                {
                    DialogResult choose = MessageBox.Show("\"title\"タグが存在しません。RSSファイルを確認してください。\nRSSファイルを開きますか?", "タグが見つかりません", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (choose == DialogResult.Yes)
                    {
                        System.Diagnostics.Process P = System.Diagnostics.Process.Start(RSS_File);
                        bool result = P.Start();
                    }
                }

                if (catch_data[0] == true && catch_data[1] == true)
                {
                    for (int i = 0; i < episode_url.Length; i++)
                    {
                        string[] httpstatus_code = new string[2];
                        httpstatus_code = HttpStatus(episode_url[i]);
                        if (httpstatus_code[0] == "OK" && httpstatus_code[1] == "OK")
                            Download_check(episode_url[i], Folder_URI, Episode_Title_CATCH(i, episode_url, episode_title));
                    }
                }
            }
            catch (XmlException Exception) //Exception発生時のエラーメッセ表示
            {
                MessageBox.Show("Error: " + Exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // HTTPステータスの確認
        public string[] HttpStatus(string check_URI)
        {
            string access_url = null;
            string[] access_code = new string[2];

            //URIの有効性の確認
            Uri uriResult;
            bool result = Uri.TryCreate(check_URI, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (result)
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                HttpWebRequest webreq = (HttpWebRequest)WebRequest.Create(check_URI);
                HttpWebResponse webres = null;
                try
                {
                    webres = (HttpWebResponse)webreq.GetResponse();
                    access_url = webres.ResponseUri.ToString();
                    access_code[0] = webres.StatusCode.ToString();
                    Console.WriteLine("HTTPStatus_Response: " + webres.StatusCode + ", " + webres.StatusDescription);
                    access_code[1] = webres.StatusDescription;
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        HttpWebResponse errres = (HttpWebResponse)ex.Response;
                        access_url = errres.ResponseUri.ToString(); access_code[0] = errres.StatusCode.ToString();
                        access_code[1] = errres.StatusDescription;
                    }
                }
                finally
                {
                    //閉じる
                    if (webres != null) webres.Close();
                }

            }
            else
            {
                MessageBox.Show("有効なURLではありません。", "確認してください", MessageBoxButtons.OK, MessageBoxIcon.Error);
                access_code[0] = "Error";
                access_code[1] = "Error";
            }
            return access_code;
        }

        private string Episode_Title_CATCH(int i, string[] episode_url, string[] episode_title)
        {
            int index = episode_url[i].LastIndexOf(".");
            return (i + 1).ToString("D3") + "_" + episode_title[0] + " - " + episode_title[(episode_title.Length - episode_url.Length + i)] + episode_url[i].Substring(index);
        }

        // ファイルのダウンロード
        public void Download_check(string RSS_URI, string Folder_URI, string File_Name)
        {
            WebClient downloadClient = null;
            string episode_rss = "";
            Uri uriResult;
            // URIの有効性の確認
            bool result = Uri.TryCreate(RSS_URI, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            string File_data = Folder_URI + "\\" + File_Name;
            episode_rss = File_data.Replace("\\\\", "\\");
            Uri u = new Uri(RSS_URI);
            downloadClient = new WebClient();
            //イベントハンドラの作成
            downloadClient.DownloadProgressChanged +=
                                    new DownloadProgressChangedEventHandler(
                                        downloadClient_DownloadProgressChanged);
            downloadClient.DownloadFileCompleted +=
                                    new System.ComponentModel.AsyncCompletedEventHandler(
                                        downloadClient_DownloadFileCompleted);
            //非同期ダウンロードを開始する
            downloadClient.DownloadFile(u, File_data);
        }

        private void downloadClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine("{0}% ({1}byte 中 {2}byte) ダウンロードが終了しました。",
                e.ProgressPercentage, e.TotalBytesToReceive, e.BytesReceived);
        }

        private void downloadClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
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
                download_complete++;
                if (download_all != download_complete)
                    MessageBox.Show("ダウンロード完了しました。\n残り" + (download_all - download_complete) + "件", "Download Finish", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("全てのエピソードのダウンロードが完了しました", "Download Complete!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
