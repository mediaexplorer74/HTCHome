using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTCHome.Core
{
    public static class Logger
    {
        public static void Log(string s)
        {
            if (!File.Exists(Environment.LogsPath + "\\log.txt"))
                File.WriteAllText(Environment.LogsPath + "\\log.txt", "");
            try
            {
                File.AppendAllText(Environment.LogsPath + "\\log.txt",  DateTime.Now + " -------------- " + (char)(13) + (char)(10) + "OS: " + System.Environment.OSVersion.VersionString + (char)(13) + (char)(10) + s + (char)(13) + (char)(10));
            }
            catch { }
        }
    }
}
