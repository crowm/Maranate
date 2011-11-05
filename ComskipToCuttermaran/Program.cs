using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace ComskipToCuttermaran
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: " + Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + " <comskip.txt> <video.m2v> <audio.ac3> [frameOffset]");
                return -1;
            }

            int frameOffset = 0;
            if (args.Length > 3)
            {
                frameOffset = int.Parse(args[3]);
            }

            string inputName = Path.GetFullPath(args[0]);
            string videoFilename = Path.GetFullPath(args[1]);
            string audioFilename = Path.GetFullPath(args[2]);
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

                using (var output = new StreamWriter(outputName))
                {
                    output.WriteLine(@"<?xml version=""1.0"" standalone=""yes""?>");
                    output.WriteLine(@"<StateData xmlns=""http://cuttermaran.kickme.to/StateData.xsd"">");
                    output.WriteLine("\t" + @"<usedVideoFiles FileID=""0"" FileName=""" + HttpUtility.HtmlEncode(videoFilename) + @""" />");
                    output.WriteLine("\t" + @"<usedAudioFiles FileID=""1"" FileName=""" + HttpUtility.HtmlEncode(audioFilename) + @""" StartDelay=""0"" />");

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

                        WriteCut(output, showStart, showEnd, totalFrames);

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

                    WriteCut(output, lastAdEnd + 1, totalFrames, totalFrames);

                    output.WriteLine("\t" + @"<CmdArgs OutFile=""" + HttpUtility.HtmlEncode(filename) + @"_clean.m2v"" snapToCutPoints=""false"" />");
                    output.WriteLine(@"</StateData>");

                }
            }
            return 0;
        }

        private static void WriteCut(StreamWriter output, int showStart, int showEnd, int totalFrames)
        {
            if (showStart < 0)
                showStart = 0;
            if (showEnd < 0)
                showEnd = 0;
            if (showStart > totalFrames)
                showStart = totalFrames;
            if (showEnd > totalFrames)
                showEnd = totalFrames;

            if (showEnd > showStart)
            {
                output.WriteLine("\t" + @"<CutElements refVideoFile=""0"" StartPosition=""" + showStart.ToString() + @""" EndPosition=""" + showEnd.ToString() + @""">");
                output.WriteLine("\t\t" + @"<CurrentFiles refVideoFiles=""0"" />");
                output.WriteLine("\t\t" + @"<cutAudioFiles refAudioFile=""1"" />");
                output.WriteLine("\t" + @"</CutElements>");
            }

        }

    }
}
