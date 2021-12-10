using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace k180303_Q2
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
            WriteToFile("RSS Feed service is started at " + DateTime.Now);

            RSSFeed();

            timmer.Elapsed += new ElapsedEventHandler(TimmerWrapper); // adding Event
            timmer.Interval = 300000; // Set your time here 
            timmer.Enabled = true;
            timmer.Start();
        }
        private void TimmerWrapper(object sender, ElapsedEventArgs e)
        {
            WriteToFile("RSS Feed service is recall at " + DateTime.Now);

            RSSFeed();
        }
        protected override void OnStop()
        {
            WriteToFile("RSS Feed service is stopped at " + DateTime.Now);
            timmer.Enabled = false;
        }
        public void WriteToFile(string LogDetails)
        {
            string path = ConfigurationManager.AppSettings["LogDirectory"];

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = path + "\\Q2 Logs.txt";
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
        public void RSSFeed()
        {
            try
            {
                string ExePath = ConfigurationManager.AppSettings["ExePath"];

                WriteToFile("RSS Feed start at " + DateTime.Now);

                Process.Start(ExePath);

                WriteToFile("RSS Feed end at " + DateTime.Now);
            }
            catch (Exception ex)
            {
                WriteToFile(ex.Message + " " + DateTime.Now);
            }
        }
    }
}
