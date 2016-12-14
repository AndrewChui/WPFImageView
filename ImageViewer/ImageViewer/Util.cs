using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
        public static string GetProperty(Bitmap img)
        {
            PropertyItem[] pt = img.PropertyItems;
            int orientation = 1;
            string property = "" + img.Width + " × " + img.Height + " ";
            for (int i = 0; i < pt.Length; i++)
            {
                PropertyItem p = pt[i];
                switch (pt[i].Id)
                {
                    case 0x010F:  // 设备制造商
                        property += "厂商 " + System.Text.ASCIIEncoding.ASCII.GetString(pt[i].Value, 0, pt[i].Value.Length - 1).Trim() + " ";
                        break;
                    case 0x0110: // 设备型号  
                        property += "型号 " + System.Text.ASCIIEncoding.ASCII.GetString(pt[i].Value, 0, pt[i].Value.Length - 1).Trim() + " ";
                        break;
                    case 0x9003: // 拍照时间
                        property += "时间 " + System.Text.ASCIIEncoding.ASCII.GetString(pt[i].Value, 0, pt[i].Value.Length - 1).Trim() + " ";
                        break;
                    case 0x829A: // 曝光时间  
                        property += "快门 " + GetValueOfType5(p.Value) + " ";
                        break;
                    case 0x8827: // ISO  
                        property += "ISO " + GetValueOfType3(p.Value) + " ";
                        break;
                    case 0x920a: //焦距
                        property += "焦距 " + GetValueOfType5A(p.Value) + " ";
                        break;
                    case 0x829D: //光圈
                        property += "光圈 " + GetValueOfType5A(p.Value) + " ";
                        break;
                    case 0xA433:
                        property += "镜头 " + System.Text.ASCIIEncoding.ASCII.GetString(pt[i].Value, 0, pt[i].Value.Length - 1).Trim() + " ";
                        break;
                    case 0xA434:
                        property += "镜头 "+System.Text.ASCIIEncoding.ASCII.GetString(pt[i].Value, 0, pt[i].Value.Length - 1).Trim() + " ";
                        break;
                    case 0x112:
                        orientation = Convert.ToUInt16(pt[i].Value[1] << 8 | pt[i].Value[0]);
                        break;
                    default:
                        break;

                }
            }
            switch (orientation)
            {
                case 2:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipX);//horizontal flip
                    break;
                case 3:
                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);//right-top
                    break;
                case 4:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipY);//vertical flip
                    break;
                case 5:
                    img.RotateFlip(RotateFlipType.Rotate90FlipX);
                    break;
                case 6:
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);//right-top
                    break;
                case 7:
                    img.RotateFlip(RotateFlipType.Rotate270FlipX);
                    break;
                case 8:
                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);//left-bottom
                    break;
                default:
                    break;
            }
            return property;
        }

        private static string GetValueOfType3(byte[] b) //对type=3 的value值进行读取
        {
            if (b.Length != 2) return "";
            return Convert.ToUInt16(b[1] << 8 | b[0]).ToString();
        }

        private static string GetValueOfType5(byte[] b) //对type=5 的value值进行读取
        {
            if (b.Length != 8) return "";
            UInt32 fm, fz;
            fm = 0;
            fz = 0;
            fz = Convert.ToUInt32(b[7] << 24 | b[6] << 16 | b[5] << 8 | b[4]);
            fm = Convert.ToUInt32(b[3] << 24 | b[2] << 16 | b[1] << 8 | b[0]);
            fz = fz / fm;
            fm = 1;
            return fm.ToString() + "/" + fz.ToString();
        }

        private static string GetValueOfType5A(byte[] b)//获取光圈的值
        {
            if (b.Length != 8) return "";
            UInt32 fm, fz;
            fm = 0;
            fz = 0;
            fz = Convert.ToUInt32(b[7] << 24 | b[6] << 16 | b[5] << 8 | b[4]);
            fm = Convert.ToUInt32(b[3] << 24 | b[2] << 16 | b[1] << 8 | b[0]);
            double temp = (double)fm / fz;
            return (temp).ToString();

        }
    }
}
