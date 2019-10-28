using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Gay_Podcast_Access
{
    class Episode_Download
    {
        /* めもでござりゅ
         * 
         * 動作としては、1つのファイル毎に繰り返す仕様にしたい
         * 
         * RSS.XMLからURLとかいろいろ読み出す
         * ↓
         * ダウンロードするURLとか変数に代入
         * ↓
         * ループ作業
         * ↓
         * URLのHTTPステータスチェック
         * ↓
         * ダウンロード開始するぅ
         * ↓
         * ダウンロード終わったやでって来た
         * ↓
         * 次のURLの設定
         * ↓
         * ループの頭へ
         */

        private void Xml_Extraction(string RSS_File, string File_Saving)
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
    }
}
