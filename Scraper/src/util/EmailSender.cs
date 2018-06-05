using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

using Scraper.Model;

namespace Scraper.Util
{
    class EmailSender
    {
        private static readonly string EMAIL_SETTINGS_FILE_PATH = "Scraper.resources.emailSettings.settings.json";

        public static void SendEmail(string pathToExcelFolder, List<HotelName> hotelNames)
        {
            Dictionary<string, string> emailSettings = GetEmailSettings();
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(emailSettings["smtpClientName"]);
            mail.From = new MailAddress(emailSettings["senderAddress"]);
            mail.To.Add(emailSettings["recipientAddress"]);
            mail.Subject = "Excel Availabilities for " + hotelNames[0].ResortName.Name;
            string bodyContent = "";

            foreach(HotelName hotelName in hotelNames)
            {
                bodyContent += "See attached excel file for up-to-date availability of " + hotelName.GetDisplayName() + "\n";
                Attachment attachment = new Attachment(pathToExcelFolder + @"\" + hotelName.ResortName.Name + @"\" + hotelName.GetDisplayName() + ".xls");
                mail.Attachments.Add(attachment);
            }
            mail.Body = bodyContent;

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(emailSettings["senderAddress"], emailSettings["senderPassword"]);
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);
            Console.WriteLine("Successfully sent email");
        }

        public static void SendEmail(string pathToExcelFolder, HotelName hotelName)
        {
            SendEmail(pathToExcelFolder, new List<HotelName> {{hotelName}});
        }

        public static Dictionary<string, string> GetEmailSettings()
        {
            string fileText;
            Assembly _assembly = Assembly.GetExecutingAssembly();
            using (var streamReader = new StreamReader(_assembly.GetManifestResourceStream(EMAIL_SETTINGS_FILE_PATH)))
            {
                fileText = streamReader.ReadToEnd();
            }
            Dictionary<string, string> root = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileText);

            return root;
        }
    }
}
