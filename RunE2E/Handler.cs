using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
namespace RunE2E
{
    public class Handler
    {

        public void DoIt()
        {
            WebRequest oRequest = 
                WebRequest.Create(
                    @"http://crmdemo.rune2e.com/sap/bc/srt/rfc/sap/z_la_crm_lead/200/z_la_crm_lead/z_la_crm_lead");
            oRequest.Method = "POST";
            oRequest.ContentType = "text/xml";
            NetworkCredential oCredentials = new NetworkCredential("theport", "theportis");
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            oRequest.Credentials = oCredentials;
            string RequestBody = 
                @"
                <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:sap-com:document:sap:soap:functions:mc-style"">
                   <soapenv:Header/>
                   <soapenv:Body>
                      <urn:ZLaCrmLead>
                <Function>0</Function>
                      </urn:ZLaCrmLead>
                   </soapenv:Body>
                </soapenv:Envelope>
                ";
            byte[] bData = encoding.GetBytes(RequestBody);
            oRequest.ContentLength = bData.Length;
            Stream oStream = oRequest.GetRequestStream();
            oStream.Write(bData, 0, bData.Length);
            oStream.Close();
            HttpWebResponse oResponse;
            StreamReader oReader;
            string sResponseBody;
            try
            {
                oResponse = (System.Net.HttpWebResponse)oRequest.GetResponse();
                oReader = new StreamReader(oResponse.GetResponseStream());
                sResponseBody = oReader.ReadToEnd();
            }
            catch (System.Net.WebException we)
            {
                if (we.Response == null) { sResponseBody = we.ToString(); }
                else
                {
                    HttpWebResponse oWebResponse = (System.Net.HttpWebResponse)we.Response;
                    oReader = new StreamReader(we.Response.GetResponseStream());
                    sResponseBody = oReader.ReadToEnd();
                    //this.ResponseStatus = we.Response.htttStatus;
                }
            }
            oRequest = null;
            oResponse = null;
            oReader = null;

        }
        


    }
}
