using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumberHelper
{
    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class ShapeAttribute : Attribute
    {
        /// <summary>
        /// Number of the sides
        /// </summary>
        public int SideNumber {get; set;}

        /// <summary>
        /// WithCycleZero identify if the first index is on cycle number 0 or not.
        /// </summary>
        public bool WithCycleZero   {get; set;}

        public int FirstIndex {get; set;}

        public ShapeAttribute(int sideNum, bool withZero, int firstIndex)
        {
            SideNumber = sideNum;
            WithCycleZero = withZero;
            FirstIndex = firstIndex;
        }

        public ShapeAttribute(int sideNum)
        {
            SideNumber = sideNum;
            WithCycleZero = true;
            FirstIndex = 1;
        }
    }
}
