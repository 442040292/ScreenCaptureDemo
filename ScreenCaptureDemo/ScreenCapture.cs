using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScreenCaptureDemo
{
    /// <summary>
    /// Provides functions to capture the entire screen, or a particular window, and save it to a file.
    /// </summary>
    public class ScreenCapture
    {
        /// <summary>
        /// Creates an Image object containing a screen shot of the entire desktop
        /// </summary>
        /// <returns></returns>
        public Image CaptureScreen()
        {
            return CaptureWindow(User32.GetDesktopWindow());
        }

        public Image CaptureScreenArea(int x, int y, int right, int bottom)
        {
            Image screenImage = CaptureWindow(User32.GetDesktopWindow());
            Rectangle selectImageRect = GetSelectImageRect(x, y, right, bottom);
            Image areaImage = DrawCaptureImage(screenImage, selectImageRect);
            screenImage.Dispose();
            return areaImage;
        }

        public Image CaptureCurrentWindow()
        {
            IntPtr ptr = User32.GetForegroundWindow();
            return CaptureWindowFromScreen(ptr);
        }

        //public static string SaveScreenArea(int x, int y, int right, int bottom)
        //{
        //    try
        //    {
        //        Image screenImage = CaptureWindow(User32.GetDesktopWindow());
        //        Rectangle selectImageRect = GetSelectImageRect(x, y, right, bottom);
        //        Image img = DrawCaptureImage(screenImage, selectImageRect);
        //        screenImage.Dispose();
        //        var now = System.DateTime.Now.ToString("yyMMddHHmmssfff");
        //        var fileName = $"{now}.png";
        //        string imagePath = Path.Combine(iBotPath.GetLatestWorkflowFolder(), iBotPath.Screenshots, fileName);
        //        img.Save(imagePath, ImageFormat.Png);
        //        img.Dispose();
        //        return Path.Combine(iBotPath.Screenshots, fileName);
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error($"Failed to save captured screenshot picture.{e.ToString()} [x:[{x}], y:[{y}],right:[{right}],bottom:[{bottom}]] ScreenCapture0000000001 ");
        //    }
        //    return "";
        //}
        /// <summary>
        /// 使用窗体句柄获取窗体区域后，在全屏中截取窗体区域的图片
        /// </summary>
        /// <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
        /// <returns></returns>
        public static Image CaptureWindowFromScreen(IntPtr handle)
        {
            //截取全屏
            Image screenImage = CaptureWindow(User32.GetDesktopWindow());
            // get the size 获取窗体尺寸
            Rectangle windowRect = Utils.GetWindowRectangle(handle);
            //从全屏中截取窗体的区域
            Image img = DrawCaptureImage(screenImage, windowRect);
            //释放全屏图
            screenImage.Dispose();
            return img;
        }
        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window
        /// 风险：截取指定窗口时 如果窗体在隐藏状态下、最小化状态很有可能截取黑色或白色的图，安装了独立显卡或WIN10开启硬件加速时也有可能截取不到
        /// </summary>
        /// <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
        /// <returns></returns>
        public static Image CaptureWindow(IntPtr handle)
        {
            // get te hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            // get the size
            Rectangle windowRect = Utils.GetWindowRectangle(handle);

            int width = windowRect.Width;
            int height = windowRect.Height;
            // create a device context we can copy to
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);
            // restore selection
            GDI32.SelectObject(hdcDest, hOld);
            // clean up 
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);
            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            GDI32.DeleteObject(hBitmap);
            return img;
        }
        /// <summary>
        /// Captures a screen shot of a specific window, and saves it to a file
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
        {
            Image img = CaptureWindow(handle);
            img.Save(filename, format);
            img.Dispose();
        }
        /// <summary>
        /// Captures a screen shot of the entire desktop, and saves it to a file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public void CaptureScreenToFile(string filename, ImageFormat format)
        {
            Image img = CaptureScreen();
            img.Save(filename, format);
            img.Dispose();
        }

        public static bool CaptureRectScreen(Rectangle rect, string imgFilePath)
        {
            bool succeeded;
            try
            {
                Image screenImage = CaptureWindow(User32.GetDesktopWindow());
                Image img = DrawCaptureImage(screenImage, rect);
                screenImage.Dispose();
                new FileInfo(imgFilePath).Directory.Create();
                img.Save(imgFilePath, imgFilePath.ToLower().EndsWith(".png") ? ImageFormat.Png : ImageFormat.Jpeg);
                img.Dispose();
                succeeded = true;
            }
            catch (Exception e)
            {
                succeeded = false;
                //Log.Error($"Failed to CaptureRectScreen.{e.ToString()} [rect:[{rect}]] ScreenCapture0000000001-1");
            }
            return succeeded;
        }

        private static Rectangle GetSelectImageRect(int x, int y, int right, int bottom)
        {
            Rectangle selectImageRect = Rectangle.FromLTRB(x, y, right, bottom);
            selectImageRect.X = Math.Min(selectImageRect.X, selectImageRect.Right);
            selectImageRect.Y = Math.Min(selectImageRect.Y, selectImageRect.Bottom);
            selectImageRect.Width = Math.Max(1, Math.Abs(selectImageRect.Width));
            selectImageRect.Height = Math.Max(1, Math.Abs(selectImageRect.Height));

            return selectImageRect;
        }

        public static Image DrawCaptureImage(Image screenImage, Rectangle selectImageRect)
        {
            Bitmap bitmap = new Bitmap(selectImageRect.Width, selectImageRect.Height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.DrawImage(screenImage, 0, 0, selectImageRect, GraphicsUnit.Pixel);
            graphics.Flush();
            graphics.Dispose();

            return bitmap;
        }
        public static Bitmap DrawCaptureImage(Bitmap screenImage, Rectangle selectImageRect)
        {
            Bitmap bitmap = new Bitmap(selectImageRect.Width, selectImageRect.Height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.DrawImage(screenImage, 0, 0, selectImageRect, GraphicsUnit.Pixel);
            graphics.Flush();
            graphics.Dispose();
            return bitmap;
        }

        public static Rectangle GetWindowRectangle(IntPtr hwnd)
        {
            try
            {
                RECT rect = new RECT();
                bool result = Win32Helper.GetWindowRect(hwnd, out rect);
                if (!result)
                    return Rectangle.Empty;

                return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
            }
            catch (Exception ex)
            {
                //Log.Error($"GetWindowRectangle hwnd[{hwnd}] Utils0000000003 Error:{ex}");
                return Rectangle.Empty;
            }
        }


        /// <summary>
        /// Helper class containing Gdi32 API functions
        /// </summary>
        private class GDI32
        {

            public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                int nWidth, int nHeight, IntPtr hObjectSource,
                int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        /// <summary>
        /// Helper class containing User32 API functions
        /// </summary>
        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            public static extern IntPtr GetForegroundWindow();
        }
    }
}
