using Localbanda.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Localbanda.Helpers
{
    public class PushHelper
    {
        internal static async Task NotifyToAdminForNewPost(LocalbandaDbContext _context, int postId)
        {
            try
            {
                if (postId > 0)
                {
                    Posts objPosts = new Posts();
                    objPosts = _context.Posts.Find(postId);
                    if (objPosts != null)
                    {
                        //Url For a Single Item 
                        string ServiceUrl = String.Format("https://onesignal.com/api/v1/notifications");
                        string QueryString = "";

                        string FinalUrl = ServiceUrl + QueryString;
                        int TotalNumberOfPages = 0;

                        string json = "{\"app_id\": \"a71269aa-d47b-41a6-9435-e656abc49b52\"," +
                                    "\"include_player_ids\": [" +
                                    "\"d3f00240-02a9-4e7e-9f89-5346e1e2211a\",\"69a9b6b6-2a6c-4c7f-922b-5cb47a54b7d9\"" +
                                    "], \"data\": { \"foo\": \"fdsfsdfsdfsdf\" }," +
                                    "\"contents\": { \"en\": \"" + "New Job Post: " + objPosts.Body + "\"} }";

                        byte[] bytes;
                        bytes = System.Text.Encoding.ASCII.GetBytes(json);
                        //Http request 
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(FinalUrl);
                        request.Headers.Add("Content-Type", "application/json");
                        request.Headers.Add("Authorization", "Basic OWMxMzFhZGUtNTQxZi00ZmUxLTkyNTMtMjJlMDUyOGQ0NWM4");
                        request.Accept = "application/json";
                        request.ContentType = "application/json; encoding='utf-8'";
                        request.ContentLength = bytes.Length;
                        request.Method = "POST";

                        Stream writer = request.GetRequestStream();
                        writer.Write(bytes, 0, bytes.Length);
                        writer.Close();
                        using (System.IO.Stream s = request.GetResponse().GetResponseStream())
                        {
                            using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                            {

                                var reader = sr.ReadToEnd();
                                dynamic dynJson = JsonConvert.DeserializeObject(reader);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GNF.SaveException(ex, _context);
            }
        }

        internal static void NotifyToUsersDaily(List<Users> users, LocalbandaDbContext _context, string msg)
        {
            try
            {

                if (users != null)
                {

                    string Ids = String.Join(",", users.Where(x=> !string.IsNullOrEmpty(x.Device_Id)).Select(x => "\"" + x.Device_Id + "\""));
                    //Url For a Single Item 
                    string ServiceUrl = String.Format("https://onesignal.com/api/v1/notifications");
                    string QueryString = "";

                    string FinalUrl = ServiceUrl + QueryString;
                    int TotalNumberOfPages = 0;

                    string json = "{\"app_id\": \"a71269aa-d47b-41a6-9435-e656abc49b52\"," +
                                "\"include_player_ids\": [" + Ids + "], \"data\": { \"foo\": \"fdsfsdfsdfsdf\" }," +
                                "\"contents\": { \"en\": \"" + msg + "\"} }";

                    byte[] bytes;
                    bytes = System.Text.Encoding.ASCII.GetBytes(json);
                    //Http request 
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(FinalUrl);
                    request.Headers.Add("Content-Type", "application/json");
                    request.Headers.Add("Authorization", "Basic OWMxMzFhZGUtNTQxZi00ZmUxLTkyNTMtMjJlMDUyOGQ0NWM4");
                    request.Accept = "application/json";
                    request.ContentType = "application/json; encoding='utf-8'";
                    request.ContentLength = bytes.Length;
                    request.Method = "POST";

                    Stream writer = request.GetRequestStream();
                    writer.Write(bytes, 0, bytes.Length);
                    writer.Close();
                    using (System.IO.Stream s = request.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {

                            var reader = sr.ReadToEnd();
                            dynamic dynJson = JsonConvert.DeserializeObject(reader);

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                GNF.SaveException(ex, _context);
            }
        }
    }
}
