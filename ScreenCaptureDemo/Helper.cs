using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScreenCaptureDemo
{
    public class Helper
    {

        public static Bitmap GetFullScreen()
        {

            Win32Helper.PrintScreen();
            IDataObject iObj = Clipboard.GetDataObject();
            if (iObj.GetDataPresent(typeof(Bitmap)))
            {
                Bitmap bmpScreen = iObj.GetData(typeof(Bitmap)) as Bitmap;
                return bmpScreen;
            }
            else
            {
                return null;
            }
        }

    }
}
