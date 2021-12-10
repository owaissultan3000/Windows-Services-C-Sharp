using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Mail;

namespace k180303_Assignment2
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                string email = ConfigurationManager.AppSettings["Email"];
                string password = ConfigurationManager.AppSettings["Password"];
                string FilePath = ConfigurationManager.AppSettings["JsonPath"];
                string[] files = Directory.GetFiles(FilePath, "*.json", SearchOption.AllDirectories);
                List<string> tags = new List<string>();

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress(email);
                foreach (string fileName in files)
                {
                    JObject data = JObject.Parse(File.ReadAllText(fileName));
                    tags.Clear();
                    foreach (JProperty property in data.Properties())
                    {

                        tags.Add(property.Value.ToString());
                    }


                    mail.To.Add(new MailAddress(tags[0]));
                    mail.Subject = tags[1];
                    mail.IsBodyHtml = true;
                    mail.Body = tags[2];

                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(email, password);
                    SmtpServer.EnableSsl = true;

                    SmtpServer.Send(mail);
                    Console.WriteLine("mail Send");

                }
                

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
