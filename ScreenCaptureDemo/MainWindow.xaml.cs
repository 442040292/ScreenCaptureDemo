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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Capture_Click(object sender, RoutedEventArgs e)
        {
            var cawindow = new CaptureWindow();
            var width = System.Windows.Forms.Screen.AllScreens.Max(x => x.Bounds.X + x.Bounds.Width);
            var height = System.Windows.Forms.Screen.AllScreens.Max(x => x.Bounds.Y + x.Bounds.Height);
            cawindow.Width = width;
            cawindow.Height = height;
            cawindow.Left = 0;
            cawindow.Top = 0;
            var image = cawindow.ShowDialog();
            if (image != null)
            {
                captureImage.Source = ImageHelper.ChangeBitmapToImageSource(image);
            }
        }



    }
}
