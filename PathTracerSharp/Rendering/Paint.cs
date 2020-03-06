﻿using System;
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
            Bitmap = GetBitmap(img, width, height);
        }

        public Paint(Image img, double width, double height) : this(img, (int)width, (int)height) { }

        public void Dispose()
        {

        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.writeablebitmap?redirectedfrom=MSDN&view=netframework-4.8
        /// </summary>
        /// <param name="img"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private WriteableBitmap GetBitmap(Image img, int width, int height)
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
        /// <param name="x">X-coord</param>
        /// <param name="y">Y-coord</param>
        /// <param name="color">Raw color code</param>
        public void SetPixel(int x, int y, int color)
        {
            bool is_valid = x >= 0 && y >= 0 && x < Bitmap.PixelWidth && y < Bitmap.PixelHeight;
            if (!is_valid) return;

            try
            {
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
            }
            finally
            {
                // Release the back buffer and make it available for display
                Bitmap.Unlock();
            }
        }

        public void SetPixels(int x, int y, Color[,] colors) 
        {
            int width = colors.GetLength(0);
            int height = colors.GetLength(1);

            bool is_valid = x >= 0 && y >= 0 && x + width < Bitmap.PixelWidth && y + height < Bitmap.PixelHeight;
            if (!is_valid) return;

            try
            {
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

                    /*for (int localY = 0; localY < height; localY++)
                    {
                        for (int localX = 0; localX < width; localX++)
                        {
                            // Get a pointer to the back buffer
                            IntPtr backBuffer = Bitmap.BackBuffer;
                            // Find the address of the pixel to draw
                            backBuffer += (y + localY) * Bitmap.BackBufferStride;
                            backBuffer += (x + localX) * 4;

                            // Assign the color data to the pixel
                            *(int*)backBuffer = (int)colors[localX, localY];
                        }
                    }*/
                }

                // Specify the area of the bitmap that changed
                Bitmap.AddDirtyRect(new Int32Rect(x, y, width, height));
            }
            finally
            {
                // Release the back buffer and make it available for display
                Bitmap.Unlock();
            }
        }
    }
}