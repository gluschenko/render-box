using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading;
using System.Threading.Tasks;
using PathTracerSharp.Core;
using PathTracerSharp.Modules.PathTracer;

namespace PathTracerSharp.Rendering
{
    public delegate void RenderStartHandler();
    public delegate void RenderCompleteHandler();

    public class Renderer
    {
        // public
        public int ChunkSize { get; private set; }
        public Paint Paint { get; private set; }

        public RenderStartHandler RenderStart { get; set; }
        public RenderCompleteHandler RenderComplete { get; set; }
        // private
        private Thread renderThread;
        //
        private struct RenderContext 
        {
            public int width;
            public int height;
            public Camera camera;
            public Scene scene;
            public Dispatcher dispatcher;
        }

        public Renderer(Paint paint)
        {
            ChunkSize = 32;
            Paint = paint;
        }

        #region PUBLIC_METHODS

        public void Render(Camera camera, Scene scene, Dispatcher dispatcher)
        {
            Stop();

            var w = Paint.Width;
            var h = Paint.Height;

            var context = new RenderContext
            {
                width = Paint.Width,
                height = Paint.Height,
                camera = camera,
                scene = scene,
                dispatcher = dispatcher,
            };

            void process() 
            {
                // Firing begin event
                dispatcher.Invoke(() => RenderStart?.Invoke());
                // Render process
                RenderRoutine(context);
                // Firing end event
                dispatcher.Invoke(() => RenderComplete?.Invoke());
            }

            Thread thread = new Thread(process) { IsBackground = true };
            thread.Start();
        }

        public void Stop()
        {
            if (renderThread != null)
            {
                renderThread.Abort();
                renderThread = null;
            }
        }
        #endregion

        #region PRIVATE_METHODS
        private void RenderRoutine(RenderContext context)
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

            for (int iy = 0; iy < height; iy += ChunkSize)
            {
                for (int ix = 0; ix < width; ix += ChunkSize)
                {
                    int sizeX = Math.Min(ChunkSize, width - ix);
                    int sizeY = Math.Min(ChunkSize, height - iy);

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

        private Color TracePath(RenderContext context, Ray ray, Color back, int depth)
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
            newRay.direction = normal; //RandomUnitVectorInHemisphereOf(ray.normalWhereObjWasHit);

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
        #endregion
    }
}
