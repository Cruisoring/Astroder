using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WheelsControl
{
    [ToolboxBitmap(typeof(System.Windows.Forms.Timer))]
    public partial class Wheels : UserControl
    {
        #region Helper functions, classes and delegate definitions

        public const char OneBefore = '\u2460';
        public const char CeilingSymbol = '\u2227';
        public const char FloorSymbol = '\u2228';
        public const char FocusedSymbol = '\u272f';

        // Painting radials

        private const float radiansPerDegree = (float)(Math.PI / 180.0);
        private const float degreesPerRadian = (float)(180 / Math.PI);

        // Helper method - calculate cosine of physicalAngle in degrees
        public static float COS(float angle)
        {
            float rad = radiansPerDegree * angle;
            float cos = (float)Math.Cos(rad);
            return cos;
        }

        // Helper method - calculate sine of physicalAngle in degrees
        public static float SIN(float angle)
        {
            float rad = radiansPerDegree * angle;
            float sin = (float)Math.Sin(rad);
            return sin;
        }

        public static float Normalized(float a)
        {
            a %= 360.0f;
            if (a < 0.0f)
            {
                a += 360.0f;
            }
            return a;
        }

        public delegate void AdapterChangedDelegate();

        public delegate void CalculatorChangedDelegate();

        public delegate void OrientationChangedDelegate();

        public delegate void HighlightChangedDelegate();

        #endregion


        public Wheels()
        {
            InitializeComponent();
        }
    }
}
