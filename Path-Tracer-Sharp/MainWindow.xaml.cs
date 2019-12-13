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
                new Sphere(new Vector(-4, -2, 0), .5f, Color.Black) { specular = Color.White },
                new Sphere(new Vector(-2, -2, 0), .6f, Color.Black) { specular = Color.White },
                new Sphere(new Vector(0, -2, 0), .7f, Color.Black) { specular = Color.White },
                new Sphere(new Vector(2, -2, 0), .6f, Color.Black) { specular = Color.White },
                new Sphere(new Vector(4, -2, 0), .5f, Color.Black) { specular = Color.White },

                new Sphere(new Vector(-4, 0, 0), .5f, Color.Red) { specular = Color.White },
                new Sphere(new Vector(-2, 0, 0), .6f, Color.Yellow) { specular = Color.White },
                new Sphere(new Vector(0, 0, 0), .7f, Color.Green) { specular = Color.White },
                new Sphere(new Vector(2, 0, 0), .6f, Color.Blue) { specular = Color.White },
                new Sphere(new Vector(4, 0, 0), .5f, Color.Red) { specular = Color.White },

                new Sphere(new Vector(-4, 2, 0), .7f, Color.Black) { specular = Color.White },
                new Sphere(new Vector(-2, 2, 0), .6f, Color.Black) { specular = Color.White },
                new Sphere(new Vector(0, 2, 0), .5f, Color.Black) { specular = Color.White },
                new Sphere(new Vector(2, 2, 0), .6f, Color.Black) { specular = Color.White },
                new Sphere(new Vector(4, 2, 0), .7f, Color.Black) { specular = Color.White },

                new Sphere(new Vector(1, 1, -3), 2f, Color.Black) { specular = Color.White },
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
            var backColor = new Color(.2f, .2f, .2f);
            //var sphereColor = Color.Green.GetRaw();

            float scale = 50;

            float halfX = width / 2;
            float halfY = height / 2;

            Vector source = new Vector(0, 0, 4);

            for (int x = 0; x < width; x++)
            {
                float posX = (x - halfX) / scale;

                Dispatcher.Invoke(() => {
                    for (int y = 0; y < height; y++)
                    {
                        float posY = (y - halfY) / scale;

                        var pos = new Vector(posX, posY, 0);
                        var ray = new Ray(source, pos - source);
                        //
                        var color = TracePath(ray, backColor, 0);
                        DrawPixel(Bitmap, new Point(x, y), color.GetRaw());



                        /*var color = backColor;

                        var closestHit = Sphere.FindClosest(Spheres, ray);

                        if (closestHit.IsActive) 
                        {
                            color = closestHit.hitObject.diffuse;
                        }*/

                        /*foreach (var sphere in Spheres)
                        {
                            var fff = sphere.Intersect(ray, out Hit hit);
                            if (fff != -1)
                            {
                                color = new Color(fff, fff, fff);
                            }
                        }

                        foreach (var sphere in Spheres) 
                        {
                            if (Vector.Distance(sphere.position, pos) < sphere.radius)
                            {
                                color = sphere.diffuse;
                            }
                        }

                        DrawPixel(Bitmap, new Point(x, z), color.GetRaw());*/
                    }
                });
            }
        }

        Color TracePath(Ray ray, Color back, int depth)
        {
            if (depth >= MaxDepth)
            {
                return back;  // Bounced enough times
            }

            //ray.FindNearestObject();
            var closestHit = Sphere.FindClosest(Spheres, ray);

            if (!closestHit.IsActive)
            {
                return back;  // Nothing was hit
            }

            //Material material = ray.thingHit->material;
            Color emittance = closestHit.hitObject.diffuse; //material.emittance;

            // Pick a random direction from here and keep going
            Ray newRay;
            newRay.origin = closestHit.position;

            var normal = closestHit.hitObject.CalcNormal(closestHit.position);
            // This is NOT a cosine-weighted distribution!
            newRay.direction = normal; //RandomUnitVectorInHemisphereOf(ray.normalWhereObjWasHit);

            // Probability of the newRay
            const float p = 1f / (2f * (float)Math.PI);

            // Compute the BRDF for this ray (assuming Lambertian reflection)
            float cos_theta = Vector.Dot(newRay.direction, normal);
            Color BRDF = closestHit.hitObject.specular / (float)Math.PI;

            // Recursively trace reflected light sources.
            Color incoming = TracePath(newRay, back, depth + 1);

            // Apply the Rendering Equation here.
            return emittance + (BRDF * incoming/* * cos_theta / p*/);
        }

        /*void Render(Image finalImage, int numSamples)
        {
            foreach (pixel in finalImage)
            {
                for (int i = 0; i < numSamples; i++)
                {
                    Ray r = camera.generateRay(pixel);
                    pixel.color += TracePath(r, 0);
                }
                pixel.color /= numSamples;  // Average samples
            }
        }*/

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
    }
}
