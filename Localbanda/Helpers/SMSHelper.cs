using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static Localbanda.Helpers.enumeration;

namespace Localbanda.Helpers
{
    public class SMSHelper
    {
        public class SMSObject
        {
            public string uname { get; set; }
            public string password { get; set; }
            public string sender { get; set; }
            public string receiver { get; set; }
            public string route { get; set; }
            public string msgtype { get; set; }
            public string sms { get; set; }
        }
        public static string SendOTP(string mobileNumbers, string messageText, string SMSRouteType = "TA")
        {
            SMSObject objSMS = new SMSObject();
            objSMS.msgtype = eSMSMessageType.Text;
            objSMS.password = "Nowgray@2019";// configs.FirstOrDefault(x => x.Config_Name == "SMSPassword").Config_Value;
            objSMS.receiver = mobileNumbers;// string.Join(",", mobileNumbers);
            objSMS.route = SMSRouteType;
            objSMS.sender = "LCLBND";// configs.FirstOrDefault(x => x.Config_Name == "SMSSenderID").Config_Value;
            objSMS.sms = messageText;
            objSMS.uname = "nowgray2";// configs.FirstOrDefault(x => x.Config_Name == "SMSUserName").Config_Value;

#if DEBUG
            //Test Mode
            return "";//
#endif
            //
            try
            {



                //Url For a Single Item 
                string ServiceUrl = "http://staticking.org/index.php/smsapi/httpapi/";
                string QueryString = "?uname=" + Uri.EscapeDataString(objSMS.uname) + "&password=" + Uri.EscapeDataString(objSMS.password) + "&sender=" + Uri.EscapeDataString(objSMS.sender) + "&receiver=" + Uri.EscapeDataString(objSMS.receiver) + "&route=" + Uri.EscapeDataString(objSMS.route) + "&msgtype=" + Uri.EscapeDataString(objSMS.msgtype) + "&sms=" + Uri.EscapeDataString(objSMS.sms) + "";

                string FinalUrl = ServiceUrl + QueryString;


                // var json = Newtonsoft.Json.JsonConvert.SerializeObject(objSMS);
                // byte[] bytes;
                //var postData = "msgtype=" + Uri.EscapeDataString(objSMS.msgtype);
                //postData += "&password=" + Uri.EscapeDataString(objSMS.password);
                //postData += "&receiver=" + Uri.EscapeDataString(objSMS.receiver);
                //postData += "&route=" + Uri.EscapeDataString(objSMS.route);
                //postData += "&sender=" + Uri.EscapeDataString(objSMS.sender);
                //postData += "&sms=" + Uri.EscapeDataString(objSMS.sms);
                //postData += "&uname=" + Uri.EscapeDataString(objSMS.uname);


                //var data = Encoding.ASCII.GetBytes(postData);
                // bytes = System.Text.Encoding.ASCII.GetBytes(postData);

                //Http request 
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(FinalUrl);

                //request.Accept = "application/json";
                // request.ContentType = "application/x-www-form-urlencoded";
                // request.ContentLength = bytes.Length;
                request.Method = "GET";


                //Stream writer = request.GetRequestStream();
                //writer.Write(bytes, 0, bytes.Length);
                //writer.Close();
                using (System.IO.Stream s = request.GetResponse().GetResponseStream())
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                    {

                        var reader = sr.ReadToEnd();
                        return reader.ToString();
                        //dynamic dynJson = JsonConvert.DeserializeObject(reader);

                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
        private static string SendSMS_StaticKing(List<string> mobileNumbers, string messageText, string SMSRouteType)
        {
            SMSObject objSMS = new SMSObject();
            objSMS.msgtype = eSMSMessageType.Text;
            objSMS.password = "Nowgray@2019";// configs.FirstOrDefault(x => x.Config_Name == "SMSPassword").Config_Value;
            objSMS.receiver = string.Join(",", mobileNumbers);
            objSMS.route = SMSRouteType;
            objSMS.sender = "LCLBND";// configs.FirstOrDefault(x => x.Config_Name == "SMSSenderID").Config_Value;
            objSMS.sms = messageText;
            objSMS.uname = "nowgray2";// configs.FirstOrDefault(x => x.Config_Name == "SMSUserName").Config_Value;

#if DEBUG
            //Test Mode
            return "";//
#endif
            //
            try
            {



                //Url For a Single Item 
                string ServiceUrl = "http://staticking.org/index.php/smsapi/httpapi/";
                string QueryString = "?uname=" + Uri.EscapeDataString(objSMS.uname) + "&password=" + Uri.EscapeDataString(objSMS.password) + "&sender=" + Uri.EscapeDataString(objSMS.sender) + "&receiver=" + Uri.EscapeDataString(objSMS.receiver) + "&route=" + Uri.EscapeDataString(objSMS.route) + "&msgtype=" + Uri.EscapeDataString(objSMS.msgtype) + "&sms=" + Uri.EscapeDataString(objSMS.sms) + "";

                string FinalUrl = ServiceUrl + QueryString;


                // var json = Newtonsoft.Json.JsonConvert.SerializeObject(objSMS);
                // byte[] bytes;
                //var postData = "msgtype=" + Uri.EscapeDataString(objSMS.msgtype);
                //postData += "&password=" + Uri.EscapeDataString(objSMS.password);
                //postData += "&receiver=" + Uri.EscapeDataString(objSMS.receiver);
                //postData += "&route=" + Uri.EscapeDataString(objSMS.route);
                //postData += "&sender=" + Uri.EscapeDataString(objSMS.sender);
                //postData += "&sms=" + Uri.EscapeDataString(objSMS.sms);
                //postData += "&uname=" + Uri.EscapeDataString(objSMS.uname);


                //var data = Encoding.ASCII.GetBytes(postData);
                // bytes = System.Text.Encoding.ASCII.GetBytes(postData);

                //Http request 
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(FinalUrl);

                //request.Accept = "application/json";
                // request.ContentType = "application/x-www-form-urlencoded";
                // request.ContentLength = bytes.Length;
                request.Method = "GET";


                //Stream writer = request.GetRequestStream();
                //writer.Write(bytes, 0, bytes.Length);
                //writer.Close();
                using (System.IO.Stream s = request.GetResponse().GetResponseStream())
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                    {

                        var reader = sr.ReadToEnd();
                        return reader.ToString();
                        //dynamic dynJson = JsonConvert.DeserializeObject(reader);

                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

    }
}
