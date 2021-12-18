using RenderBox.Core;
using RenderBox.Shared.Modules.PathTracer.Shapes;

namespace RenderBox.Shared.Modules.PathTracer.Scenes
{
    public class BigRoom : Scene
    {
        public BigRoom()
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
                new Box(new Vector3(0, 0, 0), Color.White, new Vector3(0.5, 0.5, 0.5)).SetLight(new Light(Color.White, 3f)), // Lamp

                //    A
                //  ______
                //  |    | B  C
                //  |    |______
                //F |          |
                //  |          |  D
                //  |__________|
                //      E

                new Box(new Vector3(2, 2, 2), Color.White, new Vector3(8, 0.01, 8)), // Top
                new Box(new Vector3(2, -2, 2), Color.White, new Vector3(8, 0.01, 8)), // Floor

                new Box(new Vector3(0, 0, -2), Color.White, new Vector3(4, 4, 0.01)), // A
                new Box(new Vector3(2, 0, 0), Color.White, new Vector3(0.01, 4, 4)), // B
                new Box(new Vector3(4, 0, 2), Color.White, new Vector3(4, 4, 0.01)), // C
                new Box(new Vector3(6, 0, 4), Color.White, new Vector3(0.01, 4, 4)), // D
                new Box(new Vector3(-2, 0, 2), Color.White, new Vector3(0.01, 4, 8)), // F
                
            });

            UpdateLights();
        }
    }
}
