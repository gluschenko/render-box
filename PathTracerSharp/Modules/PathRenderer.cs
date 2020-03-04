using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using PathTracerSharp.Core;
using PathTracerSharp.Modules.PathTracer;
using PathTracerSharp.Modules.PathTracer.Shapes;
using PathTracerSharp.Rendering;

namespace PathTracerSharp.Modules
{
    public class PathRenderer : Renderer<PathTraceContext>
    {
        public Camera MainCamera { get; set; }
        public Scene Scene { get; set; }

        public PathRenderer(Paint paint) : base(paint)
        {
            MainCamera = new Camera(new Vector3(0, 0, 4), Vector3.Zero);
            Scene = new Scene();

            Scene.Shapes.AddRange(new Shape[]
            {
                new Sphere(new Vector3(-4, -2, 0), .5f, Color.Black),
                new Sphere(new Vector3(-2, -2, 0), .6f, Color.Black),
                new Sphere(new Vector3(0, -2, 0), .7f, Color.Black),
                new Sphere(new Vector3(2, -2, 0), .6f, Color.Black),
                new Sphere(new Vector3(4, -2, 0), .5f, Color.Black),

                new Sphere(new Vector3(-4, 0, 0), .5f, Color.Red),
                new Sphere(new Vector3(-2, 0, 0), .6f, Color.Yellow),
                new Sphere(new Vector3(0, 0, 0), .7f, Color.Green),
                new Sphere(new Vector3(2, 0, 0), .6f, Color.Blue),
                new Sphere(new Vector3(4, 0, 0), .5f, Color.Red),

                new Sphere(new Vector3(-4, 2, 0), .7f, Color.Black),
                new Sphere(new Vector3(-2, 2, 0), .6f, Color.Black),
                new Sphere(new Vector3(0, 2, 0), .5f, Color.Black),
                new Sphere(new Vector3(2, 2, 0), .6f, Color.Black),
                new Sphere(new Vector3(4, 2, 0), .7f, Color.Black),

                //new Sphere(new Vector(1, 1, -3), 2f, Color.Black),
                new Box(new Vector3(1, 1, -3), Vector3.One, Color.Black),
            });

            Scene.Lights.AddRange(new Light[]
            {
                new Light(new Vector3(4, 2, 3), 10),
            });
        }

        public override PathTraceContext BuildContext(Dispatcher dispatcher)
        {
            return new PathTraceContext
            {
                width = Paint.Width,
                height = Paint.Height,
                camera = MainCamera,
                scene = Scene,
                dispatcher = dispatcher,
            };
        }

        protected override void RenderRoutine(PathTraceContext context)
        {
            float scale = 50;

            var width = context.width;
            var height = context.height;
            var camera = context.camera;
            var scene = context.scene;
            var dispatcher = context.dispatcher;
            //
            float halfX = width / 2;
            float halfY = height / 2;

            Vector3 source = camera.Position;

            for (int iy = 0; iy < height; iy += BatchSize)
            {
                for (int ix = 0; ix < width; ix += BatchSize)
                {
                    int sizeX = Math.Min(BatchSize, width - ix);
                    int sizeY = Math.Min(BatchSize, height - iy);

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
                            var pos = new Vector3(posX, posY, 0);
                            var ray = new Ray(source, pos - source);
                            //
                            var color = TracePath(context, ray, scene.BackgroundColor, 0);
                            tile[x, y] = color;
                        }
                    }

                    dispatcher.Invoke(() => {
                        for (int y = 0; y < sizeY; y++)
                        {
                            int globalY = iy + y;

                            for (int x = 0; x < sizeX; x++)
                            {
                                int globalX = ix + x;
                                //
                                Paint.SetPixel(globalX, globalY, tile[x, y]);
                            }
                        }
                    });
                }
            }
        }

        private Color TracePath(PathTraceContext context, Ray ray, Color back, int depth)
        {
            // Bounced enough times
            if (depth >= context.camera.MaxDepth) return back;

            var closestHit = FindClosest(context.scene.Shapes, ray);

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
            newRay.direction = normal; //RandomUnitVectorInHemisphereOf(normal);

            // Probability of the newRay
            //const float p = 1f / (2f * (float)Math.PI);

            // Compute the BRDF for this ray (assuming Lambertian reflection)
            //float cos_theta = Vector.Dot(newRay.direction, normal);
            Color BRDF = material.specular / (float)Math.PI;

            // Recursively trace reflected light sources.
            Color incoming = TracePath(context, newRay, back, depth + 1);

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

        private Hit FindClosest(List<Shape> shapes, Ray ray)
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

    public class PathTraceContext : RenderContext 
    {
        public Camera camera;
        public Scene scene;
    }
}
