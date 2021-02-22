using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AstroCalc.Data
{
    public class Zodiac
    {
        public const string Copyright = "Copyright 2010 by William JIANG";
        public const string SoftwareName = "AstroCalc";
        public const string Email = "williamjiang0218@gmail.com";
        public const int InternalVersion = 20100727;

        public readonly static Dictionary<int, Sign> Signs = null;

        static Zodiac()
        {
            Signs = new Dictionary<int,Sign>();
            Type t = typeof(Sign);

            FieldInfo[] fields = t.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);

            foreach (FieldInfo f in fields)
            {
                object obj = f.GetValue(null);
                if (obj is Sign)
                {
                    Sign sign = obj as Sign;
                    Signs.Add(sign.Index, sign);
                }
            }
        }
    }
}
