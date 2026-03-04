using TerraForge.Domain;
using TerraForge.Settings;

namespace TerraForge.Generation;

public class HeightNormalizer : IWorldGenerationStep
{
    public void Apply(World world, WorldGenerationSettings settings)
    {
        var min = double.MaxValue;
        var max = double.MinValue;

        for (var y = 0; y < world.Height; y++)
        for (var x = 0; x < world.Width; x++)
        {
            var h = world.Cells[x, y].Height;
            if (h < min) min = h;
            if (h > max) max = h;
        }

        var range = max - min;

        for (var y = 0; y < world.Height; y++)
        for (var x = 0; x < world.Width; x++)
        {
            world.Cells[x, y].Height = (world.Cells[x, y].Height - min) / range;
        }
    }
}