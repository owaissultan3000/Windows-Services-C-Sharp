using System;
using System.Collections.Generic;
using System.Text;

namespace k180303_Assignment2.Services
{
    public class FileInformation
    {
        public string FileName { get; set; }
        public DateTime DateOfOperation { get; set; }

        public FileInformation(string filename)
        {
            FileName = filename;
            DateOfOperation = new DateTime();
        }
        public FileInformation(string filename, DateTime operationTime)
        {
            FileName = filename;
            DateOfOperation = operationTime;
        }
    }
}
