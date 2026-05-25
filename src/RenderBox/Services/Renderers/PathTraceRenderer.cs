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
        private const float RayBias = 0.001f;
        private static readonly Color Black = new(0, 0, 0);
        private static readonly Color White = Color.White;

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
                        var dir = Normalize(camera.TransformDirection(new Vector3(posX, posY, -1)));
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

        private Color TracePath(RenderContext context, Camera camera, Ray ray, Color back, int depth = 0, Shape? currentShape = null)
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

            var hitObject = hit.HitObject ?? throw new InvalidOperationException("A hit must include the shape it intersected.");
            var material = hitObject.Material;
            var emittance = material.Color; //material.emittance;

            var position = hit.Position;
            var normal = hit.FromInside ? -hit.Normal : hit.Normal;

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

            if (hitObject.Light != null)
            {
                return emittance;
            }

            if (Scene.LightingEnabled)
            {
                emittance = Enlight(emittance, hit);
            }

            if (material.Refraction > 0)
            {
                var incoming = TraceRefraction(context, camera, ray, hit, back, depth);

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
                    var lightShape = light.Shape ?? throw new InvalidOperationException("Scene lights must be attached to a shape.");
                    color += LightIntensity(hit, light, lightShape.Position, Scene.AmbientColor, ambientFactor);
                }

                emittance *= color;
            }
            else
            {
                var color = new Color();

                var posItems = new float[3];

                foreach (var light in Scene.Lights)
                {
                    var lightShape = light.Shape ?? throw new InvalidOperationException("Scene lights must be attached to a shape.");

                    for (var i = 0; i < Scene.GISamples; i++)
                    {
                        Vector3 random;

                        if (lightShape is Box boxShape)
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

                        var lightPosition = lightShape.Position + lightShape.GetLightEmission(random);

                        color += LightIntensity(hit, light, lightPosition, Scene.AmbientColor, ambientFactor);
                    }
                }

                emittance *= color / Scene.GISamples;
            }

            return emittance;
        }

        private Color LightIntensity(Hit hit, Light light, Vector3 lightPosition, Color ambientColor, float ambientFactor)
        {
            var hitObject = hit.HitObject ?? throw new InvalidOperationException("A hit must include the shape it intersected.");
            var lightColor = light.Color;
            var directLightDirection = lightPosition - hit.Position;

            var lightDistance = directLightDirection.Length;
            var lightDistance2 = lightDistance * lightDistance;

            directLightDirection *= 1f / lightDistance;

            var a = lightDistance * light.QuadraticAttenuation;
            var b = lightDistance2 * light.LinearAttenuation;
            var c = light.ConstantAttenuation;
            var attenuation = (float)((a + b + c) * (1 / light.Intensity));

            var transmission = TraceLightTransmission(hitObject, hit.Position, light, lightPosition, (float)lightDistance);
            var ndotLD = Dot(hit.Normal, directLightDirection);

            if (ndotLD > 0)
            {
                if (transmission != Black)
                {
                    var linghtnessMul = (ambientColor * ambientFactor + lightColor * ndotLD) / attenuation;
                    return transmission * lightColor * linghtnessMul;
                }
            }

            var ambientMul = ambientColor * ambientFactor / attenuation;
            return lightColor * ambientMul;
        }

        private Color TraceRefraction(RenderContext context, Camera camera, Ray ray, Hit entryHit, Color back, int depth)
        {
            var shape = entryHit.HitObject ?? throw new InvalidOperationException("A hit must include the shape it intersected.");
            var material = shape.Material;
            var ior = GetRefractionIor(material);
            var normal = entryHit.FromInside ? -entryHit.Normal : entryHit.Normal;
            var fromIor = entryHit.FromInside ? ior : 1;
            var toIor = entryHit.FromInside ? 1 : ior;

            if (!TryRefract(ray.Direction, normal, fromIor, toIor, out var refractedDirection))
            {
                var reflectedDirection = Reflect(ray.Direction, normal);
                return TracePath(context, camera, new Ray(Offset(entryHit.Position, reflectedDirection), reflectedDirection), back, depth + 1);
            }

            var refractedRay = new Ray(Offset(entryHit.Position, refractedDirection), refractedDirection);
            var incoming = TracePath(context, camera, refractedRay, back, depth + 1);
            return Color.Lerp(incoming * material.Color, incoming, 1 - material.Refraction);
        }

        private Color TraceLightTransmission(Shape targetShape, Vector3 targetPosition, Light light, Vector3 lightPosition, float lightDistance)
        {
            if (!Scene.ShadowsEnabled)
            {
                return White;
            }

            var lightShape = light.Shape ?? throw new InvalidOperationException("Scene lights must be attached to a shape.");
            var direction = Normalize(targetPosition - lightPosition);
            var ray = new Ray(Offset(lightPosition, direction), direction);
            var throughput = White;

            for (var depth = 0; depth < MainCamera.MaxBounceDepth + 2; depth++)
            {
                var hit = FindClosestHit(ray, lightDistance, lightShape, targetShape);

                if (!hit.IsHitting)
                {
                    return throughput;
                }

                var shape = hit.HitObject ?? throw new InvalidOperationException("A hit must include the shape it intersected.");
                var material = shape.Material;
                if (material.Refraction <= 0)
                {
                    return Black;
                }

                throughput *= Color.Lerp(White, material.Color, material.Refraction);

                // Keep direct lighting stable: refractive objects tint/attenuate shadow rays,
                // while visible lensing is handled by TraceRefraction on camera paths.
                ray = new Ray(Offset(hit.Position, ray.Direction), ray.Direction);
            }

            return Black;
        }

        private Hit FindClosestHit(Ray ray, float maxDistance, Shape? ignoredShape1, Shape? ignoredShape2)
        {
            var closestHit = new Hit();
            var minDist = double.PositiveInfinity;

            foreach (var shape in Scene.Shapes)
            {
                if (shape == ignoredShape1 || shape == ignoredShape2)
                {
                    continue;
                }

                shape.GetIntersection(ray, maxDistance, out var hit, out var distance);

                if (!hit.IsHitting || distance <= RayBias)
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

        private static bool TryRefract(Vector3 incident, Vector3 normal, float fromIor, float toIor, out Vector3 refracted)
        {
            var i = Normalize(incident);
            var n = Normalize(normal);
            var cosi = MathHelpres.Clamp(Dot(i, n), -1, 1);

            if (cosi > 0)
            {
                n = -n;
            }
            else
            {
                cosi = -cosi;
            }

            var eta = fromIor / toIor;
            var k = 1 - eta * eta * (1 - cosi * cosi);
            if (k < 0)
            {
                refracted = Vector3.Zero;
                return false;
            }

            refracted = Normalize(eta * i + (eta * cosi - MathHelpres.FastSqrt(k)) * n);
            return true;
        }

        private static float GetRefractionIor(Material material, float spectralOffset = 0)
        {
            var ior = material.RefractionEta <= 0
                ? 1 - material.RefractionEta
                : material.RefractionEta;

            return Math.Max(1.01f, ior + material.ChromaticAberration * spectralOffset);
        }

        private static Vector3 Offset(Vector3 position, Vector3 direction)
        {
            return position + Normalize(direction) * RayBias;
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

                foreach (var shape in Scene.Shapes.Where(x => x.Light == null && x.Material.Refraction <= 0))
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

        private Hit FindClosestHit(Ray ray, float maxDistance, Shape? currentShape = null)
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

            var origRotation = MainCamera.Rotation;
            var rotationStep = (float)MathHelpres.DegToRad(5);

            if (key == Key.Left) MainCamera.Rotation += Vector3.Up * rotationStep;
            if (key == Key.Right) MainCamera.Rotation += Vector3.Down * rotationStep;
            if (key == Key.Up) MainCamera.Rotation += Vector3.Left * rotationStep;
            if (key == Key.Down) MainCamera.Rotation += Vector3.Right * rotationStep;
            if (key == Key.Z) MainCamera.Rotation += Vector3.Forward * rotationStep;
            if (key == Key.X) MainCamera.Rotation += Vector3.Back * rotationStep;

            if (origPos != MainCamera.Position || origRotation != MainCamera.Rotation)
            {
                onRender();
            }
        }
    }
}
