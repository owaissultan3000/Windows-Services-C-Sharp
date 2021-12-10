using k180303_Assignment2.Services;
using System;

namespace k180303_Assignment2
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryWatcher watcherService = new DirectoryWatcher();
            watcherService.GetPath();
            watcherService.Watch();
        }
    }
}
