using Localbanda.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Localbanda.Helpers.enumeration;

namespace Localbanda.Helpers
{
    public class NotificationHelper
    {
        internal static Task<bool> SendNotification_ForgetPassword(Users objUser, LocalbandaDbContext _context)
        {
            EmailTemplates templates = _context.EmailTemplates.FirstOrDefault(x => x.ID == "ForgetPassword");

            var callbackUrl = new Uri(@GlobalConfig.AdminWebUrl + "reset-password?UserID=" + objUser.User_Id + "&code=" + objUser.Password);
            string subject = templates.Subject;
            string body = "";

            subject = subject.Replace("#email#", objUser.Email);

            string message = templates.Body;
            message = message.Replace("#EMAIL#", objUser.Email);
            message = message.Replace("#email#", objUser.Email);
            message = message.Replace("#forgetpasswordLink#", callbackUrl.ToString());
            message = message.Replace("#LoginLink#", callbackUrl.ToString());
            body = message;

            return SaveNotifications(objUser.Email, subject, body, eNotificationType.ForgetPassword, _context, true);
        }
        internal static async Task<bool> SendNotification_InviteWebUser(Users objUser, LocalbandaDbContext _context)
        {
            EmailTemplates templates = await _context.EmailTemplates.FirstOrDefaultAsync(x => x.ID == "InviteWebUser");

            var callbackUrl = new Uri(@GlobalConfig.AdminWebUrl);
            string subject = templates.Subject;
            string body = "";

            subject = subject.Replace("#email#", objUser.Email);

            string message = templates.Body;
            message = message.Replace("#EMAIL#", objUser.Email);
            message = message.Replace("#Email#", objUser.Email);
            message = message.Replace("#email#", objUser.Email);
            message = message.Replace("#Password#", objUser.Password);
            message = message.Replace("#url#", callbackUrl.ToString());
            message = message.Replace("#LoginLink#", callbackUrl.ToString());

            body = message;

            return await SaveNotifications(objUser.Email, subject, body, eNotificationType.ForgetPassword, _context, true);
        }
        internal static async Task<bool> SendNotification_InviteAppUser(Users objUser, LocalbandaDbContext _context)
        {
            EmailTemplates templates = await _context.EmailTemplates.FirstOrDefaultAsync(x => x.ID == "InviteAppUser");

            var callbackUrl = new Uri(@GlobalConfig.MainWebUrl);
            string subject = templates.Subject;
            string body = "";

            subject = subject.Replace("#email#", objUser.Email);
            string message = templates.Body;
            message = message.Replace("#EMAIL#", objUser.Email);
            message = message.Replace("#email#", objUser.Email);
            message = message.Replace("#Email#", objUser.Email);
            message = message.Replace("#Password#", objUser.Password);
            message = message.Replace("#LoginLink#", callbackUrl.ToString());
            message = message.Replace("#url#", callbackUrl.ToString());
            body = message;

            return await SaveNotifications(objUser.Email, subject, body, eNotificationType.ForgetPassword, _context, true);
        }
        internal static async Task<bool> SendNotification_NewUserCreated(Users objUser, LocalbandaDbContext _context)
        {
            EmailTemplates templates = await _context.EmailTemplates.FirstOrDefaultAsync(x => x.ID == "NewUserCreated");

            var url = new Uri(@GlobalConfig.AdminWebUrl);
            string subject = templates.Subject;
            string body = "";



            string message = templates.Body;


            //message = message.Replace("#FristName#", objUser.First_Name);
            //message = message.Replace("#LastName#", objUser.Last_Name);
            message = message.Replace("#Name#", objUser.Full_Name);
            message = message.Replace("#Email#", objUser.Email);

            message = message.Replace("#url#", url.ToString());

            body = message;
            //await EmailHelper.SendMail(objUser.Email, subject, body, true, _context); //
            return await SaveNotifications(objUser.Email, subject, body, eNotificationType.UserRegistration, _context, true);
        }
        public static async Task<bool> SaveNotifications(string EmailTo, string Subject, string EmailBody, eNotificationType NotificationType, LocalbandaDbContext _context, bool NotifyNow = false, EmailTemplates objTemplate = null, Users user = null)
        {
            Email_Notification objEmail = new Email_Notification();
            try
            {

                objEmail.Email_Content = EmailBody;
                objEmail.Subject = Subject;
                objEmail.To_Email = EmailTo;
                objEmail.DTS = DateTime.Now;

                try
                {
                    if (objTemplate != null)
                    {
                        objEmail.From_Email = objTemplate.FromAddress;
                    }
                    if (user != null)
                    {
                        objEmail.User_Id = user.User_Id;
                    }

                }
                catch (Exception ex)
                {

                }
                try
                {
                    if (NotifyNow)
                    {
                        objEmail.Is_Sent = await EmailHelper.SendMail(objEmail.To_Email, objEmail.Subject, objEmail.Email_Content, true, _context);
                    }
                }
                catch (Exception ex)
                {

                }
                await _context.Email_Notification.AddAsync(objEmail);
                _context.Entry(objEmail).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                return objEmail.Is_Sent;
            }

            return objEmail.Is_Sent;
        }
    }
}
