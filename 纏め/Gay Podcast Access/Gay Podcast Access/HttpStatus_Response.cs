using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gay_Podcast_Access
{
    class HttpStatus_Response
    {
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
                    else Console.WriteLine("2 Exception: " + ex);
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
    }
}