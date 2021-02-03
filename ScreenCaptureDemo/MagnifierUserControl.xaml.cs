using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScreenCaptureDemo
{
    /// <summary>
    /// Interaction logic for MagnifierUserControl.xaml
    /// </summary>
    public partial class MagnifierUserControl : UserControl
    {
        public MagnifierUserControl()
        {
            InitializeComponent();
        }

        System.Drawing.Bitmap bitmap;


        public void SetBackImage(System.Drawing.Bitmap bitmapA)
        {
            back.Source = ImageHelper.ChangeBitmapToImageSource(bitmapA);
            bitmap = bitmapA;
        }

        public void ChangeLoacation(System.Drawing.Point point)
        {
            if (point.X <= 40 || point.Y <= 40 || (bitmap.Width - point.X) <= 40 || (bitmap.Height - point.Y) <= 40)
            {
                back.Source = null;
            }
            else
            {
                var showImage = ScreenCapture.DrawCaptureImage(bitmap, new System.Drawing.Rectangle((int)(point.X - 40), (int)(point.Y - 20), 80, 40));
                back.Source = ImageHelper.ChangeBitmapToImageSource(showImage);
            }
        }
    }
}
