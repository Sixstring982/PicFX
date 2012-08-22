using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace PicFX
{
    enum Channel
    {
        R,
        G,
        B,
        All
    }

    class PicMod
    {
        private Bitmap resetBMP = null;
        public Bitmap loadedBMP = null;
        private Point[] MPath = null;

        private const int tileSize = 10;

        private Point[] GenerateMPath()
        {
            Random rand = new Random(1234);
            List<Point> newPath = new List<Point>();
            for (int x = 0; x < loadedBMP.Width - tileSize; x += tileSize)
            {
                for (int y = 0; y < loadedBMP.Height - tileSize; y += tileSize)
                {
                    newPath.Add(new Point(x / tileSize, y / tileSize));
                }
            }
            int a, b;
            Point tmp;
            for (int i = 0; i < newPath.Count * 5; i++)
            {
                a = rand.Next() % newPath.Count;
                b = rand.Next() % newPath.Count;
                tmp = newPath[a];
                newPath[a] = newPath[b];
                newPath[b] = tmp;
            }
            return newPath.ToArray();
        }

        public void Shift(Direction dir, Channel c, int distance)
        {
            if (loadedBMP == null)
            {
                ConX.ErrorWrite("No picture has been loaded yet.");
                return;
            }
            ConX.InfoWrite("Beginning Shift...");
            Bitmap oldBMP = new Bitmap(loadedBMP);
            BitmapData oldD = FastGraphics.WholeLock(oldBMP);
            BitmapData bmpD = FastGraphics.WholeLock(loadedBMP);

            int startX = dir == Direction.Right ? distance : (dir == Direction.Left ? -distance : 0);
            int endX = startX + loadedBMP.Width;
            int startY = dir == Direction.Down ? distance : (dir == Direction.Up ? -distance : 0);
            int endY = startY + loadedBMP.Height;

            Color newC, oldC;
            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    newC = FastGraphics.GetPixel(bmpD, x, y);
                    oldC = FastGraphics.GetPixel(oldD, x - startX, y - startY);
                    switch(c)
                    {
                        case Channel.B:   newC = Color.FromArgb(newC.A, newC.R, oldC.B, newC.G); break;
                        case Channel.G:   newC = Color.FromArgb(newC.A, newC.R, newC.B, oldC.G); break;
                        case Channel.R:   newC = Color.FromArgb(newC.A, oldC.R, newC.B, newC.G); break;
                        case Channel.All: newC = Color.FromArgb(newC.A, oldC.R, oldC.B, oldC.G); break;
                    }
                    FastGraphics.SetPixel(bmpD, x, y, newC);
                }
            }

            ConX.InfoWrite("Shift Complete.");
            loadedBMP.UnlockBits(bmpD);
        }

        public void CThreshold(byte threshold, Channel c)
        {
            if (loadedBMP == null)
            {
                ConX.ErrorWrite("No picture has been loaded yet.");
                return;
            }
            ConX.InfoWrite("Beginning CThreshold...");
            BitmapData bmpData = loadedBMP.LockBits(new Rectangle(0, 0, loadedBMP.Width, loadedBMP.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            Color readC;
            for (int x = 0; x < bmpData.Width; x++)
            {
                for (int y = 0; y < bmpData.Height; y++)
                {
                    readC = FastGraphics.GetPixel(bmpData, x, y);
                    switch (c)
                    {
                        case Channel.R:
                            if (readC.R < threshold)
                                FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(0, readC.G, readC.B));
                            break;
                        case Channel.G:
                            if (readC.G < threshold)
                                FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(readC.R, 0, readC.B));
                            break;
                        case Channel.B:
                            if (readC.B < threshold)
                                FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(readC.R, readC.G, 0));
                            break;
                    }
                }
            }

            ConX.InfoWrite("CThreshold Complete.");
            loadedBMP.UnlockBits(bmpData);
        }

        public void CLow(byte threshold, Channel c)
        {
            if (loadedBMP == null)
            {
                ConX.ErrorWrite("No picture has been loaded yet.");
                return;
            }
            ConX.InfoWrite("Beginning CLow...");
            BitmapData bmpData = loadedBMP.LockBits(new Rectangle(0, 0, loadedBMP.Width, loadedBMP.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            Color readC;
            for (int x = 0; x < bmpData.Width; x++)
            {
                for (int y = 0; y < bmpData.Height; y++)
                {
                    readC = FastGraphics.GetPixel(bmpData, x, y);
                    switch (c)
                    {
                        case Channel.R:
                            if (readC.R > threshold)
                                FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(0, readC.G, readC.B));
                            break;
                        case Channel.G:
                            if (readC.G > threshold)
                                FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(readC.R, 0, readC.B));
                            break;
                        case Channel.B:
                            if (readC.B > threshold)
                                FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(readC.R, readC.G, 0));
                            break;
                    }
                }
            }

            ConX.InfoWrite("CThreshold CLow.");
            loadedBMP.UnlockBits(bmpData);
        }

        public void CKeep(byte threshold, Channel c)
        {
            if (loadedBMP == null)
            {
                ConX.ErrorWrite("No picture has been loaded yet.");
                return;
            }
            ConX.InfoWrite("Beginning CKeep...");
            BitmapData bmpData = loadedBMP.LockBits(new Rectangle(0, 0, loadedBMP.Width, loadedBMP.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            Color readC;
            byte p1, p2;
            for (int x = 0; x < bmpData.Width; x++)
            {
                for (int y = 0; y < bmpData.Height; y++)
                {
                    readC = FastGraphics.GetPixel(bmpData, x, y);
                    switch (c)
                    {
                        case Channel.R:
                            p1 = readC.G;
                            p2 = readC.B;
                            if (readC.G < threshold)
                                p1 = 0;
                            if (readC.B < threshold)
                                p2 = 0;
                            FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(readC.R, p1, p2));
                            break;
                        case Channel.G:
                            p1 = readC.R;
                            p2 = readC.B;
                            if (readC.G < threshold)
                                p1 = 0;
                            if (readC.B < threshold)
                                p2 = 0;
                            FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(p1, readC.G, p2));
                            break;
                        case Channel.B:
                            p1 = readC.R;
                            p2 = readC.G;
                            if (readC.G < threshold)
                                p1 = 0;
                            if (readC.B < threshold)
                                p2 = 0;
                            FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(p1, p2, readC.B));
                            break;
                    }
                }
            }

            ConX.InfoWrite("CKeep Complete.");
            loadedBMP.UnlockBits(bmpData);
        }

        public void Compress(int factor)
        {
            if (loadedBMP == null)
            {
                ConX.ErrorWrite("No picture has been loaded yet.");
                return;
            }
            ConX.InfoWrite("Beginning Compress...");
            BitmapData bmpData = loadedBMP.LockBits(new Rectangle(0, 0, loadedBMP.Width, loadedBMP.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            Color readC;
            Color newc;
            factor = (int)Math.Pow(2, factor);
            for (int x = 0; x < bmpData.Width; x++)
            {
                for (int y = 0; y < bmpData.Height; y++)
                {
                    readC = FastGraphics.GetPixel(bmpData, x, y);
                    newc = Color.FromArgb(readC.A, (byte)((readC.R / factor) * factor),
                        (byte)((readC.G / factor) * factor), (byte)((readC.B / factor) * factor));
                    FastGraphics.SetPixel(bmpData, x, y, newc);
                }
            }

            ConX.InfoWrite("Compress Complete.");
            loadedBMP.UnlockBits(bmpData);
        }

        public void Multiply(double multiplier, Channel channel)
        {
            if (loadedBMP == null)
            {
                ConX.ErrorWrite("No picture has been loaded yet.");
                return;
            }
            ConX.InfoWrite("Beginning Multiply...");
            BitmapData bmpData = loadedBMP.LockBits(new Rectangle(0, 0, loadedBMP.Width, loadedBMP.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            Color readColor;
            double product;
            for (int x = 0; x < bmpData.Width; x++)
            {
                for (int y = 0; y < bmpData.Height; y++)
                {
                    readColor = FastGraphics.GetPixel(bmpData, x, y);
                    switch (channel)
                    {
                        case Channel.R:
                            product = readColor.R * multiplier;
                            if (product > 255) product = 255;
                            FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(readColor.A, (byte)product, readColor.G, readColor.B));
                            break;
                        case Channel.G:
                            product = readColor.G * multiplier;
                            if (product > 255) product = 255;
                            FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(readColor.A, readColor.R, (byte)product, readColor.B));
                            break;
                        case Channel.B:
                            product = readColor.B * multiplier;
                            if (product > 255) product = 255;
                            FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(readColor.A, readColor.R, readColor.G, (byte)product));
                            break;
                    }
                }
            }

            ConX.InfoWrite("Multiply Complete.");
            loadedBMP.UnlockBits(bmpData);
        }

        public void ReChannel(Channel channel)
        {
            if (loadedBMP == null)
            {
                ConX.ErrorWrite("No picture has been loaded yet.");
                return;
            }
            ConX.InfoWrite("Beginning ReChannel...");
            BitmapData bmpData = loadedBMP.LockBits(new Rectangle(0, 0, loadedBMP.Width, loadedBMP.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            BitmapData rstData = resetBMP.LockBits(new Rectangle(0, 0, loadedBMP.Width, loadedBMP.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            Color readColor;
            Color oldRead;
            for (int x = 0; x < bmpData.Width; x++)
            {
                for (int y = 0; y < bmpData.Height; y++)
                {
                    readColor = FastGraphics.GetPixel(bmpData, x, y);
                    oldRead = FastGraphics.GetPixel(rstData, x, y);
                    switch (channel)
                    {
                        case Channel.R:
                            FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(readColor.A, oldRead.R, readColor.G, readColor.B));
                            break;
                        case Channel.G:
                            FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(readColor.A, readColor.R, oldRead.G, readColor.B));
                            break;
                        case Channel.B:
                            FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(readColor.A, readColor.R, readColor.G, oldRead.B));
                            break;
                    }
                }
            }

            ConX.InfoWrite("ReChannel Complete.");
            resetBMP.UnlockBits(rstData);
            loadedBMP.UnlockBits(bmpData);
        }

        public void Reset()
        {
            ConX.InfoWrite("Beginning reset...");
            loadedBMP = new Bitmap(resetBMP);
            ConX.InfoWrite("Reset Complete.");
        }

        public void GreyChannel(Channel channel)
        {
            if (loadedBMP == null)
            {
                ConX.ErrorWrite("No picture has been loaded yet.");
                return;
            }
            ConX.InfoWrite("Beginning GreyChannel...");
            BitmapData bmpData = loadedBMP.LockBits(new Rectangle(0, 0, loadedBMP.Width, loadedBMP.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            Color readColor;
            for (int x = 0; x < bmpData.Width; x++)
            {
                for (int y = 0; y < bmpData.Height; y++)
                {
                    readColor = FastGraphics.GetPixel(bmpData, x, y);
                    switch (channel)
                    {
                        case Channel.R:
                            FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(readColor.A, readColor.R, readColor.R, readColor.R));
                            break;
                        case Channel.G:
                            FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(readColor.A, readColor.G, readColor.G, readColor.G));
                            break;
                        case Channel.B:
                            FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(readColor.A, readColor.B, readColor.B, readColor.B));
                            break;
                    }
                }
            }

            ConX.InfoWrite("GreyChannel Complete.");
            loadedBMP.UnlockBits(bmpData);
        }

        public void DeChannel(Channel channel)
        {
            if (loadedBMP == null)
            {
                ConX.ErrorWrite("No picture has been loaded yet.");
                return;
            }
            ConX.InfoWrite("Beginning DeChannel...");
            BitmapData bmpData = loadedBMP.LockBits(new Rectangle(0, 0, loadedBMP.Width, loadedBMP.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            Color readColor;
            for (int x = 0; x < bmpData.Width; x++)
            {
                for (int y = 0; y < bmpData.Height; y++)
                {
                    readColor = FastGraphics.GetPixel(bmpData, x, y);
                    switch (channel)
                    {
                        case Channel.R:
                            FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(readColor.A, 0, readColor.G, readColor.B));
                            break;
                        case Channel.G:
                            FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(readColor.A, readColor.R, 0, readColor.B));
                            break;
                        case Channel.B:
                            FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(readColor.A, readColor.R, readColor.G, 0));
                            break;
                    }
                    if (y > bmpData.Height - 10)
                        y = y;
                }
            }

            ConX.InfoWrite("DeChannel Complete.");
            loadedBMP.UnlockBits(bmpData);
        }

        public void Flip(bool horizontal)
        {
            if (loadedBMP == null)
            {
                ConX.ErrorWrite("No picture has been loaded yet.");
                return;
            }
            ConX.InfoWrite("Beginning Flip...");
            BitmapData bmpData = loadedBMP.LockBits(new Rectangle(0, 0, loadedBMP.Width, loadedBMP.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            if (horizontal)
            {
                Color c1, c2;
                for (int y = 0; y < bmpData.Height / 2; y++)
                {
                    for (int x = 0; x < bmpData.Width; x++)
                    {
                        c1 = FastGraphics.GetPixel(bmpData, x, y);
                        c2 = FastGraphics.GetPixel(bmpData, x, bmpData.Height - y - 1);
                        FastGraphics.SetPixel(bmpData, x, y, c2);
                        FastGraphics.SetPixel(bmpData, x, bmpData.Height - y - 1, c1);
                    }
                }
            }
            if (!horizontal)
            {
                Color c1, c2;
                for (int x = 0; x < bmpData.Width / 2; x++)
                {
                    for (int y = 0; y < bmpData.Height; y++)
                    {
                        c1 = FastGraphics.GetPixel(bmpData, x, y);
                        c2 = FastGraphics.GetPixel(bmpData, bmpData.Width - x - 1, y);
                        FastGraphics.SetPixel(bmpData, x, y, c2);
                        FastGraphics.SetPixel(bmpData, bmpData.Width - x - 1, y, c1);
                    }
                }
            }

            ConX.InfoWrite("Flip Complete.");
            loadedBMP.UnlockBits(bmpData);
        }

        public void Negate()
        {
            if (loadedBMP == null)
            {
                ConX.ErrorWrite("No picture has been loaded yet.");
                return;
            }
            ConX.InfoWrite("Beginning Negate...");
            BitmapData bmpData = loadedBMP.LockBits(new Rectangle(0, 0, loadedBMP.Width, loadedBMP.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            Color negativeColor;
            Color c;
            for (int x = 0; x < bmpData.Width; x++)
            {
                for (int y = 0; y < bmpData.Height; y++)
                {
                    c = FastGraphics.GetPixel(bmpData, x, y);
                    negativeColor = Color.FromArgb(c.A, 0xff - c.R, 0xff - c.G, 0xff - c.B);
                    FastGraphics.SetPixel(bmpData, x, y, negativeColor);
                }
            }
            ConX.InfoWrite("Negate Complete.");
            loadedBMP.UnlockBits(bmpData);
        }

        public void DeRes(int scale)
        {
            if (loadedBMP == null)
            {
                ConX.ErrorWrite("No picture has been loaded yet.");
                return;
            }
            ConX.InfoWrite("Beginning DeRes...");
            BitmapData bmpData = loadedBMP.LockBits(new Rectangle(0, 0, loadedBMP.Width, loadedBMP.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            Rectangle fillRect = new Rectangle(0, 0, scale, scale);
            for (int x = 0; x < bmpData.Width; x += scale)
            {
                for (int y = 0; y < bmpData.Height; y += scale)
                {
                    fillRect.Width = fillRect.Height = scale;
                    fillRect.X = x;
                    fillRect.Y = y;
                    if (x + scale >= bmpData.Width) fillRect.Width -= ((x + scale) % bmpData.Width);
                    if (y + scale >= bmpData.Height) fillRect.Height -= ((y + scale) % bmpData.Height);
                    FastGraphics.FillRect(bmpData, fillRect, FastGraphics.GetPixel(bmpData, x, y));
                }
            }
            ConX.InfoWrite("DeRes Complete.");
            loadedBMP.UnlockBits(bmpData);
        }

        public void Mplus()
        {
            if (loadedBMP == null)
            {
                ConX.ErrorWrite("No picture has been loaded yet.");
                return;
            }
            ConX.InfoWrite("Beginning M+...");
            Bitmap newBMP = new Bitmap(loadedBMP);
            BitmapData bmpData = loadedBMP.LockBits(new Rectangle(0, 0, loadedBMP.Width, loadedBMP.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            BitmapData newData = newBMP.LockBits(new Rectangle(0, 0, newBMP.Width, newBMP.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            Rectangle srcRect = new Rectangle(0, 0, tileSize, tileSize);
            Rectangle destRect = new Rectangle(0, 0, tileSize, tileSize);
            int j;
            for (int i = 0; i < MPath.Length; i++)
            {
                srcRect.X = MPath[i].X * tileSize;
                srcRect.Y = MPath[i].Y * tileSize;
                j = i + 1;
                if (i == MPath.Length - 1) j = 0;
                destRect.X = MPath[j].X * tileSize;
                destRect.Y = MPath[j].Y * tileSize;

                for (int x = 0; x < tileSize; x++)
                {
                    for (int y = 0; y < tileSize; y++)
                    {
                        FastGraphics.SetPixel(newData, destRect.X + x, destRect.Y + y,
                            FastGraphics.GetPixel(bmpData, srcRect.X + x, srcRect.Y + y));
                    }
                }
            }

            ConX.InfoWrite("M+ Complete.");
            newBMP.UnlockBits(newData);
            loadedBMP.UnlockBits(bmpData);
            loadedBMP = newBMP;
        }

        public void Mminus()
        {
            if (loadedBMP == null)
            {
                ConX.ErrorWrite("No picture has been loaded yet.");
                return;
            }
            ConX.InfoWrite("Beginning M-...");
            Bitmap newBMP = new Bitmap(loadedBMP);
            BitmapData bmpData = loadedBMP.LockBits(new Rectangle(0, 0, loadedBMP.Width, loadedBMP.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            BitmapData newData = newBMP.LockBits(new Rectangle(0, 0, newBMP.Width, newBMP.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            Rectangle srcRect = new Rectangle(0, 0, tileSize, tileSize);
            Rectangle destRect = new Rectangle(0, 0, tileSize, tileSize);
            int j;
            for (int i = MPath.Length - 1; i >= 0; i--)
            {
                j = i + 1;
                if (i == MPath.Length - 1) j = 0;
                srcRect.X = MPath[j].X * tileSize;
                srcRect.Y = MPath[j].Y * tileSize;
                destRect.X = MPath[i].X * tileSize;
                destRect.Y = MPath[i].Y * tileSize;

                for (int x = 0; x < tileSize; x++)
                {
                    for (int y = 0; y < tileSize; y++)
                    {
                        FastGraphics.SetPixel(newData, destRect.X + x, destRect.Y + y,
                            FastGraphics.GetPixel(bmpData, srcRect.X + x, srcRect.Y + y));
                    }
                }
            }

            ConX.InfoWrite("M- Complete.");
            newBMP.UnlockBits(newData);
            loadedBMP.UnlockBits(bmpData);
            loadedBMP = newBMP;
        }

        public void CRotate()
        {
            if (loadedBMP == null)
            {
                ConX.ErrorWrite("No picture has been loaded yet.");
                return;
            }
            ConX.InfoWrite("Beginning CRotate...");
            BitmapData bmpData = loadedBMP.LockBits(new Rectangle(0, 0, loadedBMP.Width, loadedBMP.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            Color readC;
            for (int x = 0; x < bmpData.Width; x++)
            {
                for (int y = 0; y < bmpData.Height; y++)
                {
                    readC = FastGraphics.GetPixel(bmpData, x, y);
                    FastGraphics.SetPixel(bmpData, x, y, Color.FromArgb(readC.A, readC.G, readC.B, readC.R));
                }
            }

            ConX.InfoWrite("CRotate Complete.");
            loadedBMP.UnlockBits(bmpData);
        }

        public void Pane(int size)
        {
            if (loadedBMP == null)
            {
                ConX.ErrorWrite("No picture has been loaded yet.");
                return;
            }
            ConX.InfoWrite("Beginning Pane...");
            BitmapData bmpData = loadedBMP.LockBits(new Rectangle(0, 0, loadedBMP.Width, loadedBMP.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            Color[] cs = new Color[size];
            for (int x = 0; x < bmpData.Width; x += size)
            {
                for (int y = 0; y < bmpData.Height; y++)
                {
                    if (x + size - 1 >= bmpData.Width) break;
                    for (int i = 0; i < size; i++)
                        cs[i] = FastGraphics.GetPixel(bmpData, x + i, y);
                    for (int i = 0; i < size; i++)
                        FastGraphics.SetPixel(bmpData, x + i, y, cs[size - 1 - i]);
                }
            }

            ConX.InfoWrite("Pane Complete.");
            loadedBMP.UnlockBits(bmpData);
        }

        public void Unload()
        {
            resetBMP = null;
            loadedBMP = null;
            MPath = null;
            ConX.InfoWrite("Image unloaded. Load an image to begin.");
        }

        public void CSmear()
        {
            if (loadedBMP == null)
            {
                ConX.ErrorWrite("No picture has been loaded yet.");
                return;
            }
            Mplus();
            Mminus();
            DeChannel(Channel.R);
            DeChannel(Channel.G);
            DeRes(20);
            ReChannel(Channel.R);
            DeRes(10);
            Pane(8);
            ReChannel(Channel.G);
        }

        public void LoadFromFilename(string filename)
        {
            if (File.Exists(filename))
            {
                loadedBMP = new Bitmap(filename);
                resetBMP = new Bitmap(filename);
                MPath = GenerateMPath();
                ConX.InfoWrite("Bitmap loaded!");
            }
            else
            {
                ConX.ErrorWrite("Filename does not exist.");
            }
        }

        public void SaveToFilename(string filename)
        {
            loadedBMP.Save(filename);
            ConX.InfoWrite("File saved.");
        }
    }
}
