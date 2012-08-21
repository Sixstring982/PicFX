using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PicFX
{
    class CamMapForm : Form
    {
        private const int MAPSCALE = 4;

        private Bitmap field;
        private CameraForm cForm = null;
        private Rectangle selectedRect = Rectangle.Empty;
        public CamMapForm(PicMod modder)
        {
            SetupField(new Size(modder.loadedBMP.Width / MAPSCALE,
                modder.loadedBMP.Height / MAPSCALE));

            this.ClientSize = field.Size;
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.SizeGripStyle = SizeGripStyle.Hide;
            this.SetStyle(ControlStyles.Opaque, true);
            this.Paint += new PaintEventHandler(CamMapForm_Paint);
            this.MouseDown += new MouseEventHandler(CamMapForm_MouseDown);

            selectedRect = new Rectangle(0, 0, Math.Min(512, modder.loadedBMP.Width),
                Math.Min(512, modder.loadedBMP.Height));

            cForm = new CameraForm(selectedRect.Width, selectedRect.Height, modder);

            selectedRect.Width /= MAPSCALE;
            selectedRect.Height /= MAPSCALE;
            cForm.Show();
            this.Show();
            Run(30.0);
        }

        void CamMapForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X < selectedRect.Width / 2) selectedRect.X = 0;
            else if (e.X > field.Width - (selectedRect.Width / 2))
                selectedRect.X = field.Width - selectedRect.Width;
            else selectedRect.X = e.X - (selectedRect.Width / 2);

            if (e.Y < selectedRect.Height / 2) selectedRect.Y = 0;
            else if (e.Y > field.Height - (selectedRect.Height / 2))
                selectedRect.Y = field.Height - selectedRect.Height;
            else selectedRect.Y = e.Y - (selectedRect.Height / 2);
        }

        private void Run(double fps)
        {
            int sleepms = 0;
            DateTime lastFrame = DateTime.Now;
            while (!this.IsDisposed && !cForm.IsDisposed)
            {
                sleepms = (int)((1000.0 / fps) - (DateTime.Now - lastFrame).TotalMilliseconds);
                if (sleepms > 0) System.Threading.Thread.Sleep(sleepms);
                RedrawRect();
                MainInvdate();
                Application.DoEvents();
            }
            if (!cForm.IsDisposed) cForm.Close();
            this.Close();
        }

        private void RedrawRect()
        {
            BitmapData fDat = field.LockBits(new Rectangle(Point.Empty, field.Size),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            for (int x = 0; x < fDat.Width; x++)
            {
                for (int y = 0; y < fDat.Height; y++)
                {
                    FastGraphics.SetPixel(fDat, x, y, selectedRect.Contains(new Point(x, y)) ?
                        Color.White : Color.Black);
                }
            }

            field.UnlockBits(fDat);
        }

        private void MainInvdate()
        {
            this.Invalidate();
            if(cForm != null)
                cForm.MainInvalidate(selectedRect.Y * MAPSCALE, selectedRect.X * MAPSCALE);
        }

        void CamMapForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(field, Point.Empty);
        }

        private void SetupField(Size size)
        {
            field = new Bitmap(size.Width, size.Height);
            Graphics.FromImage(field).FillRectangle(new SolidBrush(Color.CornflowerBlue), new Rectangle(Point.Empty, size));
        }
    }
}
