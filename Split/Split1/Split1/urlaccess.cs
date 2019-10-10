using System;
using System.Windows.Forms;

namespace Split1
{
    class urlaccess
    {
        public void accessCheck(String url)
        {
            String access_url = null;
            String[] access_code = new String[2];

            //URIの有効性の確認
            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (result)
            {
                //WebRequestの作成
                System.Net.HttpWebRequest webreq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);

                System.Net.HttpWebResponse webres = null;
                try
                {
                    //サーバーからの応答を受信するためのWebResponseを取得
                    webres = (System.Net.HttpWebResponse)webreq.GetResponse();

                    //応答したURIを表示する
                    access_url = webres.ResponseUri.ToString();
                    //応答ステータスコードを表示する
                    access_code[0] = webres.StatusCode.ToString();
                    Console.WriteLine(webres.StatusCode + ", " + webres.StatusDescription);
                    access_code[1] = webres.StatusDescription;
                }
                catch (System.Net.WebException ex)
                {
                    //HTTPプロトコルエラーかどうか調べる
                    if (ex.Status == System.Net.WebExceptionStatus.ProtocolError)
                    {
                        //HttpWebResponseを取得
                        System.Net.HttpWebResponse errres = (System.Net.HttpWebResponse)ex.Response;
                        //応答したURIを表示する
                        access_url = errres.ResponseUri.ToString();
                        //応答ステータスコードを表示する
                        access_code[0] = errres.StatusCode.ToString();
                        access_code[1] = errres.StatusDescription;
                    }
                }
                finally
                {
                    //閉じる
                    if (webres != null)
                        webres.Close();
                }

                MessageBox.Show("接続URL: " + access_url + "\n" + "HTTPステータス: " + access_code[0] + " " + access_code[1], "HTTP接続結果", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("有効なURLではありません。", "確認してください", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}