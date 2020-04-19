using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Authentication;

namespace JasperLIB
{
    public class MyWebPage
    {
        public const SslProtocols _Tls12 = (SslProtocols)0x00000C00;
        public const SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;

        private string m_szHost;
        //---------------------------------------------------------------------
        public MyWebPage(string nameOrAddress)
        {
            m_szHost = nameOrAddress;
        }
        //---------------------------------------------------------------------
        public bool PingHost()
        {
            bool bFlag = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(m_szHost);
                bFlag = reply.Status == IPStatus.Success;
            }
            catch(PingException ex)
            {

            }
            finally
            {
                if( pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return bFlag;
        }
        //---------------------------------------------------------------------
        public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        //---------------------------------------------------------------------
        public String GetResponseString(string szURL, string szPostString)
        {
            System.GC.Collect();

            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(szURL);

            httpRequest.KeepAlive = false;
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            
            byte[] bytedata = Encoding.UTF8.GetBytes(szPostString);
            httpRequest.ContentLength = bytedata.Length;
            HttpWebResponse httpWebResponse = null;            

            try
            {
                // 取得发向服务器的流
                using (Stream requestStream = httpRequest.GetRequestStream())
                {
                    // 使用 POST 方法请求的时候，实际的参数通过请求的 Body 部分以流的形式传送
                    requestStream.Write(bytedata, 0, bytedata.Length);
                    requestStream.Close();
                }

                // GetResponse 方法才真的发送请求，等待服务器返回
                
                httpWebResponse = (HttpWebResponse)httpRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();

                StringBuilder sb = new StringBuilder();
                using (StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        sb.Append(line);
                    }
                }
                httpWebResponse.Close();
                return sb.ToString();
            }
            catch (WebException ex)
            {
                //httpWebResponse.Close();
                return ex.Message;
            }
        }
        //---------------------------------------------------------------------
    }
}
