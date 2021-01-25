
using Localbanda.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Localbanda.Helpers.enumeration;

namespace Localbanda.Helpers
{
    public class GNF
    {
        private readonly LocalbandaDbContext _context;
        private static IHostingEnvironment _env;
        public GNF(IHostingEnvironment env)
        {
            _env = env;
        }

        public static string RandomReferenceNo(int length)
        {
            const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder res = new System.Text.StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public static class eCategory
        {
            public const string
                Garbage_Recycling = "Garbage & Recycling",
                Debris_Graffiti_Vandalism = "Debris, Graffiti & Vandalism",
                Signage_Street_Lights = "Signage & Street Lights",
                Parks_Trees_Grass = "Parks, Trees & Grass",
                Road_Curb_Sidewalks = "Road, Curb & Sidewalks",
                Water = "Water";
        }
        public static class eTaskStatus
        {
            public const string
                Pending = "Pending",
                Completed = "Completed",
                Open = "Open";

        }
        public static class eApprovalStatus
        {
            public const string
                InReview = "InReview",
                Rejected = "Rejected",
                Approved = "Approved";
        }
        public static class ePriority
        {
            public const string
                High = "High",
                Medium = "Medium",
                Low = "Low";
        }
        public static class eStatusColor
        {
            public const string
                Open = "#137eff",
                Pending = "#ffd14d",
                Completed = "#5bc146";
        }
        public static class eApprovalStatusColor
        {
            public const string
                Rejected = "#137eff",
                InReview = "#ffd14d",
                Approved = "#5bc146";
        }
        public static void SaveException(Exception ex, LocalbandaDbContext _context)
        {
            try
            {
                ExceptionLog objEexception = new ExceptionLog();

                objEexception.Level = eLevel.Low.ToString();
                //objEexception.Logger = Request.AppRelativeCurrentExecutionFilePath;
                objEexception.Thread = ex.StackTrace;
                objEexception.Message = ex.Message;
                objEexception.Exception = ex.ToString();
                objEexception.Context = ex.Source;
                objEexception.date = DateTime.Now;
                _context.ExceptionLog.Add(objEexception);
                _context.Entry(objEexception).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                _context.SaveChanges();

                string html = "";
                html += "<table width='100%' style='font-size:12px' border='1' cellpadding='10'><tr>";
                html += "<td width='150px'><b>Message:</b></td><td>" + objEexception.Message + "</td>";
                html += "</tr><tr>";

                html += "<td><b>Level:</b></td><td>" + objEexception.Level + "</td>";
                html += "</tr><tr>";
                html += "<td><b>Logger:</b></td><td>" + objEexception.Logger + "</td>";
                html += "</tr><tr>";
                html += "<td><b>Thread:</b></td><td>" + objEexception.Thread + "</td>";
                html += "</tr><tr>";
                html += "<td><b>Exception:</b></td><td>" + objEexception.Exception + "</td>";
                html += "</tr><tr>";
                html += "<td><b>Context:</b></td><td>" + objEexception.Context + "</td>";
                html += "</tr></table>";


                string msg = "<h5>Following error occured</h5> ";

                EmailHelper.SendMail("askdev@nowgray.com", "Critical!!!!! Exception Occured", msg + html, true, _context);

            }
            catch (Exception exe)
            {
                // GNF.SaveException(ex, _context);
            }
        }

        internal static void SaveException(string ex, string mobileAppException, LocalbandaDbContext _context)
        {
            try
            {
                ExceptionLog objEexception = new ExceptionLog();

                objEexception.Level = eLevel.Low.ToString();
                //objEexception.Logger = Request.AppRelativeCurrentExecutionFilePath;
                objEexception.Thread = "";
                objEexception.Message = mobileAppException;
                objEexception.Exception = ex;
                objEexception.Context = ex;
                objEexception.date = DateTime.Now;
                _context.ExceptionLog.Add(objEexception);
                _context.Entry(objEexception).State = EntityState.Added;
                _context.SaveChanges();

                string html = "";
                html += "<table width='100%' style='font-size:12px' border='1' cellpadding='10'><tr>";
                html += "<td width='150px'><b>Message:</b></td><td>" + mobileAppException + "</td>";
                html += "</tr><tr>";

                html += "<td><b>Level:</b></td><td>" + objEexception.Level + "</td>";
                html += "</tr><tr>";
                html += "<td><b>Logger:</b></td><td>" + objEexception.Logger + "</td>";
                html += "</tr><tr>";
                html += "<td><b>Thread:</b></td><td>" + objEexception.Thread + "</td>";
                html += "</tr><tr>";
                html += "<td><b>Exception:</b></td><td>" + objEexception.Message + "</td>";
                html += "</tr><tr>";
                html += "<td><b>Context:</b></td><td>" + objEexception.Context + "</td>";
                html += "</tr></table>";


                string msg = "<h5>Following error occured</h5> ";

                EmailHelper.SendMail("askdev@nowgray.com", "Critical!!!!! Exception Occured", msg + html, true, _context);

            }
            catch (Exception exe)
            {
                //GNF.SaveException(ex, _context);
            }
        }

        //public static void SaveUpdateServiceRequest(ServicesReportResponse objResponse, LocalbandaDbContext _context)
        //{
        //    try
        //    {


        //        string html = "";
        //        html += "<table width='100%' style='font-size:12px' border='1' cellpadding='10'><tr>";
        //        html += "<td width='150px'><b>Address:</b></td><td>" + objResponse.tasks.Address + "</td>";
        //        html += "</tr><tr>";

        //        html += "<td><b>Reference No:</b></td><td>" + objResponse.tasks.Reference_No + "</td>";
        //        html += "</tr><tr>";
        //        html += "<td><b>Comment:</b></td><td>" + objResponse.tasks.Comment + "</td>";
        //        html += "</tr><tr>";
        //        html += "<td><b>Priority:</b></td><td>";

        //        if (objResponse.tasks.Priority == "High")
        //        {
        //            html += "<span style='background-color:" + GlobalConfig.ePriorityColor.High + ";'>" + objResponse.tasks.Priority + "</span>";
        //        }
        //        else
        //       if (objResponse.tasks.Priority == "Medium")
        //        {
        //            html += "<span style='background-color:" + GlobalConfig.ePriorityColor.Medium + ";'>" + objResponse.tasks.Priority + "</span>";
        //        }
        //        else
        //       if (objResponse.tasks.Priority == "Low")
        //        {
        //            html += "<span style='background-color:" + GlobalConfig.ePriorityColor.Low + ";'>" + objResponse.tasks.Priority + "</span>";
        //        }
        //        html += "</td>";

        //        html += "</tr><tr>";
        //        // html += "<td><b>Email Id:</b></td><td>" + objResponse.Services_Request.Email_Id + "</td>";
        //        html += "</tr><tr>";
        //        html += "<td><b>Category:</b></td><td>" + objResponse.tasks.Category + "</td>";
        //        html += "</tr><tr>";
        //        if (objResponse.Photos != null && objResponse.Photos.Count > 0)
        //        {
        //            html += "<td><b>Photos:</b></td><td> ";
        //            foreach (Photos obj in objResponse.Photos)
        //            {
        //                html += "<img width='200px' src='" + obj.Raw_Url + "'/><br>";
        //            }
        //            html += "</td>";
        //        }
        //        else
        //        {
        //            html += "<td><b>Photos:</b></td><td>nothing found</td>";
        //        }

        //        html += "</tr><tr>";
        //        html += "<td><b>Status:</b></td><td>";
        //        if (objResponse.tasks.Status == eTaskStatus.Open)
        //        {
        //            html += "<span style='background-color:" + eStatusColor.Open + ";'>" + objResponse.tasks.Status + "</span>";
        //        }
        //        else
        //        if (objResponse.tasks.Status == eTaskStatus.Pending)
        //        {
        //            html += "<span style='background-color:" + eStatusColor.Pending + ";'>" + objResponse.tasks.Status + "</span>";
        //        }
        //        else
        //        if (objResponse.tasks.Status == eTaskStatus.Completed)
        //        {
        //            html += "<span style='background-color:" + eStatusColor.Completed + ";'>" + objResponse.tasks.Status + "</span>";
        //        }

        //        html += "</td>";
        //        html += "</tr><tr>";
        //        html += "<td><b>Approval Status:</b></td><td>";
        //        if (objResponse.tasks.Approval_Status == eApprovalStatus.InReview)
        //        {
        //            html += "<span style='background-color:" + eApprovalStatusColor.InReview + ";'>" + objResponse.tasks.Approval_Status + "</span>";
        //        }
        //        else
        //        if (objResponse.tasks.Status == eApprovalStatus.Approved)
        //        {
        //            html += "<span style='background-color:" + eApprovalStatusColor.Approved + ";'>" + objResponse.tasks.Approval_Status + "</span>";
        //        }
        //        else
        //        if (objResponse.tasks.Status == eApprovalStatus.Rejected)
        //        {
        //            html += "<span style='background-color:" + eApprovalStatusColor.Rejected + ";'>" + objResponse.tasks.Approval_Status + "</span>";
        //        }

        //        html += "</td>";
        //        html += "</tr></table>";

        //        string msg = "<h5>New task added</h5> ";

        //        // EmailHelper.SendMail("support@localbanda.in", "", msg + html, true);
        //        EmailHelper.SendMail(GlobalConfig.supportAdmin, "New task added!", msg + html, true, _context);

        //    }
        //    catch (Exception ex)
        //    {
        //        objResponse.Message = "Exception: " + ex.Message; objResponse.Status = Messages.APIStatusError; GNF.SaveException(ex, _context);
        //    }
        //}


        //internal static async Task UserEmail(ServicesResponse objResponse, string email_Id, LocalbandaDbContext _context, IHostingEnvironment _hostingEnvironment, bool StatusChanged = false)
        //{
        //    if (objResponse.tasks == null)
        //    {
        //        GNF.SaveException(new Exception("Problem found in userEmail - GNF - Null object supplied"), _context);
        //        return;
        //    }
        //    var file = "";
        //    try
        //    {
        //        try
        //        {
        //            string projectRootPath = _hostingEnvironment.ContentRootPath;
        //            string folderpath = @"/EmailTemplate/";
        //            file = Path.Combine(projectRootPath + folderpath, "EmailTemplate.html");
        //            DirectoryInfo di = new DirectoryInfo(projectRootPath + folderpath);
        //            if (!di.Exists)
        //            {
        //                di.Create();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            GNF.SaveException(ex, _context);


        //        }
        //        if (System.IO.File.Exists(file) && objResponse.tasks != null)
        //        {
        //            string message = System.IO.File.ReadAllText(file);
        //            message = message.Replace("#email#", email_Id);
        //            message = message.Replace("#ReferrenceNumber#", objResponse.tasks.Reference_No);
        //            message = message.Replace("#Category#", objResponse.tasks.Category);
        //            message = message.Replace("#comment#", objResponse.tasks.Comment);
        //            message = message.Replace("#location#", objResponse.tasks.Address);
        //            message = message.Replace("#time#", Convert.ToString(objResponse.tasks.DTS));
        //            message = message.Replace("#priority#", objResponse.tasks.Priority);
        //            message = message.Replace("#status#", objResponse.tasks.Status);
        //            message = message.Replace("#Approval#", objResponse.tasks.Approval_Status);
        //            if (objResponse.tasks.Priority == "High")
        //                message = message.Replace("#prioritycolor#", GlobalConfig.ePriorityColor.High);
        //            if (objResponse.tasks.Priority == "Medium")
        //                message = message.Replace("#prioritycolor#", GlobalConfig.ePriorityColor.Medium);
        //            if (objResponse.tasks.Priority == "Low")
        //                message = message.Replace("#prioritycolor#", GlobalConfig.ePriorityColor.Low);

        //            string html = "";
        //            Photos photos = _context.Photos.Where(x => x.Task_Id == objResponse.tasks.Task_Id).FirstOrDefault();
        //            if (photos != null)
        //            {
        //                message = message.Replace("#imageUrl#", photos.Thumb_Url);
        //                // html += "<td><b>Photos:</b></td><td> <img width='200px' src='" + objResponse.Photos[0].Raw_Url + "'/></td>";
        //            }
        //            else
        //            {
        //                message = message.Replace("#imageUrl#", "http://localbanda.in/dummy.png");
        //            }
        //            string body = message;
        //            string subject = StatusChanged ? "Your task #" + objResponse.tasks.Reference_No + " updated." : "Your request submitted successfully.";
        //            EmailHelper.SendMail(email_Id, subject, body, true, _context);
        //        }
        //        else
        //        {
        //            GNF.SaveException(new Exception("Email file not found."), _context);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GNF.SaveException(ex, _context);
        //    }
        //}
        public static string RandomPassword(int length)
        {
            const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new System.Text.StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }



        public static int GenerateRandomNo()
        {
#if DEBUG
            //Test Mode
            return 1234;//
#endif
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        //internal static async Task APP_Notification(LocalbandaDbContext _context, _ServicesRequest services, string Status)
        //{
        //    try
        //    {

        //        Tasks objServices = services.tasks;
        //        AppNotification objNotification = new AppNotification();
        //        objNotification.App_Id = objServices.App_Id;
        //        objNotification.Business_Id = objServices.Business_Id;
        //        objNotification.Reference_No = objServices.Reference_No;
        //        objNotification.Notified_On = DateTime.Now;
        //        objNotification.Notification_Text = "Status changed to " + objServices.Status;// + " for REF# " + objServices.Reference_No;
        //        objNotification.Notification_Type = "App";
        //        objNotification.Current_Status = objServices.Status;
        //        objNotification.Is_Notified = false;
        //        objNotification.Request_Id = objServices.Task_Id;
        //        objNotification.Genrated_On = DateTime.Now;

        //        //  objNotification.Email = objServices.Email_Id;
        //        objNotification.Current_Status = Status;
        //        _context.AppNotification.Add(objNotification);
        //        _context.Entry(objNotification).State = Microsoft.EntityFrameworkCore.EntityState.Added;
        //        await _context.SaveChangesAsync();




        //        //JavaScriptSerializer json_serializer = new JavaScriptSerializer();
        //        try
        //        {
        //            if (!string.IsNullOrEmpty(objNotification.App_Id))
        //            {
        //                //Url For a Single Item 
        //                string ServiceUrl = String.Format("https://onesignal.com/api/v1/notifications");
        //                string QueryString = "";

        //                string FinalUrl = ServiceUrl + QueryString;
        //                int TotalNumberOfPages = 0;

        //                string json = "{\"app_id\": \"4bd00269-52d2-43e7-90a5-9dfc3a0f69fd\"," +
        //                            "\"include_player_ids\": [" +
        //                            "\"" + objNotification.App_Id + "\"" +
        //                            "], \"data\": { \"foo\": \"fdsfsdfsdfsdf\" }," +
        //                            "\"contents\": { \"en\": \"" + objNotification.Notification_Text + " for REF# " + objServices.Reference_No + "\"} }";

        //                byte[] bytes;
        //                bytes = System.Text.Encoding.ASCII.GetBytes(json);
        //                //Http request 
        //                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(FinalUrl);
        //                request.Headers.Add("Content-Type", "application/json");
        //                request.Headers.Add("Authorization", "Basic OWMxMzFhZGUtNTQxZi00ZmUxLTkyNTMtMjJlMDUyOGQ0NWM4");
        //                request.Accept = "application/json";
        //                request.ContentType = "application/json; encoding='utf-8'";
        //                request.ContentLength = bytes.Length;
        //                request.Method = "POST";

        //                Stream writer = request.GetRequestStream();
        //                writer.Write(bytes, 0, bytes.Length);
        //                writer.Close();
        //                using (System.IO.Stream s = request.GetResponse().GetResponseStream())
        //                {
        //                    using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
        //                    {

        //                        var reader = sr.ReadToEnd();
        //                        dynamic dynJson = JsonConvert.DeserializeObject(reader);

        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            GNF.SaveException(ex, _context);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        GNF.SaveException(ex, _context);
        //    }
        //}

        //internal static void NotifiyUser(Users objUser, string message, LocalbandaDbContext _context)
        //{
        //    try
        //    {


        //        UsersDevices ud = _context.UsersDevices.OrderByDescending(x => x.DTS).FirstOrDefault(x => x.User_Id == objUser.User_Id);

        //        if (ud != null && !string.IsNullOrEmpty(ud.UUID) && (string.IsNullOrEmpty(ud.LastMessage) || ud.LastMessage.ToLower() != message.ToLower()))
        //        {
        //            ud.LastMessage = message;
        //            _context.UsersDevices.Add(ud);
        //            _context.Entry(ud).State = EntityState.Modified;
        //            _context.SaveChanges();

        //            //Url For a Single Item 
        //            string ServiceUrl = String.Format("https://onesignal.com/api/v1/notifications");
        //            string QueryString = "";

        //            string FinalUrl = ServiceUrl + QueryString;
        //            int TotalNumberOfPages = 0;

        //            string json = "{\"app_id\": \"4bd00269-52d2-43e7-90a5-9dfc3a0f69fd\"," +
        //                        "\"include_player_ids\": [" +
        //                        "\"" + ud.UUID + "\"" +
        //                        "], \"data\": { \"foo\": \"fdsfsdfsdfsdf\" }," +
        //                        "\"contents\": { \"en\": \" " + message + "\"} }";

        //            byte[] bytes;
        //            bytes = System.Text.Encoding.ASCII.GetBytes(json);
        //            //Http request 
        //            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(FinalUrl);
        //            request.Headers.Add("Content-Type", "application/json");
        //            request.Headers.Add("Authorization", "Basic OWMxMzFhZGUtNTQxZi00ZmUxLTkyNTMtMjJlMDUyOGQ0NWM4");
        //            request.Accept = "application/json";
        //            request.ContentType = "application/json; encoding='utf-8'";
        //            request.ContentLength = bytes.Length;
        //            request.Method = "POST";

        //            Stream writer = request.GetRequestStream();
        //            writer.Write(bytes, 0, bytes.Length);
        //            writer.Close();
        //            using (System.IO.Stream s = request.GetResponse().GetResponseStream())
        //            {
        //                using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
        //                {

        //                    var reader = sr.ReadToEnd();
        //                    dynamic dynJson = JsonConvert.DeserializeObject(reader);

        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
    }
}


