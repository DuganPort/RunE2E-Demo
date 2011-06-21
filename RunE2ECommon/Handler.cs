using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Collections;
using System.Xml;

namespace RunE2ECommon
{
    public class Handler
    {

        public void Main()
        {
            List<SoapRequest> oSoapRequests = GetSoapRequests();
            foreach (SoapRequest oSoapRequest in oSoapRequests)
            {
                WebRequest oRequest =
                WebRequest.Create(oSoapRequest.Url);
                oRequest.Method = "POST";
                oRequest.ContentType = "text/xml";
                NetworkCredential oCredentials = new NetworkCredential("theport", "theportis");
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                oRequest.Credentials = oCredentials;
                byte[] bData = encoding.GetBytes(oSoapRequest.RequestBody);
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
                    }
                }

                oRequest = null;
                oResponse = null;
                oReader = null;

                XmlDocument oResponseXml = new XmlDocument();
                oResponseXml.LoadXml(sResponseBody);
            }
        }


        public void LogActivity(XmlDocument ActivityXml)
        {
            //Get the User to whom activities will be assigned.
            WebRequest oUserRequest = WebRequest.Create(
                @"http://services.theport.com/REST/V1/Users/1968941?devkey=dedabf54c268d476f2e75db49207ab91b0bb3503f479d444fb5dedea817da66a5954def58cf8cd1d");
            oUserRequest.Method = "GET";
            HttpWebResponse oUserResponse;
            StreamReader oUserReader;
            string sUserXml;
            try
            {
                oUserResponse = (System.Net.HttpWebResponse)oUserRequest.GetResponse();
                oUserReader = new StreamReader(oUserResponse.GetResponseStream());
                sUserXml = oUserReader.ReadToEnd();
            }
            catch (System.Net.WebException we)
            {
                if (we.Response == null) { sUserXml = we.ToString(); }
                else
                {
                    HttpWebResponse oWebResponse = (System.Net.HttpWebResponse)we.Response;
                    oUserReader = new StreamReader(we.Response.GetResponseStream());
                    sUserXml = oUserReader.ReadToEnd();
                }
            }
            oUserReader = null;
            oUserRequest = null;
            oUserResponse = null;

            XmlDocument oUserXml = new XmlDocument();
            oUserXml.LoadXml(sUserXml.Replace(
                @" xmlns=""http://services.theport.com"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""",
                ""));

            string sLoggedActivities = oUserXml.SelectSingleNode("User")
                .SelectSingleNode("CustomFields")
                .SelectSingleNode("CustomField[ID=4814]")
                .SelectSingleNode("Value").InnerText;
            if (sLoggedActivities != "")
            {
                string[] aGuids = sLoggedActivities.Split(Convert.ToChar(","));
                foreach (string sGuid in aGuids)
                {
                    //If we haven't yet logged the Guid of this particular entry, then this is a new Activity.  Huzzah.
                    if (0 == 1)
                    {
                        string sXml =
                        @"
                           <Post>
                                  <Action>9</Action>
                                  <Parameters>
                                        <ActivityFeed>
                                              <ActivityTypeID>46</ActivityTypeID>
                                              <UserID>1968941</UserID>
                                              <ActivityDate>" + System.DateTime.Now.ToString() + @"</ActivityDate>
                                              <ActivityXml>" + ActivityXml.OuterXml + @"</ActivityXml>
                                        </ActivityFeed>
                                  </Parameters>
                            </Post>
                        ";
                        WebRequest oRequest =
                            WebRequest.Create(
                            @"http://services.theport.com/REST/V1/Users?devkey=dedabf54c268d476f2e75db49207ab91b0bb3503f479d444fb5dedea817da66a5954def58cf8cd1d&sysuserid=1968941");
                        oRequest.Method = "POST";
                        oRequest.ContentType = "text/xml";
                        System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                        byte[] bData = encoding.GetBytes(sXml);
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
                            }
                        }

                        oRequest = null;
                        oResponse = null;
                        oReader = null;
                    }

                    //Update the User's custom field node
                 oUserXml.SelectSingleNode("User")
                 .SelectSingleNode("CustomFields")
                 .SelectSingleNode("CustomField[ID=4814]")
                 .SelectSingleNode("Value").InnerText += "," + "!!!!!STUB; REPLACE WITH ACTUAL GUID!!!!!";

                 string sUserUpdateXml =
                     @"
                           <Post>
                                  <Action>2</Action>
                                  <Parameters>" + oUserXml.OuterXml + @"
                                  </Parameters>
                            </Post>
                        ";



                    WebRequest oUserUpdateRequest =
                           WebRequest.Create(
                           @"http://services.theport.com/REST/V1/Users?devkey=dedabf54c268d476f2e75db49207ab91b0bb3503f479d444fb5dedea817da66a5954def58cf8cd1d&sysuserid=1968941");
                    oUserUpdateRequest.Method = "POST";
                    oUserUpdateRequest.ContentType = "text/xml";
                    System.Text.ASCIIEncoding userupdateencoding = new System.Text.ASCIIEncoding();
                    byte[] bUserUpdateData = userupdateencoding.GetBytes(sUserUpdateXml);
                    oUserUpdateRequest.ContentLength = bUserUpdateData.Length;
                    Stream oUserUpdateStream = oUserUpdateRequest.GetRequestStream();
                    oUserUpdateStream.Write(bUserUpdateData, 0, bUserUpdateData.Length);
                    oUserUpdateStream.Close();
                    HttpWebResponse oUserUpdateResponse;
                    StreamReader oUserUpdateReader;
                    string sUserUpdateResponseBody;
                    try
                    {
                        oUserUpdateResponse = (System.Net.HttpWebResponse)oUserUpdateRequest.GetResponse();
                        oUserUpdateReader = new StreamReader(oUserUpdateResponse.GetResponseStream());
                        sUserUpdateResponseBody = oUserUpdateReader.ReadToEnd();

                    }
                    catch (System.Net.WebException we)
                    {

                    }
                    oUserUpdateRequest = null;
                    oUserUpdateResponse = null;
                    oUserUpdateReader = null;

                }
            }
        }

        public static List<SoapRequest> GetSoapRequests()
        {
            List<SoapRequest> oRequests = new List<SoapRequest>();
            SoapRequest oAccountRequest = new SoapRequest();
            oAccountRequest.Url = "http://crmdemo.rune2e.com/sap/bc/srt/rfc/sap/z_la_crm_accounts/200/z_la_crm_accounts/z_la_crm_accounts";
            oAccountRequest.RequestBody = @"
                <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:sap-com:document:sap:soap:functions:mc-style"">
                   <soapenv:Header/>
                   <soapenv:Body>
                      <urn:ZLaCrmAccounts>
                         <Function>SRH</Function>
                      </urn:ZLaCrmAccounts>
                   </soapenv:Body>
                </soapenv:Envelope>
                ";
            oAccountRequest.PayloadNode = "";

            SoapRequest oActivitiesRequest = new SoapRequest();
            oActivitiesRequest.Url = "http://crmdemo.rune2e.com/sap/bc/srt/rfc/sap/z_la_crm_actvities/200/z_la_crm_actvities/z_la_crm_actvities";
            oActivitiesRequest.RequestBody = @"
                <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:sap-com:document:sap:soap:functions:mc-style"">
                   <soapenv:Header/>
                   <soapenv:Body>
                      <urn:ZLaCrmActivities>
                        <Function>SRH</Function>
                        <Guid>DE6FFE73DD7E44F19F97005056AB3A65</Guid>
                      </urn:ZLaCrmActivities>
                   </soapenv:Body>
                </soapenv:Envelope>
                ";
            oActivitiesRequest.PayloadNode = "";

            SoapRequest oLeadsRequest = new SoapRequest();
            oLeadsRequest.Url = "http://crmdemo.rune2e.com/sap/bc/srt/rfc/sap/z_la_crm_lead/200/z_la_crm_lead/z_la_crm_lead";
            oLeadsRequest.RequestBody = @"
                <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:sap-com:document:sap:soap:functions:mc-style"">
                   <soapenv:Header/>
                    <soapenv:Body>
                        <urn:ZLaCrmLead>
                            <Function>SRH</Function>
                            <Guid>DE70AD15BFA04AF19F97005056AB3A65</Guid>
                        </urn:ZLaCrmLead>
                    </soapenv:Body>
                </soapenv:Envelope>
                ";
            oLeadsRequest.PayloadNode = "";

            SoapRequest oProductsRequest = new SoapRequest();
            oProductsRequest.Url = "http://crmdemo.rune2e.com/sap/bc/srt/rfc/sap/z_la_crm_products/200/z_la_crm_products/z_la_crm_products";
            oProductsRequest.RequestBody = @"
                <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:sap-com:document:sap:soap:functions:mc-style"">
                      <soapenv:Header/>
                    <soapenv:Body>
                        <urn:ZLaCrmProducts></urn:ZLaCrmProducts>
                    </soapenv:Body>
                </soapenv:Envelope>
                ";
            oProductsRequest.PayloadNode = "";


            SoapRequest oOpportunitiesRequest = new SoapRequest();
            oOpportunitiesRequest.Url = "http://crmdemo.rune2e.com/sap/bc/srt/rfc/sap/z_la_crm_opportunity/200/z_la_crm_opportunity/z_la_crm_opportunity";
            oOpportunitiesRequest.RequestBody = @"
                <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:sap-com:document:sap:soap:functions:mc-style"">
                      <soapenv:Header/>
                   <soapenv:Body>
                      <urn:ZLaCrmOpportunity><Function>SRH</Function></urn:ZLaCrmOpportunity>
                   </soapenv:Body>
                </soapenv:Envelope>
                ";
            oProductsRequest.PayloadNode = "";


            oRequests.Add(oAccountRequest);
            oRequests.Add(oActivitiesRequest);
            oRequests.Add(oLeadsRequest);
            oRequests.Add(oProductsRequest);
            oRequests.Add(oOpportunitiesRequest);


            return oRequests;
        }
    }

    public class SoapRequest
    {
        public string Url { get; set; }
        public string RequestBody { get; set; }
        public string PayloadNode { get; set; }
        public long ExternalActivityID { get; set; }
    }
}
