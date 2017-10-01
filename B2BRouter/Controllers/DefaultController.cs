using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Http;
using log4net;
namespace B2BRouter.Controllers
{
    public class DefaultController : ApiController
    {
        private string uriMt100 = System.Configuration.ConfigurationManager.AppSettings["mt100"];
        private string logPath = System.Configuration.ConfigurationManager.AppSettings["logPath"];
        private static readonly ILog Log =
          LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);



        [HttpGet]
        [Route("mt100.asp/1")]
        public string Get()
        {
            
          
            Log.Info("Entered Into: " + "mt100.asp/1 Get");
            return "Alive";

        }
        [Route("mt100.asp")]
        public HttpResponseMessage Post()
        {
            try
            {
                Log.Info("Entered Into: " + "mt100.asp Post");
                HttpWebRequest myrequest = (HttpWebRequest)HttpWebRequest.Create(uriMt100);
                Log.Info("Posting Uri :" + uriMt100);
                myrequest.Method = "POST";
                Log.Info("Request Method"+ myrequest.Method);
                myrequest.ContentType = "application/x-www-form-urlencoded";
                Log.Info("ContentType: " + myrequest.ContentType);
                myrequest.ContentLength = Request.Content.ReadAsStreamAsync().Result.Length;
                Log.Info("Request Content Length: " + myrequest.ContentLength);
                myrequest.Headers.Add("X-Cert: " + ExportToPEM(Request.GetClientCertificate()));
                Log.Info("Client Certificate X-Cert: " + ExportToPEM(Request.GetClientCertificate()));
                
                using (Stream ds = myrequest.GetRequestStream())
                {
                    ds.Write(Get(Request.Content.ReadAsStreamAsync().Result), 0, Convert.ToInt32(Request.Content.ReadAsStreamAsync().Result.Length));
                }
                Log.Info("Request has been written to stream");
                Log.Info("Request has been posted to " + uriMt100);
                return Get(myrequest.GetResponse());

              
            }
            catch (Exception e)
            {
                Log.Fatal(e);
               
            }

            return null;

        }
        private static byte[] Get(Stream strm)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = strm.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        private static HttpResponseMessage Get(WebResponse w)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            response.Content = new StreamContent(w.GetResponseStream());
            Log.Info("Response Content Length: "+ response.Content.ReadAsStreamAsync().Result.Length);
            
            return response;
        }
        public static string ExportToPEM(X509Certificate cert)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("-----BEGIN CERTIFICATE-----");
            builder.AppendLine(Convert.ToBase64String(cert.Export(X509ContentType.Cert), Base64FormattingOptions.None));
            builder.AppendLine("-----END CERTIFICATE-----");

            return builder.ToString();
        }
    }
}
