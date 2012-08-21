using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PicFX
{
    class CameraForm : Form
    {
        private Bitmap field;
        private Bitmap modBMP;
        private Rectangle selectedRect;
        public CameraForm(int width, int height, PicMod modder)
        {
            modBMP = modder.loadedBMP;
            selectedRect = new Rectangle(0, 0, width, height);
            SetupField(width, height);
            this.ClientSize = field.Size;
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.SetStyle(ControlStyles.Opaque, true);

            this.Paint += new PaintEventHandler(CameraForm_Paint);
        }

        void CameraForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(field, Point.Empty);
        }

        private void SetupField(int width, int height)
        {
            field = new Bitmap(width, height);
            RedrawImage();
        }

        public void MainInvalidate(int top, int left)
        {
            selectedRect.X = left;
            selectedRect.Y = top;
            RedrawImage();
        }

        private void RedrawImage()
        {
            BitmapData fDat = FastGraphics.WholeLock(field);
            BitmapData mDat = FastGraphics.WholeLock(modBMP);

            for (int x = 0; x < field.Width; x++)
            {
                for(int y = 0; y < field.Height; y++)
                {
                    FastGraphics.SetPixel(fDat, x, y,
                        FastGraphics.GetPixel(mDat, selectedRect.X + x, selectedRect.Y + y));
                }
            }

            field.UnlockBits(fDat);
            modBMP.UnlockBits(mDat);

            this.Invalidate();
        }
    }
}
