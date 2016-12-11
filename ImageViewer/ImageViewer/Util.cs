using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer
{
    class Util
    {
        public static Rectangle CalcImageRegion(Bitmap img, int width, int height)
        {
            int cmp = width * img.Height - height * img.Width;
            if (cmp < 0)
            {
                int y = (height - (int)(img.Height * (float)width / img.Width)) / 2;
                return new Rectangle(0, y, width, height - 2 * y);
            }
            else
            {
                int x = (width - (int)(img.Width * (float)height / img.Height)) / 2;
                return new Rectangle(x, 0, width - 2 * x, height);
            }
        }

        public static Rectangle CalcImageRegion(Bitmap img, Size size)
        {
            return CalcImageRegion(img, size.Width, size.Height);
        }
        public static void CalcShowRegion(Bitmap img, Size windowSize, Point windowPos, Point mousePoint, float zoom, ref Rectangle formRect, out Rectangle imageRect)
        {
            int imageH = (int)(img.Height * zoom);
            int imageW = (int)(img.Width * zoom);
            mousePoint.X -= windowPos.X;
            mousePoint.Y -= windowPos.Y;
            if (imageH <= windowSize.Height && imageW <= windowSize.Width)
            {
                int formY = (windowSize.Height - imageH) / 2;
                int formX = (windowSize.Width - imageW) / 2;
                formRect = new Rectangle(formX, formY, imageW, imageH);
                imageRect = new Rectangle(0, 0, img.Width, img.Height);
            }
            else
            {
                //Height can be fit screen
                if (imageH <= windowSize.Height)
                {
                    int formY = (windowSize.Height - imageH) / 2;
                    int imageX = (int)(((double)img.Width / windowSize.Width) * mousePoint.X);
                    int deltX = Math.Min((int)((zoom * imageX - mousePoint.X) / zoom), img.Width - windowSize.Width);
                    deltX = deltX < 0 ? 0 : deltX;
                    formRect = new Rectangle(0, formY, windowSize.Width, imageH);
                    imageRect = new Rectangle(deltX, 0, (int)(windowSize.Width / zoom), (int)(imageH / zoom));
                    formRect.Offset(windowPos);
                }
                else
                {
                    if (imageW <= windowSize.Width)
                    {
                        int formX = (windowSize.Width - imageW) / 2;
                        int imageY = (int)(((double)img.Height / windowSize.Height) * mousePoint.Y);
                        int deltY = Math.Min((int)((zoom * imageY - mousePoint.Y) / zoom), img.Height - windowSize.Height);
                        deltY = deltY < 0 ? 0 : deltY;
                        formRect = new Rectangle(formX, 0, imageW, windowSize.Height);
                        imageRect = new Rectangle(0, deltY, (int)(imageW / zoom), (int)(windowSize.Height / zoom));
                        formRect.Offset(windowPos);
                    }
                    else
                    {
                        int mouseX = (mousePoint.X < formRect.X ? formRect.X :
                            ((mousePoint.X > formRect.X + formRect.Width) ? formRect.X + formRect.Width : mousePoint.X));
                        int mouseY = (mousePoint.Y < formRect.Y ? formRect.Y :
                            ((mousePoint.Y > formRect.Y + formRect.Height) ? formRect.Y + formRect.Height : mousePoint.Y));
                        double ratio = Math.Min((double)windowSize.Height / img.Height, (double)windowSize.Width / img.Width);
                        int imageX = (int)((mouseX - formRect.X) / ratio);
                        int imageY = (int)((mouseY - formRect.Y) / ratio);
                        int deltX = (int)(imageX - (mouseX) / zoom);
                        deltX = deltX < 0 ? 0 : deltX;
                        int deltY = (int)(imageY - (mouseY) / zoom);
                        deltY = deltY < 0 ? 0 : deltY;
                        formRect = new Rectangle(0, 0, windowSize.Width, windowSize.Height);
                        imageRect = new Rectangle((int)(deltX + windowPos.X / ratio), (int)(deltY + windowPos.Y / ratio), (int)(windowSize.Width / zoom), (int)(windowSize.Height / zoom));
                        if (imageRect.X + imageRect.Width > img.Width) imageRect.X = img.Width - imageRect.Width;
                        if (imageRect.Y + imageRect.Height > img.Height) imageRect.Y = img.Height - imageRect.Height;
                        formRect.Offset(windowPos);
                    }
                }
            }
        }
        public static void CalcShowRegion(Bitmap img, Size windowSize, Point mousePoint, float zoom, ref Rectangle formRect, out Rectangle imageRect)
        {
            CalcShowRegion(img, windowSize, new Point(0,0) , mousePoint, zoom, ref formRect, out imageRect);
        }
    }
}
