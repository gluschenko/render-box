using RenderBox.Core;
using RenderBox.Options;
using RenderBox.Pages;
using RenderBox.Rendering;
using RenderBox.Shared.Modules.PathTracer;
using RenderBox.Shared.Modules.PathTracer.Shapes;
using System;
using System.Windows.Input;
using static RenderBox.Core.VectorMath;

namespace RenderBox.Modules
{
    [OptionsPage(typeof(PathTracerPage))]
    public class PathRenderer : Renderer
    {
        public Camera MainCamera { get; set; }
        public Scene Scene { get; set; }

        public bool ShowNormals { get; set; }

        public PathRenderer(Paint paint) : base(paint)
        {
            MainCamera = new Camera(new Vector3(0, 0, 4));
            Scene = new Scene();

            Scene.Shapes.AddRange(new Shape[]
            {
                new Sphere(new Vector3(-4, -2, 0), .6f, Color.Black),
                new Sphere(new Vector3(-2, -2, 0), .6f, Color.Black),
                new Sphere(new Vector3(0, -2, 0), .6f, Color.Black),
                new Sphere(new Vector3(2, -2, 0), .6f, Color.Black),
                new Sphere(new Vector3(4, -2, 0), .6f, Color.Black),

                new Sphere(new Vector3(-4, 0, 0), .5f, Color.Red),
                new Sphere(new Vector3(-2, 0, 0), .6f, Color.Yellow),
                new Sphere(new Vector3(0, 0, 0), .7f, Color.Green),
                new Sphere(new Vector3(2, 0, 0), .6f, Color.Blue),
                new Sphere(new Vector3(4, 0, 0), .5f, Color.Red),

                new Sphere(new Vector3(-4, 2, 0), .6f, Color.Black),
                new Sphere(new Vector3(-2, 2, 0), .6f, Color.Black),
                new Sphere(new Vector3(0, 2, 0), .6f, Color.Black),
                new Sphere(new Vector3(2, 2, 0), .6f, Color.Black),
                new Sphere(new Vector3(4, 2, 0), .6f, Color.Black),

                //new Box(new Vector3(1, 1, -30), Color.Black),
            });

            Scene.Lights.AddRange(new Light[]
            {
                new Light(new Vector3(4, 0, 4), 10),
            });
        }

        protected override void RenderScreen(RenderContext context)
        {
            var width = context.Width;
            var height = context.Height;
            var dispatcher = context.Dispatcher;
            var camera = MainCamera;
            var scene = Scene;

            var samples = 4;
            var samplesWidth = width * samples;
            var samplesHeight = height * samples;

            float scale = (float)Math.Tan(MathHelpres.DegToRad(camera.FOV * 0.5));
            float aspectRatio = (float)width / height;

            Vector3 orig = camera.Position;

            float halfX = width / 2;
            float halfY = height / 2;

            Color[,] Batch(int ix, int iy, int sizeX, int sizeY)
            {
                var tile = new Color[sizeX, sizeY];

                for (int localY = 0; localY < sizeY; localY++)
                {
                    int y = iy + localY;

                    for (int localX = 0; localX < sizeX; localX++)
                    {
                        int x = ix + localX;
                        //
                        float posX = (2 * (x + 0.5f) / width - 1) * aspectRatio * scale;
                        float posY = (1 - 2 * (y + 0.5f) / height) * scale;
                        //
                        var dir = Normalize(new Vector3(posX, posY, -1));
                        var ray = new Ray(orig, dir);
                        //
                        var color = TracePath(context, camera, scene, ray, scene.BackgroundColor, 0);
                        tile[localX, localY] = color;
                    }
                }

                return tile;
            }

            lock (camera)
            {
                lock (scene)
                {
                    BatchScreen(context, Batch);
                }
            }
        }

        private Color TracePath(RenderContext context, Camera camera, Scene scene, Ray ray, Color back, int depth)
        {
            // Bounced enough times
            if (depth >= camera.MaxDepth)
            {
                return back;
            }

            var hit = FindClosestHit(scene, ray);

            if (!hit.IsHitting)
            {
                return back; // Nothing was hit
            }

            var material = hit.HitObject.Material;
            var emittance = material.Diffuse; //material.emittance;

            var position = hit.Position;
            var normal = hit.HitObject.CalcNormal(position);

            if (ShowNormals)
            {
                return new Color(normal.x, normal.y, normal.z);
            }

            var newRay = new Ray(position, normal);

            Color BRDF = material.Specular / (float)Math.PI;

            Color incoming = TracePath(context, camera, scene, newRay, back, depth + 1);
            return emittance + (BRDF * incoming);
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

        private static Hit FindClosestHit(Scene scene, Ray ray)
        {
            var closest = new Hit();

            var minDist = double.PositiveInfinity;

            foreach (var shape in scene.Shapes)
            {
                var isHit = shape.GetIntersection(ray, 10, out Hit localHit, out var distance);

                if (!isHit)
                {
                    continue;
                }

                if (distance < minDist)
                {
                    minDist = distance;
                    closest = localHit;
                }
            }

            foreach (var light in scene.Lights)
            {

            }

            return closest;
        }

        public override void OnKeyPress(Key key, Action onRender)
        {
            var origPos = MainCamera.Position;

            if (key == Key.E) MainCamera.Position += Vector3.Back * 0.5f;
            if (key == Key.Q) MainCamera.Position += Vector3.Forward * 0.5f;
            if (key == Key.A) MainCamera.Position += Vector3.Left * 0.5f;
            if (key == Key.D) MainCamera.Position += Vector3.Right * 0.5f;
            if (key == Key.W) MainCamera.Position += Vector3.Up * 0.5f;
            if (key == Key.S) MainCamera.Position += Vector3.Down * 0.5f;

            if (origPos != MainCamera.Position)
            {
                onRender();
            }
        }
    }
}
