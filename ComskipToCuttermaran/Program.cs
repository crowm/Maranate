using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;
using System.Windows.Forms;

namespace ComskipToCuttermaran
{
    class Program
    {
        static bool _consoleCreated = false;

        [STAThreadAttribute()]
        static int Main(string[] args)
        {
            int result = MainProc(args);
            if (_consoleCreated)
            {
                Console.Write("Press any key to exit");
                Console.ReadKey();
            }
            return result;
        }

        static int MainProc(string[] args)
        {
            bool showWindow = false;
            string inputName = null;
            string videoFilename = null;
            string audioFilename = null;
            int frameOffset = 0;

            int state = 0;
            foreach (var arg in args)
            {
                if (arg == "-w")
                {
                    showWindow = true;
                    continue;
                }

                if (state == 0)
                {
                    inputName = Path.GetFullPath(arg);
                    state++;
                }
                else if (state == 1)
                {
                    videoFilename = Path.GetFullPath(arg);
                    state++;
                }
                else if (state == 2)
                {
                    audioFilename = Path.GetFullPath(arg);
                    state++;
                }
                else if (state == 3)
                {
                    frameOffset = int.Parse(arg);
                    state++;
                }
            }

            if (!showWindow)
            {
                _consoleCreated = ActivateConsoleMode();
            }

            if (inputName == null)
            {
                PrintUsage();
                return -1;
            }

            if (videoFilename == null)
            {
                videoFilename = Path.GetDirectoryName(inputName) + "\\" + Path.GetFileNameWithoutExtension(inputName) + ".m2v";
            }
            if (audioFilename == null)
            {
                audioFilename = Path.GetDirectoryName(videoFilename) + "\\" + Path.GetFileNameWithoutExtension(inputName) + ".mp2";
                if (!File.Exists(audioFilename))
                {
                    audioFilename = Path.GetDirectoryName(videoFilename) + "\\" + Path.GetFileNameWithoutExtension(inputName) + ".ac3";
                }
            }

            string extension = Path.GetExtension(inputName);

            if (extension.Equals(".txt", StringComparison.CurrentCultureIgnoreCase))
            {
                return ProcessTxtFile(inputName, videoFilename, audioFilename, frameOffset);
            }
            else if (extension.Equals(".csv", StringComparison.CurrentCultureIgnoreCase))
            {
                if (showWindow)
                    return ShowWindow(inputName, videoFilename, audioFilename, frameOffset);
                else
                    return ProcessCsvFile(inputName, videoFilename, audioFilename, frameOffset);
            }
            else
            {
                Console.WriteLine("Unexpected file extension '" + extension + "'. Please supply either a .txt or .csv file.");
                return -2;
            }
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int pid);

        static bool ActivateConsoleMode()
        {
            if (!AttachConsole(-1))
            {
                AllocConsole();
                return true;
            }
            return false;
        }


        private static void PrintUsage()
        {
            var exeName = Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            Console.WriteLine("Usage: " + exeName + " <comskip.txt> [video.m2v] [audio.mp2/audio.ac3] [frameOffset]");
            Console.WriteLine(" This loads a comskip .txt file and converts it to a cuttermaran cut file.");
            Console.WriteLine();
            Console.WriteLine(" or");
            Console.WriteLine("Usage: " + exeName + " [-w] <comskip.csv> [video.m2v] [audio.mp2/audio.ac3] [frameOffset]");
            Console.WriteLine(" This loads a comskip .csv file, tries to detect commercials,");
            Console.WriteLine(" and then writes a cuttermaran cut file.");
            Console.WriteLine(" -w : Show window to help with tuning.");
        }

        private static int ProcessTxtFile(string inputName, string videoFilename, string audioFilename, int frameOffset)
        {
            string filename = Path.GetFileNameWithoutExtension(inputName);
            string outputName = Path.GetDirectoryName(inputName) + "\\" + filename + ".cpf";
            using (var input = new StreamReader(inputName))
            {
                string line;
                line = input.ReadLine();
                if (line == null)
                {
                    Console.WriteLine("Unexpected EOF: missing line 1");
                    return -2;
                }

                string[] parts = line.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                int totalFrames;
                if (!int.TryParse(parts[3], out totalFrames))
                {
                    Console.WriteLine("The 4th word of the line is not the total number of frames");
                    Console.WriteLine("line: " + line);
                    return -3;
                }
                
                line = input.ReadLine();
                if (line == null)
                {
                    Console.WriteLine("Unexpected EOF: missing line 2");
                    return -4;
                }

                using (var output = new CuttermaranFileWriter(outputName, videoFilename, audioFilename, totalFrames))
                {
                    int lastAdEnd = -1;
                    while ((line = input.ReadLine()) != null)
                    {
                        parts = line.Split(new char[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length != 2)
                        {
                            Console.WriteLine("Unexpected line format: " + line);
                            return -5;
                        }

                        int adStart;
                        if (!int.TryParse(parts[0], out adStart))
                        {
                            Console.WriteLine("Failed to parse integer: " + parts[0]);
                            Console.WriteLine("line: " + line);
                            return -6;
                        }
                        adStart += frameOffset;

                        int showStart = lastAdEnd + 1;
                        int showEnd = adStart - 1;

                        output.WriteCut(showStart, showEnd);

                        if (!int.TryParse(parts[1], out lastAdEnd))
                        {
                            Console.WriteLine("Failed to parse integer: " + parts[0]);
                            Console.WriteLine("line: " + line);
                            return -7;
                        }
                        if (lastAdEnd < totalFrames)
                            lastAdEnd += frameOffset;
                    }
                    if (lastAdEnd == -1)
                    {
                        Console.WriteLine("Unexpected EOF: missing commercial blocks");
                        return -10;
                    }

                    output.WriteCut(lastAdEnd + 1, totalFrames);
                }
            }
            return 0;
        }

        private static int ProcessCsvFile(string inputName, string videoFilename, string audioFilename, int frameOffset)
        {
            string logFile = Path.GetDirectoryName(inputName) + "\\" + Path.GetFileNameWithoutExtension(inputName) + "_scoring.log";
            using (var output = new StreamWriter(logFile, false))
            {
            }

            var processor = new ComskipCsvProcessor(inputName, videoFilename, audioFilename, frameOffset);
            processor.Log += (message) => {
                Console.WriteLine(message);
                using (var output = new StreamWriter(logFile, true))
                {
                    output.WriteLine(message);
                }
            };

            int ret;
            ret = processor.Process();
            if (ret < 0)
                return ret;

            ret = processor.WriteCuttermaranFile();
            return ret;
        }

        private static int ShowWindow(string inputName, string videoFilename, string audioFilename, int frameOffset)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new MainForm(inputName, videoFilename, audioFilename, frameOffset);
            Application.Run(form);
            return 0;
        }

    }
}
