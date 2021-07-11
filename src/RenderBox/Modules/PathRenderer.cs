using RenderBox.Core;
using RenderBox.Options;
using RenderBox.Pages;
using RenderBox.Rendering;
using RenderBox.Shared.Modules.PathTracer;
using RenderBox.Shared.Modules.PathTracer.Shapes;
using System;
using System.Linq;
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
        public bool ShowDepth { get; set; }

        public PathRenderer(Paint paint) : base(paint)
        {
            MainCamera = new Camera(new Vector3(0, 0, 4));
            Scene = new Scene();

            var glass = new Material
            {
                Refraction = .5f,
            };

            var mirror = new Material
            {
                Reflection = 1f,
            };

            var lamp1 = new Light(Color.White, 3f);
            var lamp2 = new Light(Color.White, 3f);
            var lamp3 = new Light(Color.Blue, 3f);

            Scene.Shapes.AddRange(new Shape[]
            {
                /*
                new Sphere(new Vector3(5, 0, -10), 5f, Color.Black),

                new Sphere(new Vector3(-4, -2, 0), .5f, Color.Black),
                new Sphere(new Vector3(-2, -2, 0), .5f, Color.Black),
                new Sphere(new Vector3(0, -2, 0), .5f, Color.Black),
                new Sphere(new Vector3(2, -2, 0), .5f, Color.Black),
                new Sphere(new Vector3(4, -2, 0), .5f, Color.Black),

                new Sphere(new Vector3(-4, 0, 0), .5f, Color.Red),
                new Sphere(new Vector3(-2, 0, 0), .5f, Color.Yellow),
                new Sphere(new Vector3(2, 0, 0), .5f, Color.Blue),
                new Sphere(new Vector3(4, 0, 0), .5f, Color.Red),

                new Sphere(new Vector3(-4, 2, 0), .5f, Color.Black),
                new Sphere(new Vector3(-2, 2, 0), .5f, Color.Black),
                new Sphere(new Vector3(0, 2, 0), .5f, Color.Black),
                new Sphere(new Vector3(2, 2, 0), .5f, Color.Black),
                new Sphere(new Vector3(4, 2, 0), .5f, Color.Black),
                */

                new Box(new Vector3(0, 1.9, 0), Color.White, new Vector3(1, 0.2, 1)).SetLight(lamp1), // Lamp
                new Box(new Vector3(1.5f, 1.9, 0), Color.White, new Vector3(1, 0.2, 1)).SetLight(lamp2), // Lamp
                new Box(new Vector3(-1.5f, 1.9, 0), Color.Blue, new Vector3(1, 0.2, 1)).SetLight(lamp3), // Lamp
                
                new Box(new Vector3(0, 2, 0), Color.White, new Vector3(4, 0.01, 4)), // Top
                new Box(new Vector3(0, -2, 0), Color.White, new Vector3(4, 0.01, 4)), // Floor
                new Box(new Vector3(2, 0, 0), Color.Green, new Vector3(0.01, 4, 4)), // Right wall
                new Box(new Vector3(-2, 0, 0), Color.Red, new Vector3(0.01, 4, 4)), // Left wall
                new Box(new Vector3(0, 0, -2), Color.White, new Vector3(4, 4, 0.01)), // Back wall

                new Sphere(new Vector3(0, -0.5, -1), .5f, Color.White)
                {
                    Material = mirror,
                },
                new Sphere(new Vector3(0, -1.5, 1), .5f, Color.White)
                {
                    Material = glass,
                },
                new Sphere(new Vector3(1, -1.5, 1), .5f, Color.Yellow),
                new Sphere(new Vector3(-1, -1.5, 1), .5f, Color.Blue),

                new Box(new Vector3(0, -1.5, -1), Color.Yellow),
                new Box(new Vector3(1, -1.5, -1), Color.Red),
                new Box(new Vector3(-1, -1.5, -1), Color.Blue),
            });

            Scene.UpdateLights();
        }

        protected override void RenderScreen(RenderContext context)
        {
            var width = context.Width;
            var height = context.Height;
            var scale = context.Scale;
            var dispatcher = context.Dispatcher;
            var camera = MainCamera;

            var samples = 4;
            var samplesWidth = width * samples;
            var samplesHeight = height * samples;

            float fovScale = (float)Math.Tan(MathHelpres.DegToRad(camera.FOV * 0.5));
            float aspectRatio = (float)width / height;

            Vector3 orig = camera.Position;

            float halfX = width / 2;
            float halfY = height / 2;

            Color[,] BatchPreview(int ix, int iy, int sizeX, int sizeY)
            {
                var tile = RenderBatch(ix, iy, sizeX, sizeY, (int)(8 * scale));
                return tile;
            }

            Color[,] Batch(int ix, int iy, int sizeX, int sizeY)
            {
                var tile = RenderBatch(ix, iy, sizeX, sizeY, 1);
                return tile;
            }

            Color[,] RenderBatch(int ix, int iy, int sizeX, int sizeY, int step)
            {
                var tile = new Color[sizeX, sizeY];

                for (int localY = 0; localY < sizeY; localY += step)
                {
                    int y = iy + localY;

                    for (int localX = 0; localX < sizeX; localX += step)
                    {
                        int x = ix + localX;
                        //
                        float posX = (2 * (x + 0.5f) / width - 1) * aspectRatio * fovScale;
                        float posY = (1 - 2 * (y + 0.5f) / height) * fovScale;
                        //
                        var dir = Normalize(new Vector3(posX, posY, -1));
                        var ray = new Ray(orig, dir);
                        //
                        var color = TracePath(context, camera, ray, Scene.BackgroundColor);
                        tile[localX, localY] = color;
                    }
                }

                if (step > 1)
                {
                    for (int localY = 0; localY < sizeY; localY += step)
                    {
                        for (int localX = 0; localX < sizeX; localX += step)
                        {
                            var template = tile[localX, localY];

                            for (var y = localY; y < localY + step && y < sizeY; y++)
                            {
                                for (var x = localX; x < localX + step && x < sizeX; x++)
                                {
                                    tile[x, y] = template;
                                }
                            }
                        }
                    }
                }

                return tile;
            }

            lock (camera)
            {
                lock (Scene)
                {
                    BatchScreen(context, BatchPreview);
                    BatchScreen(context, Batch);
                }
            }
        }

        private Color TracePath(RenderContext context, Camera camera, Ray ray, Color back, int depth = 0)
        {
            // Bounced enough times
            if (depth >= camera.MaxDepth)
            {
                return back;
            }

            var maxDistance = 10;
            var hit = FindClosestHit(ray, maxDistance, out var light);

            if (!hit.IsHitting)
            {
                return back; // Nothing was hit
            }

            var material = hit.HitObject.Material;
            var emittance = material.Color; //material.emittance;

            var position = hit.Position;
            var normal = hit.Normal;

            if (ShowDepth)
            {
                var dist = Distance(hit.Position, ray.Origin);
                var rate = 1 - (dist / maxDistance);
                rate = MathHelpres.Clamp(rate, 0, 1);
                rate *= rate;

                return new Color(rate, rate, rate);
            }

            if (ShowNormals)
            {
                var x = normal.x;
                var y = normal.y;
                var z = normal.z;

                if (x < 0) x *= -0.5;
                if (y < 0) y *= -0.5;
                if (z < 0) z *= -0.5;

                return new Color(x, y, z);
            }

            if (light != null)
            {
                return emittance;
            }

            emittance = Enlight(emittance, hit);

            if (material.Refraction > 0)
            {
                var newRayDirection = Refract(ray.Direction, normal, material.RefractionEta);
                var newRay = new Ray(position, newRayDirection);

                var incoming = TracePath(context, camera, newRay, back, depth + 1);

                var refractedColor = incoming; //emittance * incoming;

                emittance = Color.Lerp(emittance, refractedColor, material.Refraction);
            }

            if (material.Reflection > 0)
            {
                var newRayDirection = Reflect(ray.Direction, normal);
                var newRay = new Ray(position, newRayDirection);

                var BRDF = material.Specular / (float)Math.PI;

                var incoming = TracePath(context, camera, newRay, back, depth + 1);

                var reflectedColor = emittance + (BRDF * incoming);

                emittance = Color.Lerp(emittance, reflectedColor, material.Reflection);
            }

            return emittance;
        }

        private Color Enlight(Color emittance, Hit hit)
        {
            var ambientFactor = 1f;

            var color = new Color();

            foreach (var light in Scene.Lights)
            {
                color += LightIntensity(hit, light, Scene.AmbientColor, ambientFactor);
            }

            emittance *= color;

            return emittance;
        }

        private Color LightIntensity(Hit hit, Light light, Color ambientColor, float ambientFactor)
        {
            var lightPosition = light.Shape.Position;
            var lightDirection = lightPosition - hit.Position;

            var lightDistance = lightDirection.Length;
            var lightDistance2 = lightDistance * lightDistance;

            lightDirection *= 1f / lightDistance;

            var attenuation = (float)
            (
                (
                    lightDistance * light.QuadraticAttenuation +
                    lightDistance2 * light.LinearAttenuation +
                    light.ConstantAttenuation
                ) * 1 / light.Intensity
            );

            var NdotLD = (float)Dot(hit.Normal, lightDirection);

            if (NdotLD > 0)
            {
                if (!IsShadow(hit.HitObject, lightPosition, lightDirection, (float)lightDistance))
                {
                    var mul = (ambientColor * ambientFactor + light.Color * NdotLD) / attenuation;
                    return light.Shape.Material.Color * mul;
                }
            }

            return light.Shape.Material.Color * (ambientColor * ambientFactor / attenuation);
        }

        private bool IsShadow(Shape currentShape, Vector3 lightPosition, Vector3 lightDirection, float lightDistance)
        {
            var ray = new Ray(lightPosition, -lightDirection);

            foreach (var shape in Scene.Shapes.Where(x => x != currentShape && x.Light == null))
            {
                if (shape.GetIntersection(ray, lightDistance, out var _, out var _))
                {
                    return true;
                }
            }

            return false;
        }

        private Hit FindClosestHit(Ray ray, int maxDistance, out Light hitLight)
        {
            var closestHit = new Hit();
            hitLight = null;

            var minDist = double.PositiveInfinity;

            foreach (var shape in Scene.Shapes)
            {
                shape.GetIntersection(ray, maxDistance, out var hit, out var distance);

                if (!hit.IsHitting)
                {
                    continue;
                }

                if (distance < minDist)
                {
                    minDist = distance;
                    closestHit = hit;
                }
            }

            if (closestHit.IsHitting && closestHit.HitObject.Light != null)
            {
                hitLight = closestHit.HitObject.Light;
            }

            return closestHit;
        }

        public override void OnKeyPress(Key key, Action onRender)
        {
            var origPos = MainCamera.Position;

            if (key == Key.E) MainCamera.Position += Vector3.Up * 0.5f;
            if (key == Key.Q) MainCamera.Position += Vector3.Down * 0.5f;
            if (key == Key.A) MainCamera.Position += Vector3.Left * 0.5f;
            if (key == Key.D) MainCamera.Position += Vector3.Right * 0.5f;
            if (key == Key.W) MainCamera.Position += Vector3.Back * 0.5f;
            if (key == Key.S) MainCamera.Position += Vector3.Forward * 0.5f;

            if (origPos != MainCamera.Position)
            {
                onRender();
            }
        }
    }
}
