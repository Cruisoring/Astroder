using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PolygonControl
{
    public class OverlayScheme
    {
        public const float DefaultOrb = 0.25f;
        public static OverlayScheme Default = new OverlayScheme(0, DefaultOrb, Color.Orange, Color.SandyBrown, Color.Yellow);
        public static Dictionary<int, OverlayScheme> DefaultSetting = new Dictionary<int, OverlayScheme>()
            {
                {3, new OverlayScheme(120, 1f, Color.Green, Color.DarkGreen, Color.LightGreen)},
                {4, new OverlayScheme(90, 1f, Color.Red, Color.Maroon, Color.LightCoral)},
                {5, new OverlayScheme(72, 0.5f, Color.Purple, Color.DarkMagenta, Color.Orchid)},
                {6, new OverlayScheme(60, 0.5f, Color.Blue, Color.Navy, Color.SkyBlue)},
                {8, new OverlayScheme(45, 0.5f, Color.Brown, Color.SaddleBrown, Color.Peru)},
            };

        public static OverlayScheme SchemeOf(int divisions)
        {
            if (DefaultSetting.ContainsKey(divisions))
                return DefaultSetting[divisions];
            else
                return new OverlayScheme(360f / divisions, DefaultOrb, Default.BaseColor, Default.DarkColor, Default.LightColor);
        }

        public float SweepAngle { get; set; }
        public float Orb { get; set; }
        public Color BaseColor { get; set; }
        public Color LightColor { get; set; }
        public Color DarkColor { get; set; }

        private OverlayScheme(float sweep, float orb, Color baseColor, Color darkColor, Color lightColor)
        {
            SweepAngle = sweep;
            Orb = orb;
            BaseColor = baseColor;
            LightColor = lightColor;
            DarkColor = darkColor;
        }
    }

}
