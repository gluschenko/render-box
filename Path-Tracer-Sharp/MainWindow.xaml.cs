using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PathTracerSharp.Shapes;
using PathTracerSharp.Rendering;
using Vector = PathTracerSharp.Rendering.Vector;

/*
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Runtime.CompilerServices;
*/

namespace PathTracerSharp
{
    public partial class MainWindow : Window
    {
        public Paint Paint { get; set; }

        public Camera MainCamera;
        public Scene Scene;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            MouseMove += MainWindow_MouseMove;

            MainCamera = new Camera(new Vector(0, 0, 4), Vector.Zero);
            Scene = new Scene();

            Scene.Shapes.AddRange(new Shape[] {
                new Sphere(new Vector(-4, -2, 0), .5f, Color.Black),
                new Sphere(new Vector(-2, -2, 0), .6f, Color.Black),
                new Sphere(new Vector(0, -2, 0), .7f, Color.Black),
                new Sphere(new Vector(2, -2, 0), .6f, Color.Black),
                new Sphere(new Vector(4, -2, 0), .5f, Color.Black),

                new Sphere(new Vector(-4, 0, 0), .5f, Color.Red),
                new Sphere(new Vector(-2, 0, 0), .6f, Color.Yellow),
                new Sphere(new Vector(0, 0, 0), .7f, Color.Green),
                new Sphere(new Vector(2, 0, 0), .6f, Color.Blue),
                new Sphere(new Vector(4, 0, 0), .5f, Color.Red),

                new Sphere(new Vector(-4, 2, 0), .7f, Color.Black),
                new Sphere(new Vector(-2, 2, 0), .6f, Color.Black),
                new Sphere(new Vector(0, 2, 0), .5f, Color.Black),
                new Sphere(new Vector(2, 2, 0), .6f, Color.Black),
                new Sphere(new Vector(4, 2, 0), .7f, Color.Black),

                //new Sphere(new Vector(1, 1, -3), 2f, Color.Black),
                new Box(new Vector(1, 1, -3), Vector.One, Color.Black),
            });

            Scene.Lights.AddRange(new Light[] {
                new Light(new Vector(4, 2, 3), 10),
            });
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) 
            {
                var pos = e.GetPosition(Image);
                Paint.SetPixel(pos, Color.Yellow.GetRaw());
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Paint = new Paint(Image, ActualWidth, ActualHeight);
            var w = Paint.Width;
            var h = Paint.Height;

            Thread thread = new Thread(() => Proc(w, h)) { IsBackground = true };
            thread.Start();

            //var timer = Stopwatch.StartNew();

            /*MessageBox.Show($"{timer.ElapsedMilliseconds} ms");
            timer.Restart();*/
        }

        public void Proc(int width, int height) 
        {
            //var sphereColor = Color.Green.GetRaw();

            int step = 16;

            float scale = 50;

            float halfX = width / 2;
            float halfY = height / 2;

            Vector source = MainCamera.Position;

            for (int iy = 0; iy < height; iy += step) 
            {
                for (int ix = 0; ix < width; ix += step) 
                {
                    int sizeX = Math.Min(step, width - ix);
                    int sizeY = Math.Min(step, height - iy);

                    Color[,] tile = new Color[sizeX, sizeY];

                    for (int y = 0; y < sizeY; y++)
                    {
                        int globalY = iy + y;
                        float posY = (globalY - halfY) / scale;

                        for (int x = 0; x < sizeX; x++)
                        {
                            int globalX = ix + x;
                            float posX = (globalX - halfX) / scale;
                            //
                            var pos = new Vector(posX, posY, 0);
                            var ray = new Ray(source, pos - source);
                            //
                            var color = TracePath(ray, Scene.BackgroundColor, 0);
                            tile[x, y] = color;
                        }
                    }

                    Dispatcher.Invoke(() => {
                        for (int y = 0; y < sizeY; y++)
                        {
                            int globalY = iy + y;

                            for (int x = 0; x < sizeX; x++)
                            {
                                int globalX = ix + x;
                                //
                                Paint.SetPixel(new Point(globalX, globalY), tile[x, y]);
                            }
                        }
                    });
                }
            }
            
            /*for (int y = 0; y < height; y++)
            {
                float posY = (y - halfY) / scale;

                for (int x = 0; x < width; x++)
                {
                    float posX = (x - halfX) / scale;

                    Dispatcher.Invoke(() => {
                        var pos = new Vector(posX, posY, 0);
                        var ray = new Ray(source, pos - source);
                        //
                        var color = TracePath(ray, backgroundColor, 0);
                        Paint.SetPixel(new Point(x, y), color);
                    });
                }
            }*/
        }

        Color TracePath(Ray ray, Color back, int depth)
        {
            // Bounced enough times
            if (depth >= MainCamera.MaxDepth) return back; 

            var closestHit = FindClosest(Scene.Shapes, ray);

            if (!closestHit.IsHitting)
            {
                return back;  // Nothing was hit
            }

            Material material = closestHit.hitObject.material;
            Color emittance = material.diffuse; //material.emittance;

            // Pick a random direction from here and keep going
            Ray newRay;
            newRay.origin = closestHit.position;

            var normal = closestHit.hitObject.CalcNormal(closestHit.position);
            // This is NOT a cosine-weighted distribution!
            newRay.direction = normal; //RandomUnitVectorInHemisphereOf(ray.normalWhereObjWasHit);

            // Probability of the newRay
            //const float p = 1f / (2f * (float)Math.PI);

            // Compute the BRDF for this ray (assuming Lambertian reflection)
            //float cos_theta = Vector.Dot(newRay.direction, normal);
            Color BRDF = material.specular / (float)Math.PI;

            // Recursively trace reflected light sources.
            Color incoming = TracePath(newRay, back, depth + 1);

            // Apply the Rendering Equation here.
            return emittance + (BRDF * incoming /* * cos_theta / p*/);
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

        Hit FindClosest(List<Shape> shapes, Ray ray)
        {
            var closest = new Hit();

            var min_dist = double.PositiveInfinity;

            foreach (var shape in shapes)
            {
                var distance = shape.GetIntersection(ray, out Hit localHit);
                if (distance != -1 && distance < min_dist)
                {
                    min_dist = distance;
                    closest = localHit;
                }
            }
            return closest;
        }
    }
}
