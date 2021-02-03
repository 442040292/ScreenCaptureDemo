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
using System.Windows.Shapes;

namespace ScreenCaptureDemo
{
    /// <summary>
    /// Interaction logic for CaptureWindow.xaml
    /// </summary>
    public partial class CaptureWindow : Window
    {
        public CaptureWindow()
        {
            InitializeComponent();

            Load();
        }


        private void Load()
        {
            var width = System.Windows.Forms.Screen.AllScreens.Max(x => x.Bounds.X + x.Bounds.Width);
            var height = System.Windows.Forms.Screen.AllScreens.Max(x => x.Bounds.Y + x.Bounds.Height);
            this.Width = width;
            this.Height = height;
            this.Left = 0;
            this.Top = 0;
            bitmap = Helper.GetFullScreen();
            origin.Source = back.Source = ImageHelper.ChangeBitmapToImageSource(bitmap);
            bigView.SetBackImage(bitmap);

        }




        public bool IsOpenBigView
        {
            get { return (bool)GetValue(IsOpenBigViewProperty); }
            set { SetValue(IsOpenBigViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpenBigView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenBigViewProperty =
            DependencyProperty.Register("IsOpenBigView", typeof(bool), typeof(CaptureWindow), new PropertyMetadata(false));




        public System.Windows.Point BigViewLocation
        {
            get { return (System.Windows.Point)GetValue(BigViewLocationProperty); }
            set { SetValue(BigViewLocationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BigViewLocation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BigViewLocationProperty =
            DependencyProperty.Register("BigViewLocation", typeof(System.Windows.Point), typeof(CaptureWindow), new PropertyMetadata(null));




        #region 框选
        System.Drawing.Bitmap bitmap;
        bool IsSelecting = false;
        System.Drawing.Point startPoint;
        System.Drawing.Rectangle selectImageRect;

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsSelecting = true;
            mypop.IsOpen = false;
            if (IsSelecting)
            {
                startPoint = System.Windows.Forms.Cursor.Position;
            }
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            var position = System.Windows.Forms.Cursor.Position;
            if (IsSelecting)
            {
                double x = Math.Min(position.X, startPoint.X);
                double y = Math.Min(position.Y, startPoint.Y);

                selectRange.Margin = new Thickness(x, y, 0, 0);
                selectRange.Width = Math.Abs(position.X - startPoint.X);
                selectRange.Height = Math.Abs(position.Y - startPoint.Y);
                selectImageRect = new System.Drawing.Rectangle((int)x, (int)y, (int)selectRange.Width, (int)selectRange.Height);
                this.Dispatcher.InvokeAsync(() =>
                {
                    origin.Margin = new Thickness(-x, -y, 0, 0);
                    origin.Width = this.Width;
                    origin.Height = this.Height;
                });
            }

            IsOpenBigView = false;
            IsOpenBigView = true;

            bigView.ChangeLoacation(position);
        }


        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsSelecting = false;
            mypop.IsOpen = true;
        }

        #endregion

        #region 普通事件
        private void Esc_Click(object sender, ExecutedRoutedEventArgs e)
        {
            //this.Close();
            DialogResult = false;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }


        /// <summary>
        /// 为了能直接拿到结果
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Bitmap ShowDialog()
        {
            var result = base.ShowDialog();
            if (result == true)
            {
                var selectImage = ImageHelper.DrawCaptureImage(bitmap, selectImageRect);
                return selectImage;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
