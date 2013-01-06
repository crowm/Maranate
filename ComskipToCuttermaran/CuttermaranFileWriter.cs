using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Maranate
{
    public class CuttermaranFileWriter : IDisposable
    {
        private StreamWriter _output;
        private int _totalFrames;
        private string _videoFilename;

        public CuttermaranFileWriter(string filename, string videoFilename, string audioFilename, int totalFrames)
        {
            _videoFilename = videoFilename;
            _totalFrames = totalFrames;

            _output = new StreamWriter(filename);

            _output.WriteLine(@"<?xml version=""1.0"" standalone=""yes""?>");
            _output.WriteLine(@"<StateData xmlns=""http://cuttermaran.kickme.to/StateData.xsd"">");
            _output.WriteLine("\t" + @"<usedVideoFiles FileID=""0"" FileName=""" + HttpUtility.HtmlEncode(videoFilename) + @""" />");
            _output.WriteLine("\t" + @"<usedAudioFiles FileID=""1"" FileName=""" + HttpUtility.HtmlEncode(audioFilename) + @""" StartDelay=""0"" />");
        }

        public void WriteCut(int showStart, int showEnd)
        {
            if (showStart < 0)
                showStart = 0;
            if (showEnd < 0)
                showEnd = 0;
            if (showStart > _totalFrames)
                showStart = _totalFrames;
            if (showEnd > _totalFrames)
                showEnd = _totalFrames;

            if (showEnd > showStart)
            {
                _output.WriteLine("\t" + @"<CutElements refVideoFile=""0"" StartPosition=""" + showStart.ToString() + @""" EndPosition=""" + showEnd.ToString() + @""">");
                _output.WriteLine("\t\t" + @"<CurrentFiles refVideoFiles=""0"" />");
                _output.WriteLine("\t\t" + @"<cutAudioFiles refAudioFile=""1"" />");
                _output.WriteLine("\t" + @"</CutElements>");
            }

        }

        public void Dispose()
        {
            string filename = Path.GetFileNameWithoutExtension(_videoFilename);
            _output.WriteLine("\t" + @"<CmdArgs OutFile=""" + HttpUtility.HtmlEncode(filename) + @"_clean.m2v"" snapToCutPoints=""false"" />");
            _output.WriteLine(@"</StateData>");
            _output.Close();
        }
    }
}
