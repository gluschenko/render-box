using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public int MaxDepth = 3;

        readonly List<Sphere> Spheres = new List<Sphere>();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            MouseMove += MainWindow_MouseMove;

            Spheres.AddRange(new Sphere[] {
                new Sphere(new Vector(-2, 0, 0), 0.5f),
                new Sphere(new Vector(0, 0, 0), 1f),
                new Sphere(new Vector(3, 0, 0), 1.5f),
            });
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) 
            {
                var pos = e.GetPosition(Image);
                DrawPixel(Bitmap, pos, Color.Yellow.GetRaw());
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Bitmap = GetBitmap(Image);

            var w = Bitmap.PixelWidth;
            var h = Bitmap.PixelHeight;
            Thread thread = new Thread(() => Proc(w, h)) { IsBackground = true };
            thread.Start();

            //var timer = Stopwatch.StartNew();

            /*MessageBox.Show($"{timer.ElapsedMilliseconds} ms");
            timer.Restart();*/
        }

        public void Proc(int width, int height) 
        {
            var backColor = new Color(.2f, .2f, .2f).GetRaw();
            var sphereColor = Color.Green.GetRaw();

            float scale = 50;

            float halfX = width / 2;
            float halfZ = height / 2;

            Vector source = new Vector(0, 0, 4);

            for (int x = 0; x < width; x++)
            {
                float posX = (x - halfX) / scale;

                Dispatcher.Invoke(() => {
                    for (int z = 0; z < height; z++)
                    {
                        float posZ = (z - halfZ) / scale;

                        var pos = new Vector(posX, 0, posZ);
                        var ray = new Ray(source, new Vector(100, 100, 0));
                        //
                        var color = backColor;

                        foreach (var sphere in Spheres)
                        {
                            //if (Vector.Distance(sphere.position, pos) > sphere.radius)
                            {
                                var fff = sphere.Intersect(ray, out Hit hit);
                                if (fff > 0) 
                                {
                                    color = new Color(fff, fff, fff).GetRaw();
                                }
                            }
                        }

                        foreach (var sphere in Spheres) 
                        {
                            if (Vector.Distance(sphere.position, pos) < sphere.radius)
                            {
                                color = sphereColor;
                            }
                        }

                        DrawPixel(Bitmap, new Point(x, z), color);
                    }
                });
            }
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.writeablebitmap?redirectedfrom=MSDN&view=netframework-4.8
        /// </summary>
        /// <param name="img"></param>
        WriteableBitmap GetBitmap(Image img) 
        {
            RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(img, EdgeMode.Aliased);

            var bitmap = new WriteableBitmap((int)ActualWidth, (int)ActualHeight, 96, 96, PixelFormats.Bgr32, null);

            img.Source = bitmap;
            img.Stretch = Stretch.None;

            return bitmap;
        }

        void DrawPixel(WriteableBitmap bitmap, Point point, int color)
        {
            int x = (int)point.X;
            int y = (int)point.Y;

            if (!(x >= 0 && y >= 0 && x < bitmap.PixelWidth && y < bitmap.PixelHeight)) return;

            try
            {
                // Reserve the back buffer for updates
                bitmap.Lock();

                unsafe
                {
                    // Get a pointer to the back buffer
                    IntPtr pBackBuffer = bitmap.BackBuffer;
                    
                    // Find the address of the pixel to draw
                    pBackBuffer += y * bitmap.BackBufferStride;
                    pBackBuffer += x * 4;

                    // Assign the color data to the pixel
                    *(int*)pBackBuffer = color;
                }

                // Specify the area of the bitmap that changed
                bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
            }
            finally
            {
                // Release the back buffer and make it available for display
                bitmap.Unlock();
            }
        }

        /*Color TracePath(Ray ray, int depth)
        {
            if (depth >= MaxDepth)
            {
                return Color.Black;  // Bounced enough times
            }

            ray.FindNearestObject();
            if (ray.hitSomething == false)
            {
                return Color.Black;  // Nothing was hit
            }

            Material material = ray.thingHit->material;
            Color emittance = material.emittance;

            // Pick a random direction from here and keep going
            Ray newRay;
            newRay.origin = ray.pointWhereObjWasHit;

            // This is NOT a cosine-weighted distribution!
            newRay.direction = RandomUnitVectorInHemisphereOf(ray.normalWhereObjWasHit);

            // Probability of the newRay
            const double p = 1.0 / (2.0 * Math.PI);

            // Compute the BRDF for this ray (assuming Lambertian reflection)
            float cos_theta = DotProduct(newRay.direction, ray.normalWhereObjWasHit);
            Color BRDF = material.reflectance / Math.PI;

            // Recursively trace reflected light sources.
            Color incoming = TracePath(newRay, depth + 1);

            // Apply the Rendering Equation here.
            return emittance + (BRDF * incoming * cos_theta / p);
        }

        void Render(Image finalImage, count numSamples)
        {
            foreach (pixel in finalImage)
            {
                foreach (i in numSamples)
                {
                    Ray r = camera.generateRay(pixel);
                    pixel.color += TracePath(r, 0);
                }
                pixel.color /= numSamples;  // Average samples.
            }
        }*/
    }
}
