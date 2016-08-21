using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace JQluv.NetworkScanner.Plugin
{
    public class JQluvWebHandler
    {
        public static string MainUrl = "https://www.prosonmia.com";
        private CookieContainer cookies = new CookieContainer();
        public JQluvWebHandler()
        {

        }
        public string SubmitData(string WebUrl, string formData)
        {
            byte[] byteArray = new ASCIIEncoding().GetBytes(formData);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(WebUrl);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.CookieContainer = cookies;
            webRequest.ContentLength = byteArray.Length;

            Stream writeToServer = webRequest.GetRequestStream();
            writeToServer.Write(byteArray, 0, byteArray.Length);
            writeToServer.Close();

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            StreamReader responseReader = new StreamReader(response.GetResponseStream());
            string returnda = responseReader.ReadToEnd();
            Console.WriteLine("Request: " + formData + Environment.NewLine + "Response: " + returnda + Environment.NewLine);
            return returnda;
        }
    }
}
