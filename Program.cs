using System;
using System.IO;
using System.Configuration;
using System.Runtime.InteropServices;
using static System.IO.File;

namespace File_Tracker
{
    class Program
    {
        #region Dll Imports
        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int cmdSHow);
        #endregion

        #region Main Method
        static void Main(string[] args)
        {
            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();

            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
            fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;

            fileSystemWatcher.Path = GetDriveName();
            fileSystemWatcher.IncludeSubdirectories = true;
            fileSystemWatcher.EnableRaisingEvents = true;

            HideWindow();
            Console.ReadKey();
        }
        #endregion

        #region Helper Method
        private static void HideWindow()
        {
            IntPtr hwnd = GetConsoleWindow();
            if (hwnd != IntPtr.Zero)
                ShowWindow(hwnd, 0);
        }
        private static void LogEntry(string fullPath, string action, string description)
        {
            string logFilePath = @"E:\LogFile.txt";
            string entryContent = "=> " + action + " | " + fullPath + " | " + description + " |"
                + DateTime.Now.ToString() + " |" + Environment.NewLine;
            AppendAllText(logFilePath, entryContent);
        }
        private static string GetDriveName()
        {
            string driveLetter = ConfigurationManager.AppSettings.Get("drive");
            return driveLetter.ToUpper() + @":\";
        }
        #endregion

        #region Events
        /// <summary>
        /// Event will fire when any file renamed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
            => LogEntry(e.FullPath, "RENAME ", "A file has been renamed from " + e.OldName + " to " + e.Name);

        /// <summary>
        /// Event will fire when any file delated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
            => LogEntry(e.FullPath, "DELETE ", "A file has beed deleted - " + e.Name);

        /// <summary>
        /// Event will fire when any file gets changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
            => LogEntry(e.FullPath, "CHANGED", "A file content has been changed - " + e.Name);

        /// <summary>
        /// Event will fire when any new file created.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
            => LogEntry(e.FullPath, "CREATED", "A new file has been created - " + e.Name + ".");
        #endregion
    }
}
