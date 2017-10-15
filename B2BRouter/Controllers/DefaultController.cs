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
using B2BRouter.App_Code;
using System.Threading;

namespace B2BRouter.Controllers
{
    public class DefaultController : ApiController
    {
        private string uriMt100 = System.Configuration.ConfigurationManager.AppSettings["mt100"];
        private string logPath = System.Configuration.ConfigurationManager.AppSettings["logPath"];
        private static readonly ILog Log =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet]
        [Route("alive")]
        public string Get()
        {

            Log.Info("Entered Into: " + "Alive");
            return "Alive";

        }

        [HttpPost]
        [Route("{Instance}/{method}")]
        public HttpResponseMessage Post(string Instance,string method)
        {
            var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            try
            {
               
                    return Get(Set(UriResolver(method, Instance), Request).GetResponse());
                           
            }
            catch(HttpRequestException hree)
            {
                Log.Fatal(hree);
                response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                return response;
            }
            catch (HttpResponseException hre)
            {
                Log.Fatal(hre);
                response = new HttpResponseMessage(hre.Response.StatusCode);
                return response;
            }
            catch (Exception e)
            {
                Log.Fatal(e);
                return response;
            }
           
        }

        /*

        
        [Route("mt100.asp")]
        public HttpResponseMessage mt100()
        {
            var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            try
            {
                Log.Info("Entered Into: " + "mt100.asp");               
                return Get(Set(UriResolver("mt100.asp"),Request).GetResponse());
            }
            catch (Exception e)
            {
                Log.Fatal(e);
                Log.Info("Exception caused 403  in" + " mt100.asp");
                return response;

            } 
        }

        [Route("mt940.asp")]
        public HttpResponseMessage mt940()
        {
            var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            try
            {
                Log.Info("Entered Into: " + "mt940.asp");
                return Get(Set(UriResolver("mt940.asp"), Request).GetResponse());
            }
            catch (Exception e)
            {
                Log.Fatal(e);
                Log.Info("Exception caused 403  in" + " mt940.asp");
                return response;

            }
        }


        [Route("mt100")]
        public HttpResponseMessage newmt100()
        {
            var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            try
            {
                Log.Info("Entered Into: " + "newmt100");
                return Get(Set(UriResolver("newmt100"), Request).GetResponse());
            }
            catch (Exception e)
            {
                Log.Fatal(e);
                Log.Info("Exception caused 403  in" + " newmt100");
                return response;

            }
        }


        [Route("prmsg.asp")]
        public HttpResponseMessage prmsg()
        {
            var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            try
            {
                Log.Info("Entered Into: " + "prmsg.asp");
                return Get(Set(UriResolver("prmsg.asp"), Request).GetResponse());
            }
            catch (Exception e)
            {
                Log.Fatal(e);
                Log.Info("Exception caused 403  in" + " prmsg.asp");
                return response;

            }
        }

        [Route("prreq.asp")]
        public HttpResponseMessage prreq()
        {
            var response = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            try
            {
                Log.Info("Entered Into: " + "prreq.asp");
                return Get(Set(UriResolver("prreq.asp"), Request).GetResponse());
            }
            catch (HttpResponseException hre)
            {
                Log.Fatal(hre);
                response = new HttpResponseMessage(hre.Response.StatusCode);
                return response;
            }
            catch (Exception e)
            {
                Log.Fatal(e);
                return response;
            }
        }

        [Route("sadad/bill")]
        public HttpResponseMessage sadadbill()
        {
            var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            try
            {
                Log.Info("Entered Into: " + "sadadbill");
                return Get(Set(UriResolver("sadadbill"), Request).GetResponse());
            }
            catch (Exception e)
            {
                Log.Fatal(e);
                Log.Info("Exception caused 403  in" + " sadadbill");
                return response;

            }
        }

        [Route("sadad/moi")]
        public HttpResponseMessage sadadmoi()
        {
            var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            try
            {
                Log.Info("Entered Into: " + "sadadmoi");
                return Get(Set(UriResolver("sadadmoi"), Request).GetResponse());
            }
            catch (Exception e)
            {
                Log.Fatal(e);
                Log.Info("Exception caused 403  in" + " sadadmoi");
                return response;

            }
        }


        [Route("account/balance")]
        public HttpResponseMessage accountbalance()
        {
            var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            try
            {
                Log.Info("Entered Into: " + "accountbalance");
                return Get(Set(UriResolver("accountbalance"), Request).GetResponse());
            }
            catch (Exception e)
            {
                Log.Fatal(e);
                Log.Info("Exception caused 403  in" + " accountbalance");
                return response;

            }
        }


        [Route("payment/stop")]
        public HttpResponseMessage paymentstop()
        {
            var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            try
            {
                Log.Info("Entered Into: " + "paymentstop");
                return Get(Set(UriResolver("paymentstop"), Request).GetResponse());
            }
            catch (Exception e)
            {
                Log.Fatal(e);
                Log.Info("Exception caused 403  in" + " paymentstop");
                return response;

            }
        }
        */
       

