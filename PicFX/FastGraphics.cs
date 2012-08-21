using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace PicFX
{
    class FastGraphics
    {
        public static void SetPixel(BitmapData g, int x, int y, Color c)
        {
            if (g.PixelFormat != PixelFormat.Format32bppArgb)
                throw new FormatException("Image not in the correct pixel format!");
            if (x < 0 || y < 0 || x > g.Width - 1 || y > g.Width - 1) return;
            if (c.A > 10)
            {
                unsafe
                {
                    byte* start = (byte*)g.Scan0;
                    *(start + 4 * ((y * g.Width) + x)) = c.B;
                    *(start + 4 * ((y * g.Width) + x) + 1) = c.G;
                    *(start + 4 * ((y * g.Width) + x) + 2) = c.R;
                    *(start + 4 * ((y * g.Width) + x) + 3) = c.A;
                }
            }
        }

        public static void ColorReplace(BitmapData g, Color mask, Color replacement)
        {
            for (int x = 0; x < g.Width; x++)
            {
                for (int y = 0; y < g.Height; y++)
                {
                    if (GetPixel(g, x, y).Equals(mask))
                        SetPixel(g, x, y, replacement);
                }
            }
        }

        public static Color GetPixel(BitmapData g, int x, int y)
        {
            int argb;
            if (x > g.Width || y > g.Height || x < 0 || y < 0) return Color.Black;
            unsafe
            {
                byte* start = (byte*)g.Scan0;
                argb = *(start + 4 * ((y * g.Width) + x));
                argb |= (*(start + 4 * ((y * g.Width) + x) + 1) << 8);
                argb |= (*(start + 4 * ((y * g.Width) + x) + 2) << 16);
                argb |= (*(start + 4 * ((y * g.Width) + x) + 3) << 24);
            }
            return Color.FromArgb(argb);
        }

        public static void FillRect(BitmapData g, Rectangle rect, Color c)
        {
            int xend = rect.X + rect.Width, yend = rect.Y + rect.Height;
            for (int x = rect.X; x < xend; x++)
            {
                for (int y = rect.Y; y < yend; y++)
                {
                    SetPixel(g, x, y, c);
                }
            }
        }

        public static void DrawImage(BitmapData dest, BitmapData src, int x, int y)
        {
            int xend = Math.Min(x + src.Width, dest.Width);
            int yend = Math.Min(y + src.Height, dest.Height);
            for (int i = x; i < xend; i++)
            {
                for (int j = y; j < yend; j++)
                {
                    SetPixel(dest, i, j,
                        GetPixel(src, i - x, j - y));
                }
            }
        }

        public static void DrawButton(BitmapData dest, Rectangle rect, bool pressed)
        {
            if (pressed)
            {
                FillRect(dest, rect, Color.FromArgb(0x50, 0x50, 0x50));
                FillRect(dest, new Rectangle(rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height - 4), Color.FromArgb(0x20, 0x20, 0x20));
            }
            else
            {
                FillRect(dest, rect, Color.FromArgb(0x20, 0x20, 0x20));
                FillRect(dest, new Rectangle(rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height - 4), Color.FromArgb(0x50, 0x50, 0x50));
            }
        }

        public static void ShadePixel(BitmapData dest, int x, int y)
        {
            if (dest.PixelFormat != PixelFormat.Format32bppArgb)
                throw new FormatException("Image not in the correct pixel format!");
            if (x < 0 || y < 0 || x > dest.Width - 1 || y > dest.Width - 1) return;
            unsafe
            {
                byte* start = (byte*)dest.Scan0;
                *(start + 4 * ((y * dest.Width) + x)) /= 2;
                *(start + 4 * ((y * dest.Width) + x) + 1) /= 2;
                *(start + 4 * ((y * dest.Width) + x) + 2) /= 2;
            }
        }

        public static void DrawHLine(BitmapData g, int startCol, int endCol, int row, Color c)
        {
            for (int x = startCol; x < endCol; x++)
                SetPixel(g, x, row, c);
        }

        public static void DrawVLine(BitmapData g, int startRow, int endRow, int col, Color c)
        {
            for (int x = startRow; x < endRow; x++)
                SetPixel(g, col, x, c);
        }

        public static void ShadeRect(BitmapData dest, Rectangle rect)
        {
            int xend = Math.Min(rect.X + rect.Width, dest.Width);
            int yend = Math.Min(rect.Y + rect.Height, dest.Height);
            for (int x = rect.X; x < xend; x++)
            {
                for (int y = rect.Y; y < yend; y++)
                {
                    ShadePixel(dest, x, y);
                }
            }
        }

        public static BitmapData WholeLock(Bitmap bmp)
        {
            return bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);
        }
    }
}
