using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComskipToCuttermaran
{
    public class FileDetect
    {
        public struct Filenames
        {
            public string inputFilename;
            public string csvFilename;
            public string videoFilename;
            public string audioFilename;
            public string logoFilename;
        }

        private static void DetectFilename(string basePath, ref string filename, params string[] extensions)
        {
            if (string.IsNullOrEmpty(filename))
            {
                foreach (var ext in extensions)
                {
                    if (System.IO.File.Exists(basePath + ext))
                    {
                        filename = basePath + ext;
                        return;
                    }
                }
            }
        }

        private static void DetectVideoFilename(string basePath, ref string filename)
        {
            DetectFilename(basePath, ref filename, ".m2v", ".mpg", ".ts");
        }

        private static void DetectAudioFilename(string basePath, ref string filename)
        {
            DetectFilename(basePath, ref filename, ".mp2", ".ac3", ".mpg", ".ts");
        }

        private static void DetectLogoFilename(string basePath, ref string filename)
        {
            filename = basePath + "-logo.png";
        }

        public static Filenames FillFilenames(string filename, string videoFilename = null, string audioFilename = null)
        {
            Filenames result;
            result.inputFilename = String.Empty;
            result.csvFilename = String.Empty;
            result.videoFilename = videoFilename;
            result.audioFilename = audioFilename;
            result.logoFilename = String.Empty;

            if (!string.IsNullOrEmpty(filename) && System.IO.File.Exists(filename))
            {
                result.inputFilename = filename;

                var basePath = System.IO.Path.GetDirectoryName(filename) + "\\" + System.IO.Path.GetFileNameWithoutExtension(filename);
                var ext = System.IO.Path.GetExtension(filename);

                if (ext.Equals(".csv", StringComparison.CurrentCultureIgnoreCase))
                {
                    result.csvFilename = filename;

                    DetectVideoFilename(basePath, ref result.videoFilename);
                    DetectAudioFilename(basePath, ref result.audioFilename);
                    DetectLogoFilename(basePath, ref result.logoFilename);
                }
                else if (ext.Equals(".m2v", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (string.IsNullOrEmpty(result.videoFilename))
                        result.videoFilename = filename;

                    DetectAudioFilename(basePath, ref result.audioFilename);
                    DetectLogoFilename(basePath, ref result.logoFilename);
                }
                else if (ext.Equals(".mp2", StringComparison.CurrentCultureIgnoreCase) ||
                         ext.Equals(".ac3", StringComparison.CurrentCultureIgnoreCase))
                {
                    DetectVideoFilename(basePath, ref result.videoFilename);

                    if (string.IsNullOrEmpty(result.audioFilename))
                        result.audioFilename = filename;

                    DetectLogoFilename(basePath, ref result.logoFilename);
                }
                else
                {
                    string[] mediaFileExtensions = { ".mpg", ".ts", ".avi", ".mp4" };
                    foreach (var mediaFileExtension in mediaFileExtensions)
                    {
                        if (ext.Equals(mediaFileExtension, StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (string.IsNullOrEmpty(result.videoFilename))
                                result.videoFilename = filename;
                            if (string.IsNullOrEmpty(result.audioFilename))
                                result.audioFilename = filename;
                            DetectLogoFilename(basePath, ref result.logoFilename);
                            return result;
                        }
                    }
                    
                    result.inputFilename = String.Empty;
                    result.videoFilename = String.Empty;
                    result.audioFilename = String.Empty;
                }
            }

            return result;
        }

    }
}
