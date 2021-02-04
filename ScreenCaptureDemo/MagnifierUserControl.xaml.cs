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
        ImageSource bacImage;

        public void SetBackImage(System.Drawing.Bitmap bitmapA)
        {
            bacImage = ImageHelper.ChangeBitmapToImageSource(bitmapA);
            bitmap = bitmapA;
            back.Source = bacImage;
        }

        public void ChangeLoacation(System.Drawing.Point point)
        {
            int size = 10;
            int rate = 160 / size;
            if (point.X <= size || point.Y <= size || (bitmap.Width - point.X) <= size || (bitmap.Height - point.Y) <= size)
            {
                back.Source = null;
            }
            else
            {
                back.Margin = new Thickness((-point.X - size) * rate, (-point.Y - size) * rate, 0, 0);
                back.Width = bitmap.Width * rate;
                back.Height = bitmap.Height * rate;
                back.Source = bacImage;

                //var showImage = ScreenCapture.DrawCaptureImage(bitmap, new System.Drawing.Rectangle((int)(point.X - size), (int)(point.Y - size), size, size));
                //back.Source = ImageHelper.ChangeBitmapToImageSource(showImage);
            }
        }
    }
}
