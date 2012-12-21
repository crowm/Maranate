using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Utils
{
    public class ExternalEditor
    {
        public static string EditStringInExternalEditor(string tempFilename, string data)
        {
            if (tempFilename == string.Empty) return string.Empty;
            if (data == string.Empty) return string.Empty;

            string path = MakeTempPath(tempFilename);
            string modifiedString;

            var editProcess = OpenInExternalEditor(path, data);

            // todo: probably use some file handles library instead of WaitForExit
            editProcess.WaitForExit();

            using (FileStream outStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader sw = new StreamReader(outStream))
                {
                    modifiedString = sw.ReadToEnd();
                }
            }

            //File.Delete(path);

            return modifiedString;
        }

        public static void OpenStringInExternalEditor(string tempFilename, string data)
        {
            if (tempFilename == string.Empty) return;
            if (data == string.Empty) return;

            string path = MakeTempPath(tempFilename);

            OpenInExternalEditor(path, data);
        }

        private static Process OpenInExternalEditor(string path, string data)
        {
            using (FileStream outStream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                using (StreamWriter sw = new StreamWriter(outStream))
                {
                    sw.Write(data);
                }
            }

            //if (notepad++ installed)
            //"'C:/Program Files/Notepad++/notepad++.exe' -multiInst -notabbar -nosession -noPlugin"

            return Process.Start(@"notepad.exe", path);
        }

        private static string MakeTempPath(string tempFilename, bool generate = true)
        {
            if (generate)
            {
                return System.IO.Path.GetTempFileName();
            }
            else
            {
                string path = System.Environment.GetEnvironmentVariable("TEMP");
                path += @"\SqlUpdate\" + MakeValidFileName(tempFilename);

                FileInfo fi = new FileInfo(path);
                if (fi.Directory.Exists == false)
                    fi.Directory.Create();

                return path;
            }
        }

        private static string MakeValidFileName(string name)
        {
            string invalidChars = Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidReStr = string.Format(@"[{0}]+", invalidChars);
            return Regex.Replace(name, invalidReStr, "");
        }
    }
}