using TerraForge.Domain;
using TerraForge.Settings;

namespace TerraForge.Generation;

public class HeightContrastBoost(double power = 0.7) : IWorldGenerationStep
{
    public void Apply(World world, WorldGenerationSettings settings)
    {
        for (var y = 0; y < world.Height; y++)
        {
            for (var x = 0; x < world.Width; x++)
            {
                var h = world.Cells[x, y].Height;
                world.Cells[x, y].Height = Math.Pow(h, power);
            }
        }
    }
}