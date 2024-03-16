using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BundleImgValidator
{
    static class Core
    {
        private const string _LogName = "MissingFilesLog.txt";
        private static StreamWriter? _logStream;
        public static string ModRootLocation { get; set; }
        public static void LogMissingFile(string name, string fileName)
        {
            _logStream?.WriteLine(name + "\t-> " + fileName);
        }

        public static void PrepareLogStream()
        {
            if (_logStream == null)
            {
                string logPath = Path.Combine(ModRootLocation, _LogName);
                try
                {
                    _logStream = new StreamWriter(new FileStream(logPath, FileMode.Truncate, FileAccess.ReadWrite, FileShare.None));
                }
                catch (Exception ex)
                {
                    _logStream?.Dispose();
                    throw new Exception($"Failed to open {_LogName}: {ex}");
                }
            }
        }
        public static void CloseLogStream()
        {
            _logStream?.Close();
            _logStream?.Dispose();
            _logStream = null;
        }
    }
}
