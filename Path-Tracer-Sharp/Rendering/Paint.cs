using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.CompilerServices;
using PathTracerSharp.Core;
using Color = PathTracerSharp.Core.Color;

/*
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using PathTracerSharp.Shapes;
*/

namespace PathTracerSharp.Rendering
{
    public class Paint
    {
        public Image Image { get; private set; }
        public WriteableBitmap Bitmap { get; private set; }
        public int Width => Bitmap.PixelWidth;
        public int Height => Bitmap.PixelHeight;

        public Paint(Image img, int width, int height)
        {
            Bitmap = GetBitmap(img, width, height);
        }

        public Paint(Image img, double width, double height) : this(img, (int)width, (int)height) { }

        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.writeablebitmap?redirectedfrom=MSDN&view=netframework-4.8
        /// </summary>
        /// <param name="img"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private WriteableBitmap GetBitmap(Image img, int width, int height)
        {
            RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(img, EdgeMode.Aliased);

            var bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);

            img.Source = bitmap;
            img.Stretch = Stretch.None;

            return bitmap;
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.writeablebitmap?redirectedfrom=MSDN&view=netframework-4.8
        /// </summary>
        /// <param name="x">X-coord</param>
        /// <param name="y">Y-coord</param>
        /// <param name="color">Raw color code</param>
        public void SetPixel(int x, int y, int color)
        {
            if (!(x >= 0 && y >= 0 && x < Bitmap.PixelWidth && y < Bitmap.PixelHeight)) return;

            try
            {
                // Reserve the back buffer for updates
                Bitmap.Lock();

                unsafe
                {
                    // Get a pointer to the back buffer
                    IntPtr pBackBuffer = Bitmap.BackBuffer;

                    // Find the address of the pixel to draw
                    pBackBuffer += y * Bitmap.BackBufferStride;
                    pBackBuffer += x * 4;

                    // Assign the color data to the pixel
                    *(int*)pBackBuffer = color;
                }

                // Specify the area of the bitmap that changed
                Bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
            }
            finally
            {
                // Release the back buffer and make it available for display
                Bitmap.Unlock();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixel(int x, int y, Color color) => SetPixel(x, y, color.GetRaw());
    }
}
