using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Utils
{
    public class Path
    {
        private static string _appName = null;
        public static string AppName
        {
            get
            {
                if (_appName == null)
                {
                    string filename = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                    _appName = System.IO.Path.GetFileNameWithoutExtension(filename);
                }
                return _appName;
            }
            set
            {
                _appName = value;
            }
        }

        private static string _appPath = null;
        public static string AppPath
        {
            get
            {
                if (_appPath == null)
                {
                string filename = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                    _appPath = System.IO.Path.GetDirectoryName(filename) + "\\";
            }
                return _appPath;
        }
            set
            {
                _appPath = value;
            }
        }

        private static string _logPath = null;
        public static string LogPath
        {
            get
            {
                if (_logPath == null)
                    _logPath = string.Format(@"{0}\DigitalWatch\{1}\log\", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), AppName);
                return _logPath;
            }
            set
            {
                _logPath = value;
            }
        }

        private static string _settingsPath = null;
        public static string SettingsPath
        {
            get
            {
                if (_settingsPath == null)
                    _settingsPath = string.Format(@"{0}\DigitalWatch\{1}\", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), AppName);
                return _settingsPath;
            }
            set
            {
                _settingsPath = value;
            }
        }

        public static string[] GetCommandLineArgs()
        {
            return GetCommandLineArgs(true);
        }

        public static string[] GetCommandLineArgs(bool includeExe)
        {
            string commandLine = System.Environment.CommandLine.Trim();

            List<string> args = new List<string>();
            int index = 0;
            int nextIndex = 0;

            while (index < commandLine.Length)
            {
                if (commandLine[index] == '"')
                {
                    index++;
                    nextIndex = commandLine.IndexOf("\" ", index);
                    if (nextIndex < 0)
                    {
                        nextIndex = commandLine.Length;
                        if (commandLine.EndsWith("\"") && (nextIndex > index))
                            nextIndex -= 1;
                    }
                    args.Add(commandLine.Substring(index, nextIndex - index));
                    index = nextIndex + 1;
                }
                else
                {
                    nextIndex = commandLine.IndexOf(' ', index);
                    if (nextIndex < 0)
                        nextIndex = commandLine.Length;
                    args.Add(commandLine.Substring(index, nextIndex - index));
                    index = nextIndex;
                }
                while ((index < commandLine.Length) && (commandLine[index] == ' '))
                    index++;
            }

            if (includeExe == false)
            {
                args.RemoveAt(0);
            }

            return args.ToArray();
        }


        [DllImport("shlwapi.dll")]
        private static extern bool PathCompactPathEx([Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags);

        public static string CompactPath(string path, int length)
        {
            StringBuilder sb = new StringBuilder();
            PathCompactPathEx(sb, path, length, 0);
            return sb.ToString();
        }

        public static string CompactPath(string path, System.Drawing.Font font, System.Drawing.Size size)
        {
            var text = string.Copy(path.ToString());
            System.Windows.Forms.TextRenderer.MeasureText(text, font, size, System.Windows.Forms.TextFormatFlags.ModifyString | System.Windows.Forms.TextFormatFlags.PathEllipsis);
            var nullIndex = text.IndexOf('\0');
            if (nullIndex >= 0)
                text = text.Substring(0, nullIndex);
            return text;
        }

    }
}
