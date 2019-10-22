using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

// 目的: RSSからURLを取り出す

namespace Split3
{
    public partial class Form1 : Form
    {
        private const bool T = true;
        private const bool F = false;

        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 動作内容: RSSファイル(.rss)を選択する → TextBox1にファイルアドレスを表示する
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog
            {
                //はじめに表示されるフォルダを指定
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                //[ファイルの種類]に表示される選択肢を指定
                Filter = "RSS File(*.rss)|*.rss",
                //タイトルを設定する
                Title = "Choose the RSS fle you want to road",
                //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
                RestoreDirectory = T
            };

            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされたとき、選択されたファイル名を表示する
                textBox1.Text = ofd.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 動作内容: RSSファイルの読み込み → ファイルから特定のタグを検索 → 検索したタグから属性を抽出

            // 1. XMLの読み込み
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(textBox1.Text);


            try // 2. XMLから特定のタグを検索
            {
                XmlElement xmlElement = xmlDocument.DocumentElement;
                XmlNodeList xmlNodeList = xmlElement.GetElementsByTagName("enclosure");
                if (xmlNodeList.Count != 0)
                {
                    // 3.XMLのタグから属性を抽出
                    String[] urls = new String[xmlNodeList.Count];
                    for (int i = 0; i < xmlNodeList.Count; i++)
                    {
                        XmlElement element = (XmlElement)xmlNodeList.Item(i);
                        urls[i] = element.GetAttribute("url"); ;
                    }
                    Message(string_add(urls), "Tags", MessageBoxIcon.Information);
                }
                else
                {
                    DialogResult choose = MessageBox.Show("\"enclosure\"タグが存在しません。RSSファイルを確認してください。\nRSSファイルを開きますか?", "タグが見つかりません", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (choose == DialogResult.Yes)
                    {
                        System.Diagnostics.Process P = System.Diagnostics.Process.Start(textBox1.Text);
                        bool result = P.Start();
                    }
                }
            }
            catch (XmlException Exception) //Exception発生時のエラーメッセ表示
            {
                Message("Error: " + Exception.Message, "Error", MessageBoxIcon.Error);
            }
        }

        public void Message(String mes, String head, MessageBoxIcon icon)
        {
            // 動作内容: メッセージボックスの表示。それだけ。
            MessageBox.Show(mes, head, MessageBoxButtons.OK, icon);
        }

        public String string_add(String[] tags)
        {
            // 動作内容: URLの表示をするために各行毎に改行を入れる
            String return_text = "";
            for (int i = 0; i < tags.Length; i++) return_text = return_text + tags[i] + "\n";
            return return_text;
        }
    }
}
