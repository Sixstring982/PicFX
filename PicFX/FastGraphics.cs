using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PicFX
{
    class FastGraphics
    {
        public static void SetPixel(BitmapData data, int x, int y, Color c)
        {
            if (data.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new FormatException("Pixel Format must be Format32bggArgb");
            }
            unsafe
            {
                byte* start = (byte*)data.Scan0;
                *(start + (4 * ((y * data.Width) + x))) = c.B;
                *(start + (4 * ((y * data.Width) + x)) + 1) = c.G;
                *(start + (4 * ((y * data.Width) + x)) + 2) = c.R;
                *(start + (4 * ((y * data.Width) + x)) + 3) = c.A;
            }
        }

        public static Color GetPixel(BitmapData data, int x, int y)
        {
            if (data.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new FormatException("Pixel Format must be Format32bggArgb");
            }
            byte a, r, g, b;
            unsafe
            {
                byte* start = (byte*)data.Scan0;
                b = *(start + (4 * ((y * data.Width) + x)));
                g = *(start + (4 * ((y * data.Width) + x)) + 1);
                r = *(start + (4 * ((y * data.Width) + x)) + 2);
                a = *(start + (4 * ((y * data.Width) + x)) + 3);
            }
            return Color.FromArgb(a, r, g, b);
        }

        public static void FillRect(BitmapData data, Rectangle rect, Color c)
        {
            for (int x = rect.X; x < (rect.X + rect.Width); x++)
            {
                for (int y = rect.Y; y < (rect.Y + rect.Height); y++)
                {
                    SetPixel(data, x, y, c);
                }
            }
        }
    }
}
