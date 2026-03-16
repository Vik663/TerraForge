using TerraForge.Domain;
using TerraForge.Settings;

namespace TerraForge.Generation;

public class TemperatureMapGenerator(int seedOffset = 20000) : IWorldGenerationStep
{
    private readonly Perlin _perlin = new(seedOffset);

    public void Apply(World world, WorldGenerationSettings settings)
    {
        for (var y = 0; y < world.Height; y++)
        {
            var lat = world.Height > 1
                ? (double)y / (world.Height - 1)
                : 0.5;


            for (var x = 0; x < world.Width; x++)
            {
                var noise = _perlin.Noise(x * 0.02, y * 0.02) * 0.03;
                
                var height = world.Cells[x, y].Height;
                
                var altitudePenalty = height * 0.4;

                var finalTemp = Math.Clamp(lat + noise - altitudePenalty, 0.0, 1.0);

                world.Cells[x, y].Temperature = finalTemp;
            }
        }
    }
}