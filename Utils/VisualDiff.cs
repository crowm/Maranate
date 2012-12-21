using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;
using RegistryKeyLib;

namespace Utils
{
    public class VisualDiff
    {
        private static Dictionary<DiffPrograms.DiffProgram, DiffPrograms.VisualDiffProgram> _programTypes;
        private static DiffPrograms.VisualDiffProgram _diffProgram;
        private static DiffPrograms.DiffProgram _diffProgramFlag;
        public static DiffPrograms.DiffProgram DiffProgram
        {
            get
            {
                return _diffProgramFlag;
            }
            set
            {
                _diffProgramFlag = value;
                _diffProgram = null;
                DetectDiffProgram();
            }
        }

        static VisualDiff()
        {
            _programTypes = new Dictionary<DiffPrograms.DiffProgram,DiffPrograms.VisualDiffProgram>();
            _programTypes.Add(DiffPrograms.DiffProgram.TortoiseMerge, new DiffPrograms.TortoiseMerge());
            _programTypes.Add(DiffPrograms.DiffProgram.TortoiseUDiff, new DiffPrograms.TortoiseUDiff());
            _programTypes.Add(DiffPrograms.DiffProgram.KDiff, new DiffPrograms.KDiff3());
            _programTypes.Add(DiffPrograms.DiffProgram.WinMerge, new DiffPrograms.WinMerge());
            _programTypes.Add(DiffPrograms.DiffProgram.SourceGearDiffMerge, new DiffPrograms.SourceGearDiffMerge());
        }

        private static void DetectDiffProgram()
        {
            if (_diffProgramFlag != DiffPrograms.DiffProgram.AutoDetect)
            {
                if (_diffProgram == null)
                {
                    _diffProgram = _programTypes[_diffProgramFlag];
                    if (!_diffProgram.IsDetected())
                    {
                        _diffProgramFlag = DiffPrograms.DiffProgram.AutoDetect;
                        _diffProgram = null;
                    }
                }
                else
                {
                    var diffProgram = _programTypes[_diffProgramFlag];
                    if (_diffProgram != diffProgram)
                    {
                        _diffProgramFlag = DiffPrograms.DiffProgram.AutoDetect;
                        _diffProgram = null;
                    }
                }
            }

            if (_diffProgramFlag == DiffPrograms.DiffProgram.AutoDetect)
            {
                // Find first detected program
                foreach (var diffProgram in _programTypes.Keys)
                {
                    _diffProgram = _programTypes[diffProgram];
                    if (_diffProgram.IsDetected())
                    {
                        _diffProgramFlag = diffProgram;
                        break;
                    }
                    _diffProgram = null;
                }
            }

            if (_diffProgramFlag == DiffPrograms.DiffProgram.AutoDetect)
                MessageBox.Show("Could not detect a Diff program to use.", "No Diff program found");
        }

        public static bool IsDetected(DiffPrograms.DiffProgram diffProgram)
        {
            var program = _programTypes[diffProgram];
            return program.IsDetected();
        }

        public static void DiffStrings(string baseString, string newString)
        {
            DiffStrings(baseString, newString, "Base", "New");
        }

