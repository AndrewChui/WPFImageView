using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageViewer
{
    public partial class SingleForm : Form
    {
        private Bitmap currentImage;
        private Rectangle formRect;
        private Rectangle imageRect;
        private bool mouseButtonDown;
        private Point lastMousePos;
        private Images images;
        public SingleForm(Images imgs)
        {
            InitializeComponent();
            images = imgs;
            images.InitSingleImage();
            currentImage = images.Image;
        }

        private void SingleForm_Load(object sender, EventArgs e)
        {
            //Size = Screen.PrimaryScreen.Bounds.Size;
            MouseWheel += SingleForm_MouseWheel;
            loadFullImage();
        }
        private void SingleForm_MouseWheel(object sender, MouseEventArgs e)
        {
            if(e.Delta<0)
            {
                images.NextSingleImage();
            }
            else
            {
                images.PreSingleImage();
            }
            currentImage = images.Image;
            loadFullImage();
        }
        private void SingleForm_Paint(object sender, PaintEventArgs e)
        {
            if (currentImage != null)
                e.Graphics.DrawImage(currentImage, formRect, imageRect, System.Drawing.GraphicsUnit.Pixel);
        }
        private void loadFullImage()
        {
            formRect = Util.CalcImageRegion(currentImage, Size);
            //formRect.Offset(getWindowPos());
            imageRect = new Rectangle(Point.Empty, currentImage.Size);
            Invalidate();
        }

        private void SingleForm_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Left && currentImage != null)
            {
                mouseButtonDown = true;
                lastMousePos = e.Location;
                Util.CalcShowRegion(currentImage, Size, e.Location, 1, ref formRect, out imageRect);
                Invalidate();
                return;
            }
            if (e.Button == MouseButtons.Right && currentImage != null)
            {
                mouseButtonDown = true;
                lastMousePos = e.Location;
                Util.CalcShowRegion(currentImage, Size, e.Location, 2, ref formRect, out imageRect);
                Invalidate();
                return;
            }
        }

        private void SingleForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void SingleForm_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left || e.Button==MouseButtons.Right) && currentImage != null)
            {
                loadFullImage();
                mouseButtonDown = false;
            }
        }

        private void SingleForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseButtonDown && currentImage != null)
            {
                //if (!mouseMoveCount) return;
                //mouseMoveCount = false;
                var deltX = (e.Location.X - lastMousePos.X) * 7;
                var deltY = (e.Location.Y - lastMousePos.Y) * 7;
                int imageX;
                int imageY;
                Size size = currentImage.Size;
                if (deltX < 0)
                    imageX = (imageRect.X + deltX < 0 ? 0 : imageRect.X + deltX);
                else
                    imageX = ((imageRect.X + deltX + imageRect.Width) > size.Width ? size.Width - imageRect.Width : imageRect.X + deltX);
                if (deltY < 0)
                    imageY = (imageRect.Y + deltY < 0 ? 0 : imageRect.Y + deltY);
                else
                    imageY = ((imageRect.Y + deltY + imageRect.Height) > size.Height ? size.Height - imageRect.Height : imageRect.Y + deltY);
                imageRect = new Rectangle(imageX, imageY, imageRect.Width, imageRect.Height);
                Invalidate();
                lastMousePos = e.Location;
            }
        }
       
    }
}
