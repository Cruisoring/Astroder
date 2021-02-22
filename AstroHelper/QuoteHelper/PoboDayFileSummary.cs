using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace QuoteHelper
{
    public class PoboDayFileSummary
    {
        public String Name { get; private set;}

        public int ItemsCount { get; private set; }

        public DateTime Since { get; private set; }

        public DateTime Until { get; private set; }

        public int TotalDays { get; private set; }

        public bool IsAlive { get; private set; }

        public PoboDayFileSummary(string name, string fileName)
        {
            Name = name;

            FileInfo info = new FileInfo(fileName);

            int fileSize = (int)info.Length;

            if (fileSize % PoboDayStructure.Size != 0)
            {
                Console.WriteLine("Wrong file size of " + fileSize.ToString());
                return;
            }

            byte[] buffer1 = new byte[PoboDayStructure.Size];
            byte[] buffer2 = new byte[PoboDayStructure.Size];

            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            buffer1 = br.ReadBytes(PoboDayStructure.Size);

            fs.Seek(-PoboDayStructure.Size, SeekOrigin.End);
            buffer2 = br.ReadBytes(PoboDayStructure.Size);

            br.Close();
            fs.Close();

            PoboDayStructure first = PoboImporter.BytesToStructures<PoboDayStructure>(buffer1)[0];
            PoboDayStructure last = PoboImporter.BytesToStructures<PoboDayStructure>(buffer2)[0];

            Since = first.Time.Date;
            Until = last.Time.Date;
            ItemsCount = fileSize / PoboDayStructure.Size;
            TotalDays = (int)(Until - Since).TotalDays + 1;
            IsAlive = true;
        }
    }
}
