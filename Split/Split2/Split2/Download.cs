using System;
using System.Net;
using System.Windows.Forms;

namespace Split2
{
    class Download
    {
        String Folder_URI = "", url = "", DL_File_Name = "";
        WebClient downloadClient = null;
        public void url_checker(String url, String DL_File_Name)
        {
            Uri uriResult;
            // URIの有効性の確認
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            url = this.url;
            DL_File_Name = this.DL_File_Name;

            // URIの有効時
            if (result)
            {
                Uri_RSScheck();
            }
            else
            {
                Message("正しいURLを入力してください", "確認してください", "Error");
            }
        }

        private void Uri_RSScheck()
        {
            // URIがRSSを指定しているかどうかを確認
            if ((url.Substring(url.Length - 3)) == "rss")
            {
                Folder_DL_Specified();
            }
            else
            {
                Message("正しいAnchorが提供しているRSSフィードURLではありません", "確認してください", "Error");
            }
        }

        private void Folder_DL_Specified()
        {
            FolderBrowserDialog fbDialog = new FolderBrowserDialog
            {
                // ダイアログの説明文を指定する
                Description = "ダイアログの説明文",
                // デフォルトのフォルダを指定する
                SelectedPath = @"C:",
                // 「新しいフォルダーの作成する」ボタンを表示する
                ShowNewFolderButton = true
            };
            //フォルダを選択するダイアログを表示する
            if (fbDialog.ShowDialog() == DialogResult.OK)
            {
                Folder_URI = fbDialog.SelectedPath;
            }

            if (Folder_URI != "") file_download();
            else
            {
                Message("フォルダ選択中にキャンセルされたか、問題が発生しました", "確認してください", "Error");
            }
        }

        private void file_download()
        {
            //ダウンロードしたファイルの保存先
            string fileName = Folder_URI + "\\" + DL_File_Name + ".rss";
            //ダウンロード元のURL
            Uri u = new Uri(url);

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
            downloadClient.DownloadFileAsync(u, fileName);
        }

        private void downloadClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine("{0}% ({1}byte 中 {2}byte) ダウンロードが終了しました。",
                e.ProgressPercentage, e.TotalBytesToReceive, e.BytesReceived);
        }

        private void downloadClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            String neko = null;
            if (e.Cancelled)
            {
                neko = "キャンセルされました。";
            }
            else if (e.Error != null)
            {
                neko = "エラー: " + e.Error.Message;
            }
            else
            {
                neko = "ダウンロードが完了しました。";
            }

            Message(neko, "ダウンロード", "Information");
        }

        private void Message(String Text, String Heading, String Icon)
        {
            MessageBoxIcon ico = new MessageBoxIcon();
            switch(Icon){
                case "Asterisk":
                    ico = MessageBoxIcon.Asterisk; //円で囲まれた小文字の i から成る記号
                    break;

                case "Error":
                    ico = MessageBoxIcon.Error; //背景が赤の円で囲まれた白い X から成る記号
                    break;

                case "Exclamation":
                    ico = MessageBoxIcon.Exclamation; //背景が黄色の三角形で囲まれた感嘆符から成る記号
                    break;

                case "Hand":
                    ico = MessageBoxIcon.Hand; //背景が赤の円で囲まれた白い X から成る記号
                    break;

                case "Information":
                    ico = MessageBoxIcon.Information; //円で囲まれた小文字の i から成る記号
                    break;

                case "None":
                    ico = MessageBoxIcon.None; //記号を表示しません
                    break;

                case "Question":
                    ico = MessageBoxIcon.Question; //円で囲んだ疑問符から成るシンボルが含まれます。 疑問符は、質問の特定の種類を明確に表さず、メッセージの言い回しはどのメッセージの種類にも適用されるため、疑問符のメッセージ アイコンは推奨されなくなりました。 さらにユーザーは、疑問符シンボルをヘルプ情報シンボルと混同することがあります。 したがって、メッセージ ボックスには疑問符シンボルを使用しないでください。 システムは引き続き、下位互換性のためだけに、その組み込みをサポートします。
                    break;

                case "Stop":
                    ico = MessageBoxIcon.Stop; //背景が赤い円で囲んだ白い X から成る記号
                    break;

                case "Warning":
                    ico = MessageBoxIcon.Warning; //背景が黄色い三角で囲んだ感嘆符から成る記号
                    break;

                default:
                    ico = MessageBoxIcon.None;
                    break;
            }
            MessageBox.Show(Text, Heading, MessageBoxButtons.OK, ico);
        }
    }
}
