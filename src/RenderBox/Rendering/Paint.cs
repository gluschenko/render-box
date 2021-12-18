using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = RenderBox.Core.Color;

namespace RenderBox.Rendering
{
    public class Paint : IDisposable
    {
        public const int DPI = 96;

        public Image Image { get; private set; }
        public WriteableBitmap Bitmap { get; private set; }
        public int Width => Bitmap?.PixelWidth ?? 0;
        public int Height => Bitmap?.PixelHeight ?? 0;
        public double Scale { get; private set; }

        public Paint(Image img, int width, int height, double scale)
        {
            Image = img;
            Scale = scale;
            CreateBitmap(img, width, height);
        }

        public void Dispose()
        {
            Bitmap = null;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.writeablebitmap?redirectedfrom=MSDN&view=netframework-4.8
        /// </summary>
        private void CreateBitmap(Image img, int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                return;
            }

            RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.HighQuality);
            RenderOptions.SetEdgeMode(img, EdgeMode.Aliased);

            var bitmap = new WriteableBitmap(width, height, DPI, DPI, PixelFormats.Bgr32, null);
            img.Source = bitmap;

            Bitmap = null;
            Bitmap = bitmap;
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.writeablebitmap?redirectedfrom=MSDN&view=netframework-4.8
        /// </summary>
        public void SetPixel(int x, int y, int color)
        {
            if (Bitmap == null)
            {
                return;
            }

            var isValid = x >= 0 && y >= 0 && x < Bitmap.PixelWidth && y < Bitmap.PixelHeight;

            if (!isValid)
            {
                return;
            }

            Bitmap.Lock();

            unsafe
            {
                var backBuffer = Bitmap.BackBuffer;

                backBuffer += y * Bitmap.BackBufferStride;
                backBuffer += x * 4;

                *(int*)backBuffer = color;
            }

            Bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
            Bitmap.Unlock();
        }

        public void SetPixels(int x, int y, Color[,] colors)
        {
            if (Bitmap == null)
            {
                return;
            }

            var width = colors.GetLength(0);
            var height = colors.GetLength(1);

            var isValid = x >= 0 && y >= 0 && x + width < Bitmap.PixelWidth && y + height < Bitmap.PixelHeight;
            if (!isValid)
            {
                return;
            }

            Bitmap.Lock();

            unsafe
            {
                var backBuffer = Bitmap.BackBuffer;

                const int Pixel = sizeof(int);

                backBuffer += y * Bitmap.PixelWidth * Pixel;
                backBuffer += (x * Pixel) - Pixel;

                var newLineOffset = (Bitmap.PixelWidth - width) * Pixel;

                for (var localY = 0; localY < height; localY++)
                {
                    for (var localX = 0; localX < width; localX++)
                    {
                        backBuffer += Pixel;
                        *(int*)backBuffer = (int)colors[localX, localY];
                    }

                    backBuffer += newLineOffset;
                }
            }

            Bitmap.AddDirtyRect(new Int32Rect(x, y, width, height));
            Bitmap.Unlock();
        }
    }
}
