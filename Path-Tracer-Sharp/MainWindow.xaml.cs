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
using System.Runtime.CompilerServices;
using PathTracerSharp.Shapes;

namespace PathTracerSharp
{
    public enum ShadingType
    {
        Diffuse = 0,
        DiffuseGlossy = 1,
        Reflection = 2,
        ReflectionAndRefraction = 3, 
    }

    public partial class MainWindow : Window
    {
        public Paint Paint { get; set; }

        public int maxDepth = 3;
        public double FOV = 0.5;
        public Color backgroundColor = new Color(.2f, .2f, .2f);

        readonly List<Shape> Shapes = new List<Shape>();
        readonly List<Light> Lights = new List<Light>();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            MouseMove += MainWindow_MouseMove;

            Shapes.AddRange(new Shape[] {
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

                //new Sphere(new Vector(1, 1, -3), 2f, Color.Black) { specular = Color.White },
                new Box(new Vector(1, 1, -3), new Vector(2, 2, 2), new Vector(-2, -2, -2), Color.Black) { specular = Color.White },
            });

            Lights.AddRange(new Light[] {
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
                        var color = TracePath(ray, backgroundColor, 0);
                        Paint.SetPixel(new Point(x, y), color.GetRaw());

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
            // Bounced enough times
            if (depth >= maxDepth) return back; 

            var closestHit = FindClosest(Shapes, ray);

            if (!closestHit.IsHitting)
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
            //const float p = 1f / (2f * (float)Math.PI);

            // Compute the BRDF for this ray (assuming Lambertian reflection)
            //float cos_theta = Vector.Dot(newRay.direction, normal);
            Color BRDF = closestHit.hitObject.specular / (float)Math.PI;

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

            float min_dist = float.MaxValue;

            foreach (var shape in shapes)
            {
                float distance = shape.GetIntersection(ray, out Hit localHit);
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
