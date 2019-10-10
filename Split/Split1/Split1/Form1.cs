using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Split1
{
    public partial class Form1 : Form
    {
        urlaccess access = new urlaccess();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String url = textBox1.Text;
            if (url == "")
                MessageBox.Show("テキストボックス空白です。\nURLを入力してください", "値が無効です", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                access.accessCheck(url);
        }
    }
}
