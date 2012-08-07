using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PicFX
{
    class ViewLoader : Form
    {
        private PicMod modder;
        private Bitmap field;
        private bool running = false;
        public ViewLoader(PicMod modder)
        {
            this.modder = modder;
            this.field = modder.loadedBMP;
            this.ClientSize = field.Size;
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.Paint += new PaintEventHandler(ViewLoader_Paint);
            this.FormClosing += new FormClosingEventHandler(ViewLoader_FormClosing);
            this.SetStyle(ControlStyles.Opaque, true);
            this.Show();
            Run(10.0f);
        }

        void ViewLoader_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
        }

        private void Stop()
        {
            running = false;
        }

        private void Run(float fps)
        {
            int sleepMS = 0;
            DateTime lastFrame = DateTime.Now;
            running = true;
            while (running)
            {
                sleepMS = (int)((1000.0 / fps) - (DateTime.Now - lastFrame).TotalMilliseconds);
                if (sleepMS > 0) System.Threading.Thread.Sleep(sleepMS);
                this.Invalidate();
                Application.DoEvents();
            }
        }

        void ViewLoader_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(field, Point.Empty);
        }
    }
}
