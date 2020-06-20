using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = PathTracerSharp.Core.Color;

namespace PathTracerSharp.Rendering
{
    public class Paint : IDisposable
    {
        public const int DPI = 96;

        public Image Image { get; private set; }
        public WriteableBitmap Bitmap { get; private set; }
        public int Width => Bitmap.PixelWidth;
        public int Height => Bitmap.PixelHeight;

        public Paint(Image img, int width, int height)
        {
            Image = img;
            Bitmap = CreateBitmap(img, width, height);
        }

        public Paint(Image img, double width, double height) : this(img, (int)width, (int)height) { }

        public void Dispose()
        {
            Bitmap = null;
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.writeablebitmap?redirectedfrom=MSDN&view=netframework-4.8
        /// </summary>
        private WriteableBitmap CreateBitmap(Image img, int width, int height)
        {
            RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.HighQuality);
            RenderOptions.SetEdgeMode(img, EdgeMode.Aliased);

            var bitmap = new WriteableBitmap(width, height, DPI, DPI, PixelFormats.Bgr32, null);
            img.Source = bitmap;
            return bitmap;
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.writeablebitmap?redirectedfrom=MSDN&view=netframework-4.8
        /// </summary>
        public void SetPixel(int x, int y, int color)
        {
            bool is_valid = x >= 0 && y >= 0 && x < Bitmap.PixelWidth && y < Bitmap.PixelHeight;
            if (!is_valid) return;


            // Reserve the back buffer for updates
            Bitmap.Lock();

            unsafe
            {
                // Get a pointer to the back buffer
                IntPtr backBuffer = Bitmap.BackBuffer;

                // Find the address of the pixel to draw
                backBuffer += y * Bitmap.BackBufferStride;
                backBuffer += x * 4;

                // Assign the color data to the pixel
                *(int*)backBuffer = color;
            }

            // Specify the area of the bitmap that changed
            Bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));

            // Release the back buffer and make it available for display
            Bitmap.Unlock();
        }

        public void SetPixels(int x, int y, Color[,] colors) 
        {
            int width = colors.GetLength(0);
            int height = colors.GetLength(1);

            bool is_valid = x >= 0 && y >= 0 && x + width < Bitmap.PixelWidth && y + height < Bitmap.PixelHeight;
            if (!is_valid) return;


            // Reserve the back buffer for updates
            Bitmap.Lock();

            unsafe
            {
                for (int localY = 0; localY < height; localY++)
                {
                    // Get a pointer to the back buffer
                    IntPtr backBuffer = Bitmap.BackBuffer;

                    // Find the address of the pixel to draw
                    backBuffer += (y + localY) * Bitmap.BackBufferStride;
                    backBuffer += x * 4;

                    for (int localX = 0; localX < width; localX++)
                    {
                        backBuffer += 4;

                        // Assign the color data to the pixel
                        *(int*)backBuffer = (int)colors[localX, localY];
                    }
                }
            }

            // Specify the area of the bitmap that changed
            Bitmap.AddDirtyRect(new Int32Rect(x, y, width, height));

            // Release the back buffer and make it available for display
            Bitmap.Unlock();
        }
    }
}
