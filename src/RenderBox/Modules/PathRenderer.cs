using RenderBox.Core;
using RenderBox.Rendering;
using RenderBox.Shared.Modules.PathTracer;
using RenderBox.Shared.Modules.PathTracer.Shapes;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace RenderBox.Modules
{
    public class PathRenderer : Renderer
    {
        public Camera MainCamera { get; set; }
        public Scene Scene { get; set; }

        public PathRenderer(Paint paint) : base(paint)
        {
            MainCamera = new Camera(new Vector3(0, 0, 4), Vector3.Zero);
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

                //new Sphere(new Vector(1, 1, -3), 2f, Color.Black),
                new Box(new Vector3(1, 1, -3), Color.Black),
            });

            Scene.Lights.AddRange(new Light[]
            {
                new Light(new Vector3(4, 2, 3), 10),
            });
        }

        protected override void RenderScreen(RenderContext context)
        {
            var width = context.Width;
            var height = context.Height;
            var dispatcher = context.Dispatcher;
            var camera = MainCamera;
            var scene = Scene;

            float scale = (float)Math.Tan(MathHelpres.DegToRad(camera.FOV * 0.5)); //width / 20;
            float aspectRatio = (float)width / height;

            Vector3 orig = camera.Position;
            //
            float halfX = width / 2;
            float halfY = height / 2;
            //
            lock (camera)
            {
                lock (scene)
                {
                    Color[,] batch(int ix, int iy, int sizeX, int sizeY)
                    {
                        Color[,] tile = new Color[sizeX, sizeY];

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
                                var dir = Vector3.Normalize(new Vector3(posX, posY, -1));
                                var ray = new Ray(orig, dir);
                                //
                                var color = TracePath(context, camera, scene, ray, scene.BackgroundColor, 0);
                                tile[localX, localY] = color;
                            }
                        }

                        return tile;
                    }

                    BatchScreen(context, batch);
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

            var closestHit = FindClosest(scene.Shapes, ray);

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
            //float cos_theta = (float)Vector3.Dot(newRay.direction, normal);
            Color BRDF = material.specular / (float)Math.PI;

            // Recursively trace reflected light sources.
            Color incoming = TracePath(context, camera, scene, newRay, back, depth + 1);

            // Apply the Rendering Equation here.
            return emittance + (BRDF * incoming /** cos_theta / p*/);
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
