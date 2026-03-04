using TerraForge.Domain;
using TerraForge.Settings;

namespace TerraForge.Generation;

public class MoistureMapGenerator(int seedOffset = 10000) : IWorldGenerationStep
{
    private readonly Perlin _perlin = new(seedOffset);

    public void Apply(World world, WorldGenerationSettings settings)
    {
        const double baseScale = 0.03;
        const int octaves = 4;
        const double persistence = 0.5;
        const double lacunarity = 2.0;

        for (var y = 0; y < world.Height; y++)
        {
            for (var x = 0; x < world.Width; x++)
            {
                var nx = x * baseScale;
                var ny = y * baseScale;

                var amplitude = 1.0;
                var frequency = 1.0;
                var moisture = 0.0;

                for (var o = 0; o < octaves; o++)
                {
                    var sample = _perlin.Noise(nx * frequency, ny * frequency);
                    moisture += sample * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                moisture = (moisture + 1) / 2.0;
                world.Cells[x, y].Moisture = moisture;
            }
        }
    }
}