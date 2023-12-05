using RenderBox.Shared.Core;
using RenderBox.Shared.Modules.PathTracer.Shapes;

namespace RenderBox.Shared.Modules.PathTracer.Scenes
{
    public class CornellBox : Scene
    {
        public CornellBox()
        {
            var glass = new Material
            {
                Refraction = 0.6f,
                Reflection = 0.6f,
            };

            var mirror = new Material
            {
                Refraction = 0f,
                Reflection = 1f,
            };

            var metal = new Material
            {
                Refraction = 0f,
                Reflection = 1f,
                IsMetallic = true,
            };

            Shapes.AddRange(new Shape[]
            {
                new Box(new Vector3(1.5f, 1.9, 0), Color.Blue, new Vector3(1, 0.2, 1)).SetLight(new Light(Color.White, 3f)), // Lamp
                new Box(new Vector3(0, 1.9, 0), Color.White, new Vector3(1, 0.2, 1)).SetLight(new Light(Color.White, 3f)), // Lamp
                new Box(new Vector3(-1.5f, 1.9, 0), Color.Yellow, new Vector3(1, 0.2, 1)).SetLight(new Light(Color.White, 3f)), // Lamp

                new Box(new Vector3(0, 2, 0), Color.White, new Vector3(4, 0.01, 4)), // Top
                new Box(new Vector3(0, -2, 0), Color.White, new Vector3(4, 0.01, 4)), // Floor
                new Box(new Vector3(2, 0, 0), Color.Green, new Vector3(0.01, 4, 4)), // Right wall
                new Box(new Vector3(-2, 0, 0), Color.Red, new Vector3(0.01, 4, 4)), // Left wall
                new Box(new Vector3(0, 0, -2), Color.White, new Vector3(4, 4, 0.01)), // Back wall

                new Sphere(new Vector3(-1, -0.5, -1), .5f, Color.White) { Material = metal },
                new Sphere(new Vector3(0, -0.5, -1), .5f, Color.White) { Material = mirror },
                new Sphere(new Vector3(1, -0.5, -1), .5f, Color.White).SetLight(new Light(Color.White, 1f)),

                new Sphere(new Vector3(0, -1.5, 1), .5f, Color.White) { Material = glass },
                new Sphere(new Vector3(1, -1.5, 1), .5f, Color.Yellow),
                new Sphere(new Vector3(-1, -1.5, 1), .5f, Color.Blue),

                new Sphere(new Vector3(1.2, -1.9, 1.4), .05f, Color.White).SetLight(new Light(Color.White, 0.5f)),

                new Sphere(new Vector3(1.8, 0, 0), .2f, Color.White),

                new Box(new Vector3(0, -1.5, -1), Color.Yellow),
                new Box(new Vector3(1, -1.5, -1), Color.Red),
                new Box(new Vector3(-1, -1.5, -1), Color.Blue),

                new Box(new Vector3(-1.6, -1.9, 1.4), Color.White, new Vector3(0.24f, 0.24f, 0.24f)) { Material = metal },
            });

            UpdateLights();
        }
    }
}
