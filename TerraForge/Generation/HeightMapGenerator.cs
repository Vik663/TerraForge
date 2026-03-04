using TerraForge.Domain;
using TerraForge.Settings;

namespace TerraForge.Generation;

public class HeightMapGenerator : IWorldGenerationStep
{
    public void Apply(World world, WorldGenerationSettings settings)
    {
        var rnd = new Random(settings.Seed);

        for (var y = 0; y < world.Height; y++)
        {
            for (var x = 0; x < world.Width; x++)
            {
                world.Cells[x, y].Height = rnd.NextDouble();
            }
        }
    }
}