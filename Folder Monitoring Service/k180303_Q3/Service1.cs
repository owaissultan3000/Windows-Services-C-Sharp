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
using System.Threading.Tasks;
using System.Timers;

namespace k180303_Q3
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

            DirectoryWatcher();

            timmer.Elapsed += new ElapsedEventHandler(TimmerWrapper); // adding Event
            //timmer.Enabled = true;
            //timmer.Start();
        }
        private void TimmerWrapper(object sender, ElapsedEventArgs e)
        {

            DirectoryWatcher();
        }

        protected override void OnStop()
        {
            timmer.Enabled = false;
        }
        
        public void DirectoryWatcher()
        {
            try
            {
                string ExePath = ConfigurationManager.AppSettings["ExePath"];


                Process.Start(ExePath);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
