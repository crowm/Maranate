using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;
using System.Windows.Forms;

namespace Maranate
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

            string extension = Path.GetExtension(inputName);

            if (extension.Equals(".txt", StringComparison.CurrentCultureIgnoreCase))
            {
                return ProcessTxtFile(inputName, videoFilename, audioFilename, frameOffset);
            }
            else if (extension.Equals(".csv", StringComparison.CurrentCultureIgnoreCase) ||
                     extension.Equals(".mpg", StringComparison.CurrentCultureIgnoreCase) ||
                     extension.Equals(".ts", StringComparison.CurrentCultureIgnoreCase) ||
                     extension.Equals(".m2v", StringComparison.CurrentCultureIgnoreCase) ||
                     extension.Equals(".mp2", StringComparison.CurrentCultureIgnoreCase) ||
                     extension.Equals(".ac3", StringComparison.CurrentCultureIgnoreCase))
            {
                if (showWindow)
                    return ShowWindow(inputName, videoFilename, audioFilename, frameOffset);
                else
                    return ProcessStatisticsFromFile(inputName, videoFilename, audioFilename, frameOffset);
            }
            else
            {
                Console.WriteLine("Unexpected file extension '" + extension + "'. Please supply either a .txt or .csv file.");
                return -2;
            }
        }

        #region "Console"
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
        #endregion


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
            var filenames = FileDetect.FillFilenames(inputName, videoFilename, audioFilename);
            videoFilename = filenames.videoFilename;
            audioFilename = filenames.audioFilename;

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

        private static int ProcessStatisticsFromFile(string inputName, string videoFilename, string audioFilename, int frameOffset)
        {
            string logFile = Path.GetDirectoryName(inputName) + "\\" + Path.GetFileNameWithoutExtension(inputName) + "_scoring.log";
            using (var output = new StreamWriter(logFile, false))
            {
            }

            var filenames = FileDetect.FillFilenames(inputName, videoFilename, audioFilename);

            Statistics.IStatisticsProcessor processor;
            processor = new Statistics.StatisticsProcessor(filenames, frameOffset);
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
            if (true)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                var form = new MainForm(inputName, videoFilename, audioFilename, frameOffset);
                Application.Run(form);
            }
            else
            {
                var _filenames = FileDetect.FillFilenames(inputName, videoFilename, audioFilename);

                var _videoFile = new MediaFile();
                _videoFile.Resolution = MediaFile.ResolutionOption.Full;
                _videoFile.OutputRGBImage = false;
                _videoFile.OutputYData = true;
                _videoFile.OutputYImage = false;
                _videoFile.Open(_filenames.videoFilename);

                int lastPrintSeconds = 0;

                int frameCount = 0;
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                for (int fieldNumber = 0; fieldNumber < _videoFile.TotalFields; fieldNumber++)
                //for (int fieldNumber = 0; fieldNumber < 1000; fieldNumber++)
                {
                    var frameField = _videoFile.GetVideoFrameField(fieldNumber, MediaFile.SeekModes.Accurate);
                    //frameField.YData.GetBitmap().Save(@"D:\temp\image-" + frameField.FieldNumber.ToString("00000") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    frameField.Dispose();

                    long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                    int elapsedSeconds = (int)(elapsedMilliseconds / 1000);
                    if (elapsedSeconds != lastPrintSeconds)
                    {
                        lastPrintSeconds = elapsedSeconds;
                        var fps = frameCount * 1000.0f / (float)stopwatch.ElapsedMilliseconds;
                        Console.WriteLine("Frame: " + frameCount.ToString() + "  FPS: " + fps.ToString("0.00"));
                    }

                    frameCount++;
                }

                //for (int fieldNumber = 500; fieldNumber < 504; fieldNumber++)
                //{
                //    var frameField = _videoFile.GetVideoFrameField(fieldNumber, MediaFile.SeekModes.Accurate);
                //    //frameField.Image.Save(@"D:\temp\image-" + frameField.FieldNumber.ToString("00000") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    //frameField.YData.GetBitmap().Save(@"D:\temp\image-" + frameField.FieldNumber.ToString("00000") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //    frameField.YData.GetFloatData().GetBitmap().Save(@"D:\temp\image-" + frameField.FieldNumber.ToString("00000") + ".png", System.Drawing.Imaging.ImageFormat.Png);

                //    var yData = frameField.YData;
                //    var floatData = frameField.YData.GetFloatData();

                //    stopwatch.Restart();
                //    for (int test = 0; test < 1000; test++)
                //    {
                //        //var image = floatData.GetBitmap();
                //        var image = yData.GetBitmap();

                //        image = null;
                //        //GC.Collect();
                //    }
                //    Console.WriteLine("Elapsed: " + stopwatch.Elapsed.ToString());

                //    frameField.Dispose();
                //}
            }

            return 0;
        }

    }
}
