using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace EFBConnect
{
    class Log
    {
        private static readonly Lazy<Log> instance = new Lazy<Log>(() => new Log());
        public static Log Instance { get { return instance.Value; } }

        private StringBuilder logData;
        public bool ShouldSave = false;

        private Log()
        {
            logData = new StringBuilder();
            var initTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Info($"Logging enabled at {initTime} (version {version})");
        }

        private string AssemblyLoadDirectory
        {
            get { return Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath); }
        }

        private void WriteLine(string level, string area, string s)
        {
            var message = $"[{DateTime.Now:HH:mm:ss.fff}][{level}][{area}] {s}";
            System.Diagnostics.Debug.WriteLine(message);
            logData.AppendLine(message);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public void Debug(string s, [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            var area = $"{Path.GetFileNameWithoutExtension(sourceFilePath)}::{memberName}";
            WriteLine("Debug", area, s);
        }

        public void Info(string s, [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            var area = $"{Path.GetFileNameWithoutExtension(sourceFilePath)}::{memberName}";
            WriteLine("Info", area, s);
        }

        public void Warning(string s, [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
#if DEBUG
            ShouldSave = true;
#endif
            var area = $"{Path.GetFileNameWithoutExtension(sourceFilePath)}::{memberName}";
            WriteLine("Warning", area, s);
        }

        public void Error(string s, bool assert = false, [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            ShouldSave = true;
            var area = $"{Path.GetFileNameWithoutExtension(sourceFilePath)}::{memberName}";
            WriteLine("Error", area, s);
            if (assert)
            {
                throw new Exception(s);
            }
        }

        public void Save()
        {
            var filename = $"Log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
            var path = Path.Combine(AssemblyLoadDirectory, filename);
            using (var dest = new StreamWriter(path))
            {
                dest.Write(logData);
            }
        }

        public void Save(string path)
        {
            using (var dest = new StreamWriter(path))
            {
                dest.Write(logData);
            }
        }

        public void ConditionalSave()
        {
            if (ShouldSave)
            {
                Save();
            }
        }
    }
}
