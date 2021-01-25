using Localbanda.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Localbanda.Helpers
{
    public class EmailHelper
    {


        #region Email Helper

        private static IHostingEnvironment _env;
        public EmailHelper(IHostingEnvironment env)
        {
            _env = env;
        }
        public static async Task<bool> SendMail(string toAddress, string subject, string mailContent, bool IsBodyHtml, LocalbandaDbContext _context)
        {

            if (String.IsNullOrEmpty(toAddress))
            {

                return false;
            }
            mailContent = SetLogo(mailContent);
            bool result = false;

            SmtpClient smtpClient = new SmtpClient();

            MailMessage message = new MailMessage();


            try
            {
                smtpClient = GetSMTPClientObject();
                MailAddress senderAddress = new MailAddress(GlobalConfig.SenderAddress);
                message.From = new MailAddress("\"" + "LocalBanda" + "\" <" + senderAddress + ">");
                //message = GetEmails(toAddress, message);
                message.To.Add(toAddress);
                message.Subject = "[LocalBanda ] " + subject;
                message.IsBodyHtml = IsBodyHtml;
                message.Body = mailContent;



                if (!String.IsNullOrEmpty(toAddress))
                {
                    smtpClient.SendMailAsync(message);
                }
                else
                {
                    // GNF.LogFailedEmail(toAddress);
                    return false;

                }

                result = true;
                // GNF.LogSentEmail(toAddress);

            }
            catch (Exception ex)
            { //Try to send using Backup SMTP In the case if SMTP failes
              //if (!SendUsingBackupSMTP(message))
              //{
              //result = false;
              // GNF.LogFailedEmail(toAddress); 

                //}
                GNF.SaveException(ex, _context);

            }

            return result;
        }
        private static string SetLogo(string mailContent)
        {
            try
            {
                string msg = mailContent.Replace("#LogoUrl#", @"<img style='margin:0 auto; height:70px' src='" + EmailConfiguration.Get_LogoUrl.ToString() + @"'/>");

                return msg;
            }
            catch (Exception eX)
            {
                return mailContent;
            }
        }

        #endregion

        #region Configuration
        private static string GetSenderAddress()
        {

            return GlobalConfig.FromAddress;//

        }

        private static SmtpClient GetSMTPClientObject()
        {

            SmtpClient smtpClient = new SmtpClient();

            smtpClient.Host = GlobalConfig.SMTPHost;
            smtpClient.Port = Convert.ToInt32(GlobalConfig.SMTPPort);
            smtpClient.UseDefaultCredentials = EmailConfiguration.UseDefaultCredentials;
            smtpClient.Timeout = 60000;
            smtpClient.Credentials = new NetworkCredential(GlobalConfig.SenderAddress, GlobalConfig.SenderPassword);// GlobalConfig.AppSettings["EmailPassword"]);
            smtpClient.EnableSsl = GlobalConfig.SSL;



            return smtpClient;
        }

        private static SmtpClient GetBackupSMTPClientObject()
        {

            SmtpClient smtpClient = new SmtpClient();

            smtpClient.Host = GlobalConfig.SMTPHost;
            smtpClient.Port = Convert.ToInt32(GlobalConfig.SMTPPort);
            smtpClient.UseDefaultCredentials = EmailConfiguration.UseDefaultCredentials;
            smtpClient.Timeout = 60000;
            smtpClient.Credentials = new NetworkCredential(GlobalConfig.SenderAddress, GlobalConfig.SenderPassword);
            //   smtpClient.EnableSsl = Convert.ToBoolean(EmailConfiguration.SmtpEnableSsl);



            return smtpClient;
        }

        internal static System.Net.Mail.MailMessage GetEmails(string toAddress, System.Net.Mail.MailMessage message)
        {
            message.To.Add(toAddress);
            string CCEmail = EmailConfiguration.EmailCopy;
            if (CCEmail.Trim() != "")
            {
                message.CC.Add(CCEmail);
            }
            string BCCEmail = EmailConfiguration.EmailBlindCopy;
            if (BCCEmail.Trim() != "")
            {
                message.Bcc.Add(BCCEmail);


            }

            return message;
        }

        private static bool SendUsingBackupSMTP(MailMessage message)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient();
                smtpClient = GetBackupSMTPClientObject();
                smtpClient.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion
        public class EmailConfiguration
        {
            public static string EditorEmail = "info@nowgray.com";
            public static string EmailBlindCopy = "";
            public static string EmailCopy = "";
            public static string Get_LogoUrl = GlobalConfig.Get_LogoUrl;
            public static bool IsApplicationUnderDevelopment = false;
            // public static object SmtpEnableSsl = GlobalConfig.SSL;
            public static bool UseDefaultCredentials = false;
        }

    }
}


