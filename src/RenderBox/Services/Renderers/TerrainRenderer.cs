using System.Windows.Threading;
using RenderBox.Services.Rendering;
using RenderBox.Shared.Core;
using RenderBox.Shared.Modules.Perlin;

namespace RenderBox.Services.Renderers
{
    public class TerrainRenderer : Renderer
    {
        public int WorldSeed { get; private set; } = 123456;
        public WorldGenerator WorldGenerator { get; private set; }
        public World World { get; private set; }

        public TerrainRenderer(Paint paint) : base(paint)
        {
            WorldGenerator = new WorldGenerator(WorldSeed);
            World = new World(WorldGenerator);

            OnRenderComplete += TerrainRenderer_OnRenderComplete;
        }

        private void TerrainRenderer_OnRenderComplete()
        {

        }

        protected override void RenderScreen(RenderContext context)
        {
            var dispatcher = context.Dispatcher;

            var grassColor = Color.Green;
            var waterColor = Color.Blue;
            var showColor = Color.White;

            var width = context.Width;
            var height = context.Height;

            var zoom = 1f;
            var halfX = width / 2f;
            var halfY = height / 2f;

            int GetRenderPriority(int x, int y)
            {
                return (int)VectorMath.Distance(new Vector2(x, y), new Vector2(width / 2, height / 2));
            }

            Color[,] Batch(int ix, int iy, int sizeX, int sizeY)
            {
                var heightMap = World.GetHeightMap();
                var worldSizeX = heightMap.GetLength(0);
                var worldSizeZ = heightMap.GetLength(1);
                var worldHalfSizeX = worldSizeX / 2;
                var worldHalfSizeZ = worldSizeZ / 2;

                var tile = new Color[sizeX, sizeY];

                bool IsValidWorldPos(int worldPosX, int worldPosZ)
                {
                    var isValidPos =
                        worldPosX >= -worldHalfSizeX &&
                        worldPosX < worldHalfSizeX &&
                        worldPosZ >= -worldHalfSizeZ &&
                        worldPosZ < worldHalfSizeZ;

                    return isValidPos;
                }

                float GetHeight(int worldPosX, int worldPosZ)
                {
                    return heightMap[worldPosX + worldHalfSizeX, worldPosZ + worldHalfSizeZ];
                }

                for (var y = 0; y < sizeY; y++)
                {
                    for (var x = 0; x < sizeX; x++)
                    {
                        var screenX = ix + x;
                        var screenY = iy + y;

                        var worldPosX = (int)MathF.Round((screenX - halfX) * zoom);
                        var worldPosZ = (int)MathF.Round((screenY - halfY) * zoom);

                        var heightRate = IsValidWorldPos(worldPosX, worldPosZ)
                            ? GetHeight(worldPosX, worldPosZ)
                            : WorldGenerator.GetHeight(worldPosX, worldPosZ);

                        var blockHeight = (int)MathF.Floor(heightRate * World.WorldHeight);

                        worldPosX = (int)MathF.Round((screenX - halfX) * zoom);
                        worldPosZ = (int)MathF.Round((screenY - halfY) * zoom);

                        if (!IsValidWorldPos(worldPosX, worldPosZ))
                        {
                            tile[x, y] = Color.Gray;
                            continue;
                        }

                        heightRate = GetHeight(worldPosX, worldPosZ);

                        var worldPosNeighborX = worldPosX + 3;
                        var worldPosNeighborZ = worldPosZ + 3;

                        var heightRateNeighbor = IsValidWorldPos(worldPosNeighborX, worldPosNeighborZ)
                            ? GetHeight(worldPosNeighborX, worldPosNeighborZ)
                            : WorldGenerator.GetHeight(worldPosNeighborX, worldPosNeighborZ);

                        var color = heightRate > 0.5f ? grassColor : waterColor;

                        if (heightRate > 0.75f)
                        {
                            color = showColor;
                        }

                        ColorHelpers.ToHSV(color, out var h, out var s, out var v);

                        var delta = (heightRateNeighbor - heightRate) * 64f;
                        v = MathHelpres.Clamp(delta / 2f + .5f, 0, 1);

                        color = ColorHelpers.FromHSV(h, s, v);

                        if (Math.Abs(worldPosX % Chunk.Size) == 0 || Math.Abs(worldPosZ % Chunk.Size) == 0)
                        {
                            ColorHelpers.ToHSV(color, out var hh, out var ss, out var vv);

                            color = ColorHelpers.FromHSV(hh, ss, vv * 0.8);
                        }

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
            //return 0.6f + MathF.Sin(x * 0.02f) * MathF.Cos(z * 0.02f) * 0.2f;

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
        public const int WorldHeight = 256;

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
        public const int Size = 32;
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
