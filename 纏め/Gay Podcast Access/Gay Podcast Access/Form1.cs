using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Gay_Podcast_Access
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        // UI部品設定
        private void button1_Click(object sender, EventArgs e)
        {
            //フォルダの指定を行う
            // メモの1番
            FolderBrowserDialog file = new FolderBrowserDialog();
            file.Description = "フォルダを指定してください。";
            file.ShowNewFolderButton = true;
            if (file.ShowDialog(this) == DialogResult.OK)
                label3.Text = file.SelectedPath;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && label3.Text != "")
            {
                button2.Enabled = true;
                timer1.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult really = MessageBox.Show("これよりエピソードのダウンロードを行います。\n指定したフォルダーのデータが全て削除されます。\n本当に構いませんか?", "Ready to Download", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (really == DialogResult.OK)
                Download_Portal(textBox1.Text, label3.Text.Replace("\\", "\\\\"), "rss.xml");
        }

        //RSSのダウンロード
        string File_URL_Data = "";

        // ベースポータル
        public void Download_Portal(string web_address, string folder_address, string File_Name)
        {
            string[] Status = new string[2];
            Status = HttpStatus(web_address);
            if (Status[0] == "OK")
            {
                Uri uriResult;
                // URIの有効性の確認
                bool result = Uri.TryCreate(web_address, UriKind.Absolute, out uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                if (result) file_download(web_address, folder_address, File_Name);
            }
        }

        // HTTPステータスの確認
        public string[] HttpStatus(string check_URI)
        {
            string access_url = null;
            string[] access_code = { "ERROR", "ERROR" };

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
                finally { if (webres != null) webres.Close(); }
            }
            else
                MessageBox.Show("有効なURLではありません。", "確認してください", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return access_code;
        }

        // ファイルのダウンロード
        string episode_rss = "";

        private void file_download(string RSS_URI, string Folder_URI, string File_Name)
        {
            WebClient downloadClient = null;
            DeleteFile(Folder_URI);
            File_URL_Data = Folder_URI;
            string File_data = Folder_URI + "\\" + File_Name;
            episode_rss = File_data.Replace("\\\\", "\\");
            Uri u = new Uri(RSS_URI);
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
                DialogResult Episode = MessageBox.Show(
                    "RSSファイルのダウンロードが完了しました\nこれより、エピソードのダウンロードを開始します。",
                    "ダウンロード完了",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                if (Episode == DialogResult.OK)
                {
                    Episode_Download episode_download = new Episode_Download();
                    episode_download.Xml_Extraction(episode_rss, File_URL_Data);
                }
            }
        }

        // ファイルの削除
        public void DeleteFile(string targetDirectoryPath)
        {
            if (!Directory.Exists(targetDirectoryPath)) return;
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