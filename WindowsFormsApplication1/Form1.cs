using System;
using System.IO;
using System.Net;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using System.Reflection;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
        {
            //Return True to force the certificate to be accepted.
            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtURL.Text))
            {
                txtResponse.Text = "";
                try
                {
                    System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(txtURL.Text);
                    req.Method = "POST";
                    req.PreAuthenticate = true;
                    req.ProtocolVersion = System.Net.HttpVersion.Version10;
                    //req.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(txtLogin.Text + ":" + txtSenha.Text));
                    //req.Credentials = new NetworkCredential("username", "password");

                    var type = req.GetType();
                    var currentMethod = type.GetProperty("CurrentMethod", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(req);

                    var methodType = currentMethod.GetType();
                    methodType.GetField("ContentBodyNotAllowed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(currentMethod, false);

                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                    if (!string.IsNullOrEmpty(txtJson.Text))
                    {
                        byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(txtJson.Text);
                        req.ContentType = "application/json; charset=utf-8";
                        req.ContentLength = byteArray.Length;
                        var dataStream = req.GetRequestStream();
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        dataStream.Close();
                    }

                    System.Net.HttpWebResponse resp = req.GetResponse() as System.Net.HttpWebResponse;
                    System.IO.Stream receiveStream = resp.GetResponseStream();
                    System.Text.Encoding encode = Encoding.GetEncoding("utf-8");
                    // Pipes the stream to a higher level stream reader with the required encoding format. 
                    System.IO.StreamReader readStream = new System.IO.StreamReader(receiveStream, encode);
                    Char[] read = new Char[256];
                    // Reads 256 characters at a time.    
                    int count = readStream.Read(read, 0, 256);
                    while (count > 0)
                    {
                        // Dumps the 256 characters on a string and displays the string to the console.
                        String str = new String(read, 0, count);
                        txtResponse.Text += (str);
                        count = readStream.Read(read, 0, 256);
                    }
                    // Releases the resources of the response.
                    resp.Close();
                    // Releases the resources of the Stream.
                    readStream.Close();
                }
                catch (Exception ex)
                {
                    txtResponse.Text = "Erro ao consumir a API " + txtURL.Text + ". Exceção: " + ex.Message + ". Trace: "+ ex.InnerException;
                }
            }
        }
    }
}
