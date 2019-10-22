using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gay_Podcast_Access
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //フォルダの指定を行う
            // メモの1番
            FolderBrowserDialog file = new FolderBrowserDialog();
            file.Description = "フォルダを指定してください。";
            file.ShowNewFolderButton = true;
            if (file.ShowDialog(this) == DialogResult.OK)
            {
                label3.Text = file.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult really = MessageBox.Show("これよりエピソードのダウンロードを行います。\n指定したフォルダーのデータが全て削除されます。\n本当に構いませんか?", "Ready to Download", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (really == DialogResult.OK)
            {
                //DLを開始する
                Download dl = new Download();
                dl.Download_Portal(textBox1.Text, label3.Text.Replace("\\", "\\\\"), "rss.xml");
            }
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
    }
}
