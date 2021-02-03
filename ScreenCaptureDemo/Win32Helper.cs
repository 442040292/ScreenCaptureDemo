using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows;

namespace ScreenCaptureDemo
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public class Win32Helper
    {
        [DllImport("user32")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);



        //根据坐标获取窗口句柄
        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(int xPoint, int yPoint);


        public static RECT GetWindowRectByPoint(Point Point)
        {
            RECT winRect = new RECT();
            var winPtr = WindowFromPoint((int)Point.X, (int)Point.Y);
            if (winPtr == IntPtr.Zero)
            {
                return winRect;
            }

            GetWindowRect(winPtr, out winRect);
            return winRect;
        }



        [DllImport("user32.dll")]
        static extern void keybd_event
        (
            byte bVk,// 虚拟键值  
            byte bScan,// 硬件扫描码  
            uint dwFlags,// 动作标识  
            IntPtr dwExtraInfo// 与键盘动作关联的辅加信息  
        );

        /// <summary>
        /// 模拟Print Screen键盘消息。截取全屏图片。
        /// </summary>
        public static void PrintScreen()
        {
            keybd_event((byte)0x2c, 0, 0x0, IntPtr.Zero);//down
            System.Windows.Forms.Application.DoEvents();
            keybd_event((byte)0x2c, 0, 0x2, IntPtr.Zero);//up
            System.Windows.Forms.Application.DoEvents();
        }


    }
}