        private static HttpWebRequest Set(XProxy xp, HttpRequestMessage m)
        {
            string clientCert = "";
            Log.Info("Received Inbound request to uri" + xp.In);

            HttpWebRequest myrequest = (HttpWebRequest)HttpWebRequest.Create(xp.Out);

            if (m.GetClientCertificate() != null && myrequest.ClientCertificates.Count > 0)
            {
                Log.Info("Inbound request has certificate with serial number: " + System.Text.Encoding.ASCII.GetString(m.GetClientCertificate().GetSerialNumber()));
                clientCert = Base64Encode(ExportToPEM(m.GetClientCertificate()));
                myrequest.Headers.Add(xp.CertHeader + ": " + clientCert);
                myrequest.ClientCertificates.Add(m.GetClientCertificate());
            }



            if (myrequest.ClientCertificates != null && myrequest.ClientCertificates.Count > 0)
            {
                Log.Debug("Outbound Request is having certificate with serial number: " + myrequest.ClientCertificates[0].GetSerialNumber());
                Log.Debug("Outbound request passing certificate in header variable [" + xp.CertHeader + "]: " + myrequest.Headers[xp.CertHeader]);
            }

         

            if (m.Headers.Contains(xp.CertHeader) )
            {
               
                if (m.Headers.GetValues(xp.CertHeader).FirstOrDefault() != null)
                {
                    Log.Info("Inbound request has variable [" + xp.CertHeader + "] in header");
                    myrequest.Headers.Add(xp.CertHeader + ": " + m.Headers.GetValues(xp.CertHeader).FirstOrDefault());
                    Log.Debug("Outbound request passing certificate in header variable [" + xp.CertHeader + "]: " + myrequest.Headers[xp.CertHeader]);
                }
                else
                {
                    Log.Info("Inbound request has no variable [" + xp.CertHeader + "] in header");
                }
               
            }
            else
            {
                Log.Warn("Inbound request has no client certificate," + " no certificate attached to Outbound request in header or request object");
            }

            myrequest.Method = xp.HttpMethod.Trim();
            Log.Debug("Setting Outbound request Verb: " + myrequest.Method);
            myrequest.ContentType = xp.ContentType.Trim();
            Log.Debug("Setting Outboud request ContentType: " + myrequest.ContentType);

            if (Convert.ToInt32(xp.TimeOut.Trim()) == 0)
            {
                myrequest.Timeout = Timeout.Infinite;
                myrequest.ReadWriteTimeout = Timeout.Infinite;
                Log.Debug("Setting Outboud request time out to: Infinite");
                Log.Debug("Setting Outboud ReadWrite time out to: Infinite");
            }
            else
            {
                myrequest.Timeout = Convert.ToInt32(xp.TimeOut.Trim());
                myrequest.ReadWriteTimeout = Convert.ToInt32(xp.TimeOut.Trim());
                Log.Debug("Setting Outboud request time out to: " + myrequest.Timeout + " ms");
                Log.Debug("Setting Outboud ReadWrite time out to: " + myrequest.ReadWriteTimeout + " ms");
            }
            if (string.IsNullOrWhiteSpace(xp.HTTPUserAgent)) { myrequest.UserAgent = xp.HTTPUserAgent; }
            Log.Debug("Setting Outboud request user agent to: " + myrequest.UserAgent);


            if (xp.HTTPKeepAlive) { myrequest.KeepAlive = true; }
            else { myrequest.KeepAlive = false; }
            Log.Debug("Setting Outboud request KeepAlive to: " + myrequest.KeepAlive);

            using (Stream ds = myrequest.GetRequestStream())
            {
                ds.Write(Get(m.Content.ReadAsStreamAsync().Result), 0, Convert.ToInt32(m.Content.ReadAsStreamAsync().Result.Length));
            }
            Log.Debug("Outbound request Content Lenght is: " + myrequest.ContentLength);
            Log.Info("Sending Outbound request to uri" + xp.Out);
            return myrequest;
        }
        private XProxy UriResolver(string inComUri,string instance)
        {
            Mapper m = MapperManager.Load();
            for (int i = 0; i < m.XProxy.Count(); i++)
            {
                if (string.Equals(m.XProxy[i].In, inComUri))
                {
                    if (string.Equals(m.XProxy[i].Instance, instance))
                    {
                        Log.Debug("Incomming Uri : " + m.XProxy[i].In + ", Xproxy forward Uri : " + m.XProxy[i].Out);
                        return m.XProxy[i];
                    } 
                }
            }
            Log.Warn("No xproxy setting found for Uri: " + inComUri);
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
            Log.Info("Received response from server: IsFromCache" + w.IsFromCache+" , Uri: " + w.ResponseUri+" ,Type " + w.ContentType);
            return response;
        }
        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        private static string ExportToPEM(X509Certificate cert)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("-----BEGIN CERTIFICATE-----");
            builder.AppendLine(Convert.ToBase64String(cert.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine("-----END CERTIFICATE-----");

            return builder.ToString();
        }
    }
}
