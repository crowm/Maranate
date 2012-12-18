using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;

namespace ComskipToCuttermaran
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                PrintUsage();
                return -1;
            }

            string inputName = Path.GetFullPath(args[0]);
            string videoFilename = Path.GetDirectoryName(inputName) + "\\" + Path.GetFileNameWithoutExtension(inputName) + ".m2v";
            string audioFilename = Path.GetDirectoryName(inputName) + "\\" + Path.GetFileNameWithoutExtension(inputName) + ".mp2";
            if (!File.Exists(audioFilename))
            {
                audioFilename = Path.GetDirectoryName(inputName) + "\\" + Path.GetFileNameWithoutExtension(inputName) + ".ac3";
            }

            int frameOffset = 0;
            if (args.Length >= 4)
            {
                frameOffset = int.Parse(args[3]);
            }
            if (args.Length >= 3)
            {
                videoFilename = Path.GetFullPath(args[1]);
                audioFilename = Path.GetFullPath(args[2]);
            }

            string extension = Path.GetExtension(inputName);

            if (extension.Equals(".txt", StringComparison.CurrentCultureIgnoreCase))
            {
                return ProcessTxtFile(inputName, videoFilename, audioFilename, frameOffset);
            }
            else if (extension.Equals(".csv", StringComparison.CurrentCultureIgnoreCase))
            {
                return ProcessCsvFile(inputName, videoFilename, audioFilename, frameOffset);
            }
            else
            {
                Console.WriteLine("Unexpected file extension '" + extension + "'. Please supply either a .txt or .csv file.");
                return -2;
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: " + Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + " <comskip.txt> [video.m2v] [audio.mp2/audio.ac3] [frameOffset]");
            Console.WriteLine(" or");
            Console.WriteLine("Usage: " + Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + " <comskip.csv> [video.m2v] [audio.mp2/audio.ac3] [frameOffset]");
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
            return processor.Process();
        }

    }
}
