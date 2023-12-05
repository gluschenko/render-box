using System.Diagnostics;
using System.Windows.Input;
using RenderBox.Services.Rendering;
using RenderBox.Shared.Core;
using RenderBox.Shared.Modules.PathTracer;
using RenderBox.Shared.Modules.PathTracer.Scenes;
using RenderBox.Shared.Modules.PathTracer.Shapes;
using static RenderBox.Shared.Core.VectorMath;

namespace RenderBox.Services.Renderers
{
    public enum RenderMode
    {
        Light = 0,
        Normals = 1,
        Depth = 2,
        Time = 3,
    }

    public class PathTraceRenderer : Renderer
    {
        public Camera MainCamera { get; set; }
        public Scene Scene { get; set; }

        public RenderMode Mode { get; set; }

        private readonly Stopwatch _stopwatch;

        public PathTraceRenderer(Paint paint) : base(paint)
        {
            _stopwatch = new Stopwatch();

            MainCamera = new Camera(new Vector3(0, 0, 4));
            Scene = new CornellBox();
            //Scene = new BigRoom();
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

            var fovScale = (float)Math.Tan(MathHelpres.DegToRad(camera.FOV * 0.5));
            var aspectRatio = (float)width / height;

            var orig = camera.Position;

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

            int GetRenderPriority(int x, int y)
            {
                return (int)Distance(new Vector2(x, y), new Vector2(width / 2, height / 2));
            }

            Color[,] RenderBatch(int ix, int iy, int sizeX, int sizeY, int step)
            {
                var tile = new Color[sizeX, sizeY];

                for (var localY = 0; localY < sizeY; localY += step)
                {
                    var y = iy + localY;

                    for (var localX = 0; localX < sizeX; localX += step)
                    {
                        var x = ix + localX;
                        //
                        var posX = (2 * (x + 0.5f) / width - 1) * aspectRatio * fovScale;
                        var posY = (1 - 2 * (y + 0.5f) / height) * fovScale;
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
                    for (var localY = 0; localY < sizeY; localY += step)
                    {
                        for (var localX = 0; localX < sizeX; localX += step)
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
                    BatchScreen(context, BatchPreview, GetRenderPriority);
                    BatchScreen(context, Batch, GetRenderPriority);
                }
            }
        }

        private Color TracePath(RenderContext context, Camera camera, Ray ray, Color back, int depth = 0, Shape currentShape = null)
        {
            if (Mode == RenderMode.Time)
            {
                _stopwatch.Restart();
            }

            // Bounced enough times
            if (depth >= camera.MaxBounceDepth)
            {
                return back;
            }

            var maxDistance = camera.MaxDistance;
            var hit = FindClosestHit(ray, maxDistance, currentShape);

            if (!hit.IsHitting)
            {
                return back; // Nothing was hit
            }

            var material = hit.HitObject.Material;
            var emittance = material.Color; //material.emittance;

            var position = hit.Position;
            var normal = hit.Normal;

            if (Mode == RenderMode.Depth)
            {
                var dist = Distance(hit.Position, ray.Origin);
                var rate = 1 - (dist / maxDistance);
                rate = MathHelpres.Clamp(rate, 0, 1);
                rate *= rate;

                return new Color(rate, rate, rate);
            }

            if (Mode == RenderMode.Normals)
            {
                var x = normal.x;
                var y = normal.y;
                var z = normal.z;

                if (x < 0) x *= -0.5f;
                if (y < 0) y *= -0.5f;
                if (z < 0) z *= -0.5f;

                return new Color(x, y, z);
            }

            if (hit.HitObject.Light != null)
            {
                return emittance;
            }

            if (Scene.LightingEnabled)
            {
                emittance = Enlight(emittance, hit);
            }

            if (material.Refraction > 0)
            {
                var newRayDirection = Refract(ray.Direction, normal, material.RefractionEta);
                var newRay = new Ray(position, newRayDirection);

                var incoming = TracePath(context, camera, newRay, back, depth + 1, hit.HitObject);

                var refractedColor = incoming; //emittance * incoming;

                emittance = Color.Lerp(emittance, refractedColor, material.Refraction);
            }

            if (material.Reflection > 0)
            {
                var newRayDirection = Reflect(ray.Direction, normal);
                var newRay = new Ray(position, newRayDirection);

                var brdf = material.Specular / (float)Math.PI;

                var incoming = TracePath(context, camera, newRay, back, depth + 1, currentShape);

                var reflectedColor = !material.IsMetallic
                    ? emittance + (brdf * incoming)
                    : incoming;

                emittance = Color.Lerp(emittance, reflectedColor, material.Reflection);
            }

            if (Mode == RenderMode.Time)
            {
                var ticks = Math.Log(_stopwatch.ElapsedTicks) / 10.0;
                return new Color(ticks, ticks, ticks);
            }

            return emittance;
        }

        private Color Enlight(Color emittance, Hit hit)
        {
            var ambientFactor = 1f;

            if (Scene.AmbientOcclusion)
            {
                ambientFactor = CalcAmbientOcclusion(hit);
            }

            if (!Scene.SoftShadows)
            {
                var color = new Color();

                foreach (var light in Scene.Lights)
                {
                    color += LightIntensity(hit, light, light.Shape.Position, Scene.AmbientColor, ambientFactor);
                }

                emittance *= color;
            }
            else
            {
                var color = new Color();

                var posItems = new float[3];

                foreach (var light in Scene.Lights)
                {
                    for (var i = 0; i < Scene.GISamples; i++)
                    {
                        Vector3 random;

                        if (light.Shape is Box boxShape)
                        {
                            var side = Rand.Int(0, 6);
                            var c = side % 3;
                            posItems[c] = side % 2 == 0 ? 0.5f : -0.5f;
                            posItems[(c + 1) % 3] = Rand.Float() - 0.5f;
                            posItems[(c + 2) % 3] = Rand.Float() - 0.5f;

                            random = new Vector3(posItems[0], posItems[1], posItems[2]) * boxShape.Scale;
                        }
                        else
                        {
                            random = new Vector3(Rand.Float() - 0.5f, Rand.Float() - 0.5f, Rand.Float() - 0.5f);
                        }

                        var lightPosition = light.Shape.Position + light.Shape.GetLightEmission(random);

                        color += LightIntensity(hit, light, lightPosition, Scene.AmbientColor, ambientFactor);
                    }
                }

                emittance *= color / Scene.GISamples;
            }

            return emittance;
        }

        private Color LightIntensity(Hit hit, Light light, Vector3 lightPosition, Color ambientColor, float ambientFactor)
        {
            var lightColor = light.Color;
            var lightDirection = lightPosition - hit.Position;

            var lightDistance = lightDirection.Length;
            var lightDistance2 = lightDistance * lightDistance;

            lightDirection *= 1f / lightDistance;

            var a = lightDistance * light.QuadraticAttenuation;
            var b = lightDistance2 * light.LinearAttenuation;
            var c = light.ConstantAttenuation;
            var attenuation = (float)((a + b + c) * (1 / light.Intensity));

            var ndotLD = Dot(hit.Normal, lightDirection);

            if (ndotLD > 0)
            {
                if (!IsShadow(hit.HitObject, lightPosition, lightDirection, (float)lightDistance))
                {
                    var linghtnessMul = (ambientColor * ambientFactor + lightColor * ndotLD) / attenuation;
                    return lightColor * linghtnessMul;
                }
            }

            var refraction = hit.HitObject.Material.Refraction;

            if (refraction > 0)
            {
                lightColor *= hit.HitObject.Material.Color;
            }

            var ambientMul = (ambientColor * ambientFactor + lightColor * ndotLD * refraction) / attenuation;
            return lightColor * ambientMul;
        }

        private bool IsShadow(Shape currentShape, Vector3 lightPosition, Vector3 lightDirection, float lightDistance)
        {
            if (!Scene.ShadowsEnabled)
            {
                return false;
            }

            var ray = new Ray(lightPosition, -lightDirection);

            foreach (var shape in Scene.Shapes.Where(x => x != currentShape))
            {
                if (shape.GetIntersection(ray, lightDistance, out var _, out var _))
                {
                    return true;
                }
            }

            return false;
        }

        private float CalcAmbientOcclusion(Hit hit)
        {
            var factor = 0f;

            for (var i = 0; i < Scene.GISamples; i++)
            {
                var randomRay = new Vector3(Rand.Float() * 2 - 1, Rand.Float() * 2 - 1, Rand.Float() * 2 - 1);

                var ndotRR = (float)Dot(hit.Normal, randomRay);

                if (ndotRR < 0.0f)
                {
                    randomRay = -randomRay;
                    ndotRR = -ndotRR;
                }

                var ray = new Ray(hit.Position, randomRay);

                var dist = 1000f;

                foreach (var shape in Scene.Shapes.Where(x => x.Light == null))
                {
                    if (shape.GetIntersection(ray, dist, out var _, out var testDist))
                    {
                        dist = (float)testDist;
                    }
                }

                factor += ndotRR / (1.0f + dist * dist);
            }

            return 1f - (factor / Scene.GISamples) * 4f;
        }

        private Hit FindClosestHit(Ray ray, float maxDistance, Shape currentShape = null)
        {
            var closestHit = new Hit();

            var minDist = double.PositiveInfinity;

            foreach (var shape in Scene.Shapes)
            {
                if (shape == currentShape)
                {
                    continue;
                }

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
