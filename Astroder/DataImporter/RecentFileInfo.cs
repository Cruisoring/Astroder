using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataImporter
{
    public enum SourceType
    {
        Pobo,
        Text
    }

    [Serializable]
    public class RecentFileInfo : IEquatable<RecentFileInfo>, IComparable<RecentFileInfo>
    {
        public static int Limit = 20; 

        public DateTime LastAccess { get; set; }

        public string Name { get; set; }

        public string FullFileName { get; set; }

        public SourceType Source { get; set; }

        //public RecordType Type { get; set; }

        public RecentFileInfo() {}

        public RecentFileInfo(string name, string fullFileName, SourceType source)
        {
            LastAccess = DateTime.Now;
            Name = name;
            FullFileName = fullFileName;
            Source = source;
        }

        public override string ToString()
        {
            int start = FullFileName.IndexOf(@"\", FullFileName.Length - 23);
            string fileName = FullFileName.Substring(start);
            return String.Format("{0}, {1}", fileName, LastAccess);
        }
        
        #region IEquatable<RecentFile> 成员

        public bool Equals(RecentFileInfo other)
        {
            return this.FullFileName == other.FullFileName && Source == other.Source;
        }

        #endregion

        #region IComparable<RecentFile> 成员

        public int CompareTo(RecentFileInfo other)
        {
            return other.LastAccess.CompareTo(this.LastAccess);
        }

        #endregion
    }
}
