using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Timers;


namespace k180303_Q1
{
    public partial class Service1 : ServiceBase
    {
        public System.Timers.Timer timmer = new System.Timers.Timer();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteToFile("Email service is started at " + DateTime.Now);

            SendEmail();

            timmer.Elapsed += new ElapsedEventHandler(TimmerWrapper); // adding Event
            timmer.Interval = 900000; // Set your time here 
            timmer.Enabled = true;
            timmer.Start();
        }
        private void TimmerWrapper(object sender, ElapsedEventArgs e)
        {
            WriteToFile("Email service is recall at " + DateTime.Now);

            SendEmail();
        }

        protected override void OnStop()
        {
            WriteToFile("Email service is stopped at " + DateTime.Now);
            timmer.Enabled = false;
        }
        public void WriteToFile(string LogDetails)
        {
            string path = ConfigurationManager.AppSettings["LogDirectory"];

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = path + "\\Q1 Logs.txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(LogDetails);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(LogDetails);
                }
            }
        }
        public void SendEmail()
        {
            try
            {
                string ExePath = ConfigurationManager.AppSettings["ExePath"];

                WriteToFile("Email sending start at " + DateTime.Now);

                Process.Start(ExePath);

                WriteToFile("Email sending end at " + DateTime.Now);
            }
            catch (Exception ex)
            {
                WriteToFile(ex.Message + " " + DateTime.Now);
            }
        }
    }
}
