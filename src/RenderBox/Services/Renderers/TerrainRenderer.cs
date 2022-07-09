using System;
using System.Collections.Generic;
using RenderBox.Core;
using RenderBox.Services.Rendering;
using RenderBox.Shared.Modules.Perlin;

namespace RenderBox.Services.Renderers
{
    public class TerrainRenderer : Renderer
    {
        public int WorldSeed { get; private set; } = 24567;
        public WorldGenerator WorldGenerator { get; private set; }
        public World World { get; private set; }

        public TerrainRenderer(Paint paint) : base(paint)
        {
            WorldGenerator = new WorldGenerator(WorldSeed, 4);
        }

        protected override void RenderScreen(RenderContext context)
        {
            var grassColor = Color.Green;
            var waterColor = Color.Blue;

            var zoom = 0.75f;
            var halfX = context.Width / 2f;
            var halfY = context.Height / 2f;

            Color[,] Batch(int ix, int iy, int sizeX, int sizeY)
            {
                var tile = new Color[sizeX, sizeY];

                for (var y = 0; y < sizeY; y++)
                {
                    var posY = (iy + y - halfY) * zoom;

                    for (var x = 0; x < sizeX; x++)
                    {
                        var posX = (ix + x - halfX) * zoom;

                        var neignbor = WorldGenerator.GetHeight(posX + 1, posY + 1);

                        var n = WorldGenerator.GetHeight(posX, posY);

                        var color = n > 0.5f ? grassColor : waterColor;

                        ColorHelpers.ToHSV(color, out var h, out var s, out var v);

                        var delta = (neignbor - n) * 24f;
                        v = MathHelpres.Clamp(delta / 2f + .5f, 0, 1);

                        color = ColorHelpers.FromHSV(h, s, v);

                        if (Math.Abs(Math.Round(posX)) == 128 || Math.Abs(Math.Round(posY)) == 128) 
                        {
                            color = Color.Yellow;
                        }

                        tile[x, y] = color;
                    }
                }

                return tile;
            }

            BatchScreen(context, Batch);
        }
    }

    public class WorldGenerator
    {
        private int _size;
        private PerlinNoise _perlinNoise;

        public WorldGenerator(int seed, int size)
        {
            _perlinNoise = new PerlinNoise(seed);
            _size = size;
        }

        public float GetHeight(float x, float z)
        {
            var rate1 = _perlinNoise.FractalNoise2D(x, z, 10, 250, 1);
            var rate2 = _perlinNoise.FractalNoise2D(x, z, 7, 50, 1);
            var rate3 = _perlinNoise.FractalNoise2D(x, z, 6, 100, 1);

            var rock = 1f - Math.Abs(rate1 * rate1 * rate1);
            var mountains = Math.Clamp(rate2 * rate3, 0f, 1f);
            var hills = Math.Abs(rate3);

            var result = 0.4f + (rock * 0.1f) + (mountains * 0.5f) + (hills * 0.1f);
            return result;
        }
    }

    public class World
    {
        public const int WorldHeight = 1024; 

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

    public record struct Offset(short X, short Y, short Z);

    public record struct Block(BlockType Type);

    public enum BlockType : byte
    {
        Air = 0,
        Stone = 1,
        Grass = 2,
        Dirt = 3,
        Water = 4,
    }
}