        public static void DiffStrings(string baseString, string newString, string baseNiceName, string newNiceName)
        {
            string filename1 = System.IO.Path.GetTempFileName();
            string filename2 = System.IO.Path.GetTempFileName();

            StringBuilder sb = new StringBuilder();
            List<int> goPositions = new List<int>();

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename1))
            {
                sw.Write(baseString);
            }

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename2))
            {
                sw.Write(newString);
            }

            DiffFiles(filename1, filename2, baseNiceName, newNiceName);

            File.Delete(filename1);
            File.Delete(filename2);
        }

        public static void DiffFiles(string baseFile, string newFile)
        {
            DiffFiles(baseFile, newFile, null, null);
        }

        public static void DiffFiles(string baseFile, string newFile, string baseNiceName, string newNiceName)
        {
            if (DiffProgram == DiffPrograms.DiffProgram.AutoDetect)
                DetectDiffProgram();
            if (DiffProgram == DiffPrograms.DiffProgram.AutoDetect)
                return;
            _diffProgram.DiffFiles(baseFile, newFile, baseNiceName, newNiceName);
        }

        public static void MergeFiles(string baseFile, string mineFile, string theirsFile, string mergedFile)
        {
            MergeFiles(baseFile, mineFile, theirsFile, mergedFile, null, null, null, null);
        }

        public static void MergeFiles(string baseFile, string mineFile, string theirsFile, string mergedFile, string baseNiceName, string mineNiceName, string theirsNiceName, string mergedNiceName)
        {
            if (DiffProgram == DiffPrograms.DiffProgram.AutoDetect)
                DetectDiffProgram();
            if (DiffProgram == DiffPrograms.DiffProgram.AutoDetect)
                return;
            _diffProgram.MergeFiles(baseFile, mineFile, theirsFile, mergedFile, baseNiceName, mineNiceName, theirsNiceName, mergedNiceName);
        }



    }

    public class DiffPrograms
    {
        public enum DiffProgram
        {
            AutoDetect,
            TortoiseMerge,
            TortoiseUDiff,
            KDiff,
            WinMerge,
            SourceGearDiffMerge
        }

        public abstract class VisualDiffProgram
        {
            public string ProgramPath { get; set; }
            public string ArgumentsFormatDiff { get; set; }
            public string ArgumentsFormatMerge { get; set; }

            public virtual bool IsDetected()
            {
                if (string.IsNullOrEmpty(ProgramPath))
                    return false;
                return System.IO.File.Exists(ProgramPath);
            }

            public virtual void DiffFiles(string baseFile, string newFile, string baseNiceName, string newNiceName)
            {
                if (string.IsNullOrEmpty(ProgramPath))
                    throw new NullReferenceException("ProgramPath is not set");
                if (string.IsNullOrEmpty(ArgumentsFormatDiff))
                    throw new NullReferenceException("ArgumentsFormatDiff is not set");

                if (string.IsNullOrEmpty(baseFile))
                    throw new ArgumentNullException("baseFile");
                if (string.IsNullOrEmpty(newFile))
                    throw new ArgumentNullException("newFile");
                if (string.IsNullOrEmpty(baseNiceName))
                    baseNiceName = System.IO.Path.GetFileName(baseFile);
                if (string.IsNullOrEmpty(newNiceName))
                    newNiceName = System.IO.Path.GetFileName(newFile);

                Process proc = new Process();
                proc.StartInfo.FileName = ProgramPath;
                proc.StartInfo.Arguments = string.Format(ArgumentsFormatDiff,
                    PathToFullNameAndTestExists(baseFile),
                    PathToFullNameAndTestExists(newFile),
                    baseNiceName,
                    newNiceName);

                proc.Start();

                proc.WaitForExit();
            }

            public virtual void MergeFiles(string baseFile, string mineFile, string theirsFile, string mergedFile, string baseNiceName, string mineNiceName, string theirsNiceName, string mergedNiceName)
            {
                if (string.IsNullOrEmpty(ProgramPath))
                    throw new NullReferenceException("ProgramPath is not set");
                if (string.IsNullOrEmpty(ArgumentsFormatMerge))
                    throw new NullReferenceException("ArgumentsFormatMerge is not set");

                if (string.IsNullOrEmpty(baseFile))
                    throw new ArgumentNullException("baseFile");
                if (string.IsNullOrEmpty(mineFile))
                    throw new ArgumentNullException("mineFile");
                if (string.IsNullOrEmpty(theirsFile))
                    throw new ArgumentNullException("theirsFile");
                if (string.IsNullOrEmpty(mergedFile))
                    throw new ArgumentNullException("mergedFile");

                if (string.IsNullOrEmpty(baseNiceName))
                    baseNiceName = System.IO.Path.GetFileName(baseFile);
                if (string.IsNullOrEmpty(mineNiceName))
                    mineNiceName = System.IO.Path.GetFileName(mineFile);
                if (string.IsNullOrEmpty(theirsNiceName))
                    theirsNiceName = System.IO.Path.GetFileName(theirsFile);
                if (string.IsNullOrEmpty(mergedNiceName))
                    mergedNiceName = System.IO.Path.GetFileName(mergedFile);

                Process proc = new Process();
                proc.StartInfo.FileName = ProgramPath;
                proc.StartInfo.Arguments = string.Format(ArgumentsFormatMerge,
                    PathToFullNameAndTestExists(baseFile),
                    PathToFullNameAndTestExists(mineFile),
                    PathToFullNameAndTestExists(theirsFile),
                    PathToFullNameAndTestExists(mergedFile),
                    baseNiceName,
                    mineNiceName,
                    theirsNiceName,
                    mergedNiceName);

                proc.Start();

                proc.WaitForExit();
            }

            protected static string PathToFullNameAndTestExists(string filename)
            {
                FileInfo fi = new FileInfo(filename);
                if (fi.Exists == false)
                    throw new FileNotFoundException("File not found.", fi.FullName);
                return fi.FullName;
            }

            protected static FileInfo StringToFileInfoAndTestExists(string filename)
            {
                FileInfo fi = new FileInfo(filename);
                if (fi.Exists == false)
                    throw new FileNotFoundException("File not found.", fi.FullName);
                return fi;
            }

            protected class Arguments
            {
                private List<string> _args = new List<string>();

                public void AddArgument(string name, string value)
                {
                    if ((value.Length >= 2) && value.StartsWith("\"") && value.EndsWith("\""))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }
                    _args.Add(name + "\"" + value + "\"");
                }
                public void AddArgument(string name)
                {
                    _args.Add(name);
                }

                public override string ToString()
                {
                    return string.Join(" ", _args.ToArray());
                }
            }

            protected static RegistryKey64 Open64Or32Key(RegistryHive hKey, string subkey)
            {
                RegistryKey64 key;
                key = RegistryKey64.OpenKey(hKey, subkey, false, RegistryKey64.RegWow64Options.KEY_WOW64_64KEY);
                if (key != null)
                    return key;

                key = RegistryKey64.OpenKey(hKey, subkey, false, RegistryKey64.RegWow64Options.KEY_WOW64_32KEY);
                return key;
            }
        }

        public class TortoiseMerge : VisualDiffProgram
        {
            public TortoiseMerge()
            {
                ProgramPath = DetectPath();
                ArgumentsFormatDiff = "/base:\"{0}\" /mine:\"{1}\" /basename:\"{2}\" /minename:\"{3}\" ";
                ArgumentsFormatMerge = "/base:\"{0}\" /mine:\"{1}\" /theirs:\"{2}\" /merged:\"{3}\" /basename:\"{4}\" /minename:\"{5}\" /theirsname:\"{6}\" /mergedname:\"{7}\" ";
            }

            public string DetectPath()
            {
                string path = "";
                RegistryKey64 key = Open64Or32Key(RegistryHive.LocalMachine, @"SOFTWARE\TortoiseSVN");
                if (key != null)
                {
                    try
                    {
                        path = key.GetValue("TMergePath") as string;
                    }
                    finally
                    {
                        key.Close();
                    }
                }
                return path;
            }
            public static string DetectPath_TortoiseProc()
            {
                string path = null;
                RegistryKey64 key = Open64Or32Key(RegistryHive.LocalMachine, @"SOFTWARE\TortoiseSVN");
                if (key != null)
                {
                    try
                    {
                        path = key.GetValue("ProcPath") as string;
                    }
                    finally
                    {
                        key.Close();
                    }
                }
                return path;
            }

            public static void ShowLog(string path, bool waitForCompletion)
            {
                ShowLog(path, -1, -1, waitForCompletion);
            }
            public static void ShowLog(string path, int startRev, int endRev, bool waitForCompletion)
            {
                Process proc = new Process();
                proc.StartInfo.FileName = DetectPath_TortoiseProc();

                var args = new Arguments();

                args.AddArgument("/command:", "log");
                args.AddArgument("/path:", path);
                if (startRev >= 0)
                    args.AddArgument("/startrev:", startRev.ToString());
                if (endRev >= 0)
                    args.AddArgument("/endrev:", endRev.ToString());

                proc.StartInfo.Arguments = args.ToString();
                proc.Start();
                if (waitForCompletion)
                    proc.WaitForExit();
            }
        }

        public class TortoiseUDiff : TortoiseMerge
        {
            private string _detectedPath = null;
            public string DetectUDiff()
            {
                string path = "";
                RegistryKey64 key = Open64Or32Key(RegistryHive.LocalMachine, @"SOFTWARE\TortoiseSVN");
                if (key != null)
                {
                    try
                    {
                        path = key.GetValue("Directory") as string;
                        path += @"bin\TortoiseUDiff.exe";
                    }
                    finally
                    {
                        key.Close();
                    }
                }
                return path;
            }

            public override bool IsDetected()
            {
                if (_detectedPath == null)
                    _detectedPath = DetectUDiff();
                return !string.IsNullOrEmpty(_detectedPath);
            }

            public override void DiffFiles(string baseFile, string newFile, string baseNiceName, string newNiceName)
            {
                if (baseFile == null)
                    throw new ArgumentNullException("baseFile");
                if (newFile == null)
                    throw new ArgumentNullException("newFile");

                Process proc = new Process();
                proc.StartInfo.FileName = _detectedPath;

                string baseFileContents;
                using (var reader = new StreamReader(baseFile))
                {
                    baseFileContents = reader.ReadToEnd();
                }
                string newFileContents;
                using (var reader = new StreamReader(newFile))
                {
                    newFileContents = reader.ReadToEnd();
                }

                var udiff = UnifiedDiff.GenerateUnifiedDiff(baseFileContents, newFileContents, baseNiceName, newNiceName);

                string udiffFilename = System.IO.Path.GetTempFileName();
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(udiffFilename))
                {
                    sw.Write(udiff);
                }

                var args = new Arguments();

                FileInfo udiffFI = StringToFileInfoAndTestExists(udiffFilename);
                args.AddArgument("/patchfile:", udiffFI.FullName);

                if ((baseNiceName != null) && (newNiceName != null))
                    args.AddArgument("/title:", string.Join(", ", new string[] { baseNiceName, newNiceName }));
                else if (baseNiceName != null)
                    args.AddArgument("/title:", string.Join(", ", new string[] { baseNiceName, newFile }));
                else if (newNiceName != null)
                    args.AddArgument("/title:", string.Join(", ", new string[] { baseFile, newNiceName }));
                else
                    args.AddArgument("/title:", string.Join(", ", new string[] { baseFile, newFile }));

                proc.StartInfo.Arguments = args.ToString();

                proc.Start();

                proc.WaitForExit();

                File.Delete(udiffFilename);

            }
        }

        public class KDiff3 : VisualDiffProgram
        {
            public KDiff3()
            {
                ProgramPath = DetectPath();
                ArgumentsFormatDiff = "\"{0}\" -L \"{2}\" \"{1}\" -L \"{3}\" ";
                ArgumentsFormatMerge = "\"{0}\" -L \"{4}\" \"{1}\" -L \"{5}\" \"{2}\" -L \"{6}\" -o \"{3}\" ";
            }

            public static string DetectPath()
            {
                string path = "";
                RegistryKey64 key = Open64Or32Key(RegistryHive.LocalMachine, @"SOFTWARE\KDiff3");
                if (key != null)
                {
                    try
                    {
                        path = key.GetValue("") as string;
                        if (path != null)
                            path += @"\kdiff3.exe";
                    }
                    finally
                    {
                        key.Close();
                    }
                }
                return path;
            }
        }

        public class WinMerge : VisualDiffProgram
        {
            public WinMerge()
            {
                ProgramPath = DetectPath();
                ArgumentsFormatDiff = "/u /dl \"{2}\" -dr \"{3}\" \"{0}\" \"{1}\" ";
                ArgumentsFormatMerge = "/u -dl \"{4}\" -dr \"{5}\" \"{1}\" \"{2}\" \"{3}\" ";
            }

            public static string DetectPath()
            {
                string path = "";
                RegistryKey64 key = Open64Or32Key(RegistryHive.CurrentUser, @"SOFTWARE\Thingamahoochie\WinMerge");
                if (key != null)
                {
                    try
                    {
                        path = key.GetValue("Executable") as string;
                    }
                    finally
                    {
                        key.Close();
                    }
                }
                if (path == null)
                {
                    key = RegistryKey64.OpenKey(RegistryHive.ClassesRoot, @"CLSID\{4E716236-AA30-4C65-B225-D68BBA81E9C2}\InprocServer32", false, RegistryKey64.RegWow64Options.KEY_WOW64_32KEY);
                    if (key != null)
                    {
                        try
                        {
                            path = key.GetValue("") as string;
                            if (path != null)
                                path = path.Substring(0, path.LastIndexOf('\\') + 1) + "WinMergeU.exe";
                        }
                        finally
                        {
                            key.Close();
                        }
                    }
                }
                return path;
            }
        }

        public class SourceGearDiffMerge : VisualDiffProgram
        {
            public SourceGearDiffMerge()
            {
                ProgramPath = DetectPath();
                ArgumentsFormatDiff = "-nosplash -t1=\"{2}\" -t2=\"{3}\" \"{0}\" \"{1}\" ";
                ArgumentsFormatMerge = "-nosplash -merge -result=\"{3}\" -t1=\"{4}\" -t2=\"{5}\" -t3=\"{6}\" \"{0}\" \"{1}\" \"{2}\" ";
            }

            public static string DetectPath()
            {
                string path = @"C:\Program Files\SourceGear\Common\DiffMerge\sgdm.exe";
                string clsid = "{2F410E77-24FD-4788-8412-3810115E7BCB}";
                if (RegistryKey64.Is64BitOperatingSystem())
                    clsid = "{41E0355D-F488-487D-B7BA-D235D5834F1D}";
                RegistryKey64 key = RegistryKey64.OpenKey(RegistryHive.ClassesRoot, @"CLSID\" + clsid + @"\InprocServer32", false, RegistryKey64.RegWow64Options.None);
                if (key != null)
                {
                    try
                    {
                        path = key.GetValue("") as string;
                        if (path != null)
                            path = path.Substring(0, path.LastIndexOf('\\') + 1) + "sgdm.exe";
                    }
                    finally
                    {
                        key.Close();
                    }
                }
                return path;
            }
        }


    }
}
