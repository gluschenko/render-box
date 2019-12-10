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
using System.Diagnostics;

namespace PathTracerSharp
{
    public partial class MainWindow : Window
    {
        public WriteableBitmap Bitmap { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Bitmap = GetBitmap(Image);

            var timer = Stopwatch.StartNew();

            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    DrawPixel(Bitmap, new Point(x, y));
                }
            }

            MessageBox.Show($"{timer.ElapsedMilliseconds} ms");
            timer.Restart();
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.writeablebitmap?redirectedfrom=MSDN&view=netframework-4.8
        /// </summary>
        /// <param name="img"></param>
        WriteableBitmap GetBitmap(Image img) 
        {
            RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(img, EdgeMode.Aliased);

            img.Width = Width;
            img.Height = Height;

            var bitmap = new WriteableBitmap((int)img.Width, (int)img.Height, 96, 96, PixelFormats.Bgr32, null);

            img.Source = bitmap;
            img.Stretch = Stretch.None;

            return bitmap;
        }

        void DrawPixel(WriteableBitmap bitmap, Point point)
        {
            int column = (int)point.X;
            int row = (int)point.Y;

            try
            {
                // Reserve the back buffer for updates.
                bitmap.Lock();

                unsafe
                {
                    // Get a pointer to the back buffer.
                    IntPtr pBackBuffer = bitmap.BackBuffer;

                    // Find the address of the pixel to draw.
                    pBackBuffer += row * bitmap.BackBufferStride;
                    pBackBuffer += column * 4;

                    // Compute the pixel's color.
                    int color_data = 
                        255 << 16 | // R
                        128 << 8 |  // G
                        255 << 0;   // B

                    // Assign the color data to the pixel.
                    *(int*)pBackBuffer = color_data;
                }

                // Specify the area of the bitmap that changed.
                bitmap.AddDirtyRect(new Int32Rect(column, row, 1, 1));
            }
            finally
            {
                // Release the back buffer and make it available for display.
                bitmap.Unlock();
            }
        }
    }
}
