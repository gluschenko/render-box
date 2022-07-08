using System.Collections.Generic;
using RenderBox.Core;
using RenderBox.Services.Rendering;
using RenderBox.Shared.Modules.Perlin;

namespace RenderBox.Services.Renderers
{
    public class TerrainRenderer : Renderer
    {
        public PerlinNoise Perlin { get; private set; }

        public TerrainRenderer(Paint paint) : base(paint)
        {
            Perlin = new PerlinNoise(Rand.Int(1, 100));
        }

        protected override void RenderScreen(RenderContext context)
        {
            var grassColor = Color.Green;

            var zoom = context.Width / 800f;
            var halfX = context.Width / 2f;
            var halfY = context.Height / 2f;

            Color[,] Batch(int ix, int iy, int sizeX, int sizeY)
            {
                var tile = new Color[sizeX, sizeY];

                for (var y = 0; y < sizeY; y++)
                {
                    var posY = (iy + y) * zoom;

                    for (var x = 0; x < sizeX; x++)
                    {
                        var posX = (ix + x) * zoom;

                        var neignbor = Perlin.FractalNoise2D(posX + 1, posY + 1, 4, 100, 1);

                        var n = Perlin.FractalNoise2D(posX, posY, 4, 100, 1);


                        ColorHelpers.ToHSV(grassColor, out var h, out var s, out var v);

                        var delta = (neignbor - n) * 16f;
                        v = MathHelpres.Clamp(delta / 2f + .5f, 0, 1);

                        var color = ColorHelpers.FromHSV(h, s, v);

                        tile[x, y] = color;
                    }
                }

                return tile;
            }

            BatchScreen(context, Batch);
        }

        public class World
        {
            public Dictionary<Offset, Chunk> Chunks { get; set; }

            public World()
            {
                Chunks = new Dictionary<Offset, Chunk>();
            }
        }

        public class Chunk
        {
            public const int Size = 64;
            public Block[,,] Blocks { get; set; }

            public Chunk()
            {
                Blocks = new Block[Size, Size, Size];
            }
        }

        public struct Offset
        {
            public short X { get; set; }
            public short Y { get; set; }
            public short Z { get; set; }

            public Offset(short x, short y, short z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }

        public struct Block
        {
            public const int BlocksPerMeter = 8;
            public const float Size = 1 / (float)BlocksPerMeter;

            public BlockType Type { get; set; }

            public Block(BlockType type)
            {
                Type = type;
            }
        }

        public enum BlockType : byte
        {
            Air = 0,
            Stone = 1,
            Grass = 2,
            Dirt = 3,
            Water = 4,
        }
    }
}
