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
        public void episodeDownload_Portal(string RSS_File)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(RSS_File);


            try // 2. XMLから特定のタグを検索
            {
                XmlElement xmlElement = xmlDocument.DocumentElement;
                XmlNodeList xmlNodeList = xmlElement.GetElementsByTagName("enclosure");
                if (xmlNodeList.Count != 0)
                {
                    // 3.XMLのタグから属性を抽出
                    string[] episode_url = new string[xmlNodeList.Count];
                    for (int i = 0; i < xmlNodeList.Count; i++)
                    {
                        XmlElement element = (XmlElement)xmlNodeList.Item(i);
                        episode_url[i] = element.GetAttribute("url"); ;
                    }
                    //Message(string_add(urls), "Tags", MessageBoxIcon.Information);
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
            }
            catch (XmlException Exception) //Exception発生時のエラーメッセ表示
            {
                MessageBox.Show("Error: " + Exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
