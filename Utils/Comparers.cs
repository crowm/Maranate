using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;

namespace Utils
{
    public class DirectoryInfoComparer : IComparer<DirectoryInfo>
    {
        public int Compare(DirectoryInfo x, DirectoryInfo y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }

    public class FileInfoComparer : IComparer<FileInfo>
    {
        public int Compare(FileInfo x, FileInfo y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
}
