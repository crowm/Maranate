
using System.IO;
using System.Text;
namespace Utils
{
    static public class UnifiedDiff
    {
        public static string GenerateUnifiedDiff(string oldString, string newString, string oldLabel, string newLabel)
        {
            Stream streamOut = new MemoryStream();

            Diff.UnifiedDiff.Create(new Diff.UnifiedDiffInfo(stringToStream(oldString), oldLabel),
                new Diff.UnifiedDiffInfo(stringToStream(newString), newLabel), streamOut, 3);

            streamOut.Position = 0;

            return streamToString(streamOut);
        }

        private static MemoryStream stringToStream(string input)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            return new MemoryStream(byteArray);
        }

        private static string streamToString(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}