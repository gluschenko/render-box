using System;
using System.Collections.Generic;
using System.Windows.Threading;
using RenderBox.Core;
using RenderBox.Services.Rendering;
using RenderBox.Shared.Modules.Perlin;

namespace RenderBox.Services.Renderers
{
    public class TerrainRenderer : Renderer
    {
        public int WorldSeed { get; private set; } = 123456;
        public WorldGenerator WorldGenerator { get; private set; }
        public World World { get; private set; }

        private Dispatcher _dispatcher;

        public TerrainRenderer(Paint paint) : base(paint)
        {
            WorldGenerator = new WorldGenerator(WorldSeed);
            World = new World(WorldGenerator);

            OnRenderComplete += TerrainRenderer_OnRenderComplete;
        }

        private void TerrainRenderer_OnRenderComplete()
        {
            //Render(_dispatcher);
        }

        protected override void RenderScreen(RenderContext context)
        {
            _dispatcher = context.Dispatcher;

            var grassColor = Color.Green;
            var waterColor = Color.Blue;
            var showColor = Color.White;

            var width = context.Width;
            var height = context.Height;

            var zoom = 2.0f;
            var halfX = width / 2f;
            var halfY = height / 2f;

            int GetRenderPriority(int x, int y)
            {
                return (int)VectorMath.Distance(new Vector2(x, y), new Vector2(width / 2, height / 2));
            }

            Color[,] Batch(int ix, int iy, int sizeX, int sizeY)
            {
                var heightMap = World.GetHeightMap();
                var heightMapSizeX = heightMap.GetLength(0);
                var heightMapSizeZ = heightMap.GetLength(1);

                var tile = new Color[sizeX, sizeY];

                for (var y = 0; y < sizeY; y++)
                {
                    var screenY = iy + y;
                    var worldPosZ = (int)Math.Round((screenY - halfY) * zoom + (heightMapSizeZ / 2));

                    for (var x = 0; x < sizeX; x++)
                    {
                        var screenX = ix + x;
                        var worldPosX = (int)Math.Round((screenX - halfX) * zoom + (heightMapSizeX / 2));

                        var isValidPos = 
                            worldPosX >= 0 && 
                            worldPosX < heightMapSizeX && 
                            worldPosZ >= 0 && 
                            worldPosZ < heightMapSizeZ;

                        if (!isValidPos)
                        {
                            tile[x, y] = Color.Gray;
                            continue;
                        }

                        var heightRate = heightMap[worldPosX, worldPosZ];
                        var heightRateNeighbor = (worldPosX < heightMapSizeX - 1 && worldPosZ < heightMapSizeZ - 1)
                            ? heightMap[worldPosX + 1, worldPosZ + 1]
                            : heightRate;

                        var color = heightRate > 0.5f ? grassColor : waterColor;

                        if (heightRate > 0.75f)
                        {
                            color = showColor;
                        }

                        ColorHelpers.ToHSV(color, out var h, out var s, out var v);

                        var delta = (heightRateNeighbor - heightRate) * 86f;
                        v = MathHelpres.Clamp(delta / 2f + .5f, 0, 1);

                        color = ColorHelpers.FromHSV(h, s, v);

                        /*if (Math.Abs(Math.Round(posX)) % Chunk.Size == 0 || Math.Abs(Math.Round(posY)) % Chunk.Size == 0) 
                        {
                            color = Color.Yellow;
                        }*/

                        tile[x, y] = color;
                    }
                }

                return tile;
            }

            BatchScreen(context, Batch, GetRenderPriority);
        }
    }

    public class WorldGenerator
    {
        private readonly PerlinNoise _perlinNoise;

        public WorldGenerator(int seed)
        {
            _perlinNoise = new PerlinNoise(seed);
        }

        public float GetHeight(float x, float z)
        {
            var rate1 = _perlinNoise.FractalNoise2D(x, z, 16, 2000, 1);
            var rate2 = _perlinNoise.FractalNoise2D(x, z, 12, 400, 1);
            var rate3 = _perlinNoise.FractalNoise2D(x, z, 8, 800, 1);

            var rock = 1f - Math.Abs(rate1 * rate1 * rate1);
            var mountains = Math.Clamp(rate2 * rate3, 0f, 1f);
            var hills = Math.Abs(rate3);

            var result = 0.4f + (rock * 0.1f) + (hills * 0.05f) + (mountains * 0.45f);
            return result;
        }

        public int GetBlockHeight(int x, int z)
        {
            var rate = GetHeight(x, z);
            return (int)MathF.Floor(rate * World.WorldHeight);
        }

        public float[,] GetHeightMap()
        {
            var result = new float[World.WorldWidth, World.WorldWidth];

            var halfSizeX = result.GetLength(0) / 2;
            var halfSizeZ = result.GetLength(1) / 2;

            for (var x = 0; x < result.GetLength(0); x++)
            {
                for (var z = 0; z < result.GetLength(1); z++)
                {
                    result[x, z] = GetHeight(x - halfSizeX, z - halfSizeZ);
                }
            }

            return result;
        }
    }

    public class World
    {
        public const int WorldSize = 32;
        public const int WorldWidth = WorldSize * Chunk.Size;
        public const int WorldHeight = 1024;

        private readonly WorldGenerator _worldGenerator;
        private readonly float[,] _heightMap;

        public Dictionary<Offset, Chunk> Chunks { get; set; }

        public World(WorldGenerator worldGenerator)
        {
            _worldGenerator = worldGenerator;
            _heightMap = _worldGenerator.GetHeightMap();

            Chunks = new Dictionary<Offset, Chunk>();
        }

        public float[,] GetHeightMap()
        {
            return _heightMap;
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
