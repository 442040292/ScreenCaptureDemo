using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenCaptureDemo
{


    public class Utils
    {

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

    }
}
