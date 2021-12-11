using k180303_Assignment2.SerializableDictionary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;

namespace k180303_Assignment2.Services
{
    public class DirectoryWatcher
    {
        SerializableDictionary<string, List<DateTime>> DirectoryNewState = new SerializableDictionary<string, List<DateTime>>();
        SerializableDictionary<string, List<DateTime>> DirectoryOldState = new SerializableDictionary<string, List<DateTime>>();


        List<FileInformation> NewlyAddedFiles = new List<FileInformation>();
        List<FileInformation> DeletedFiles = new List<FileInformation>();
        List<FileInformation> UpdatedFiles = new List<FileInformation>();
        string DirectoryPath = string.Empty;
        string TransferPath = string.Empty;
        string LogPath = string.Empty;
        bool CorrectPath = false;
        bool FileDeleted = false;
        bool FileAdded = false;
        bool FileUpdated = false;
        bool IsSixtyMinutes = false;
        bool IsFirstTimeCheking = true;
        TimeSpan WatchingInterval;
        public void GetPath()
        {
            try
            {
                DirectoryPath = ConfigurationManager.AppSettings["MonitorDirectory"];
                TransferPath = ConfigurationManager.AppSettings["FileTransferDirectory"];
                LogPath = ConfigurationManager.AppSettings["LogDirectory"];
                if (!string.IsNullOrEmpty(DirectoryPath) && Directory.Exists(DirectoryPath))
                {
                    CorrectPath = true;
                }
                if (!Directory.Exists(LogPath))
                {
                    Directory.CreateDirectory(LogPath);
                }
                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception Occured " + ex);
            }
      
        }

        public void Watch()
        {
            if (!string.IsNullOrEmpty(DirectoryPath) && !Directory.Exists(DirectoryPath))
            {
                Console.WriteLine("Folder Path Not Found!!");
                return;
            }

            Action action = CompareDirectoryChanges;
            WatchingInterval = new TimeSpan(0, 0, 1, 0);

            ThreadStart start = delegate { RunAfterTimespan(action); };
            Thread thread = new Thread(start);
            Console.WriteLine("Thread has been launched");
            thread.Start();

        }
        public void RunAfterTimespan(Action action)
        {
            try
            {
                while (true)
                {
                    action();
                    if (WatchingInterval.Hours >= 1 && !FileAdded && !FileDeleted && !FileUpdated)
                    {
                        
                        WatchingInterval = new TimeSpan(0, 1, 0, 0);
                        string LogString = "No change recorded 1 hr achieved at time " + DateTime.Now + "\n";
                        Console.WriteLine(LogString);
                        IsSixtyMinutes = true;
                    }

                    else if (WatchingInterval.Hours >= 1 && (FileAdded || FileDeleted || FileUpdated))
                    {
                        
                        WatchingInterval = new TimeSpan(0, 0, 1, 0);
                        string LogString = "timer reset change recorded at time " + DateTime.Now + "\n";
                        Console.WriteLine(LogString);
                        IsSixtyMinutes = false;
                    }

                    Thread.Sleep(WatchingInterval);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void CompareDirectoryChanges()
        {
            GetCurrentStateOfDirectory();
           if (DirectoryOldState.Keys.Count != 0 || DirectoryNewState.Keys.Count > 0)
                CompareStates();
            DirectoryOldState = new SerializableDictionary<string, List<DateTime>>(DirectoryNewState);
            DirectoryNewState.Clear();
        }

        private void CompareStates()
        {

            FileDeleted = false;
            FileAdded = false;
            FileUpdated = false;
            FindDeletedFiles();
            FindNewlyAddedFiles();
            FindModifiedFiles();

            string FilePath = LogPath + "\\Q3 Logs.txt";
            if (!File.Exists(FilePath))
            {
                File.Create(FilePath);
            }

            if ((!FileDeleted && !FileAdded && !FileUpdated && !IsSixtyMinutes)) //no change recorded
            {
                WatchingInterval = WatchingInterval.Add(new TimeSpan(0, 0, 2, 0));
                string LogString = "No change recorded at time " + DateTime.Now + WatchingInterval.TotalSeconds +"\n";
                Console.WriteLine(LogString);
                File.AppendAllText(FilePath, LogString);
            }
            
        }

        private void FindModifiedFiles()
        {
            try
            {
                foreach (string file in DirectoryOldState.Keys)
                {
                    if (DirectoryNewState.ContainsKey(file))
                    {
                        List<DateTime> OldfileInfo = DirectoryOldState[file];
                        List<DateTime> NewFileInfo = DirectoryNewState[file];
                        if (OldfileInfo[1] != NewFileInfo[1])
                        {
                            UpdatedFiles.Add(new FileInformation(file, NewFileInfo[1]));
                            FileUpdated = true;
                            Thread.Sleep(1000);
                            Console.WriteLine("Modified File Found with name " + file + DateTime.Now);
                            IsSixtyMinutes = false;
                            CopyFile(file);

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void FindNewlyAddedFiles()
        {
            try
            {

                foreach (string file in DirectoryNewState.Keys)
                {
                    if (!DirectoryOldState.ContainsKey(file))
                    {
                        List<DateTime> NewFileInfo = DirectoryNewState[file];
                        NewlyAddedFiles.Add(new FileInformation(file, NewFileInfo[0]));
                        FileAdded = true;
                        Thread.Sleep(1000);
                        if (!IsFirstTimeCheking)
                        {
                            Console.WriteLine("Newly Added File Found with name " + file);
                            CopyFile(file);
                        }
                          
                        IsSixtyMinutes = false;
                        

                    }

                }
                IsFirstTimeCheking = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void FindDeletedFiles()
        {
            try
            {
                foreach (string file in DirectoryOldState.Keys)
                {
                    if (!DirectoryNewState.ContainsKey(file))
                    {
                        DeletedFiles.Add(new FileInformation(file));
                        FileDeleted = true;
                        Thread.Sleep(1000);
                        Console.WriteLine("Deleted File Found with name " + file);
                        IsSixtyMinutes = false;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CopyFile(string fileName)
        {
            try
            {
                string source = fileName;
                string[] splitlist = fileName.Split("\\");
                string target = TransferPath + '\\' + splitlist[splitlist.Length - 1];
                if (!Directory.Exists(TransferPath))
                {
                    Directory.CreateDirectory(TransferPath);
                    File.Copy(source, target);
                }
                else
                {
                    if (File.Exists(target))
                    {
                        File.Delete(target);
                        File.Copy(source, target);
                    }
                    else
                    {
                        File.Copy(source, target);
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GetCurrentStateOfDirectory()
        {
            try
            {
                string[] files = Directory.GetFiles(DirectoryPath);
                if(files.Length > 0)
                {
                    foreach (string file in files)
                    {
                        FileInfo fi = new FileInfo(Path.Combine(DirectoryPath, file));
                        List<DateTime> fileInfo = new List<DateTime>();
                        fileInfo.Add(fi.CreationTime);
                        fileInfo.Add(fi.LastWriteTime);
                        DirectoryNewState.Add(file, fileInfo);
                    }
                }
                
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
