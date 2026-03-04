using TerraForge.Domain;
using TerraForge.Settings;

namespace TerraForge.Generation;

public class ContinentShaper : IWorldGenerationStep
{
    public void Apply(World world, WorldGenerationSettings settings)
    {
        var cx = world.Width / 2.0;
        var cy = world.Height / 2.0;

        var maxDist = Math.Sqrt(cx * cx + cy * cy);

        for (var y = 0; y < world.Height; y++)
        for (var x = 0; x < world.Width; x++)
        {
            var dx = x - cx;
            var dy = y - cy;
            var dist = Math.Sqrt(dx * dx + dy * dy);

            var factor = 1.0 - dist / maxDist;
            factor = Math.Pow(factor, 0.6);

            world.Cells[x, y].Height *= factor;
        }
    }
}