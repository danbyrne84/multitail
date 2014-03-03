using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTail
{
    class Program
    {
        static void Main(string[] args)
        {
            LogLocations settings = (LogLocations)ConfigurationManager.GetSection("logLocations");

            var app = new MultiTailApp();
            app.Init(settings);

            // any key to quit
            Console.ReadLine();
        }
    }

    public class MultiTailApp
    {
        protected List<string> _logFiles = null;
        protected Dictionary<string, long> _fileSizes = new Dictionary<string, long>();

        public void Init(LogLocations settings)
        {
            _logFiles = settings.Log;
            foreach (var path in _logFiles)
            {                    
                var dir = System.IO.Path.GetDirectoryName(path);

                // connect to UNC path
                if (dir.Contains("\\\\"))
                {
                    ExtremeMirror.PinvokeWindowsNetworking.connectToRemote(dir,
                        ConfigurationManager.AppSettings["username"],
                        ConfigurationManager.AppSettings["password"]
                    );
                }

                // set up watcher for the directory & files
                var fw = new FileSystemWatcher(dir);
                var filter = "";

                if (path.Contains("*."))
                {
                    filter = path.Substring(path.IndexOf("*."));
                    fw.Filter = filter;
                }

                // set up file sizes so we can diff old vs new
                var files = Directory.GetFiles(Path.GetDirectoryName(path), filter);
                SetupFiles(files);

                fw.Changed += fw_Changed;
                fw.EnableRaisingEvents = true;
            }
        }

        public void SetupFiles(string[] files)
        {
            foreach (var file in files)
            {
                var finf = new System.IO.FileInfo(file);
                if (!_fileSizes.ContainsKey(file))
                    _fileSizes.Add(file, finf.Length);
            }
        }

        void fw_Changed(object sender, FileSystemEventArgs e)
        {
            var oldSize = _fileSizes[e.FullPath];
            var finf = new System.IO.FileInfo(e.FullPath);
            _fileSizes[e.FullPath] = finf.Length;

            // read file from oldSize to new size output to console
            var sr = new StreamReader(e.FullPath);
            sr.BaseStream.Position = oldSize;
            var update = sr.ReadToEnd();
            sr.Close();

            // pretty
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.FullPath);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(update);
        }
    }
}
