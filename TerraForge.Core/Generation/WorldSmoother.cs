using TerraForge.Domain;
using TerraForge.Settings;

namespace TerraForge.Generation;

public class WorldSmoother(int iterations = 3) : IWorldGenerationStep
{
    public void Apply(World world, WorldGenerationSettings settings)
    {
        for (var iter = 0; iter < iterations; iter++)
        {
            var newHeights = new double[world.Width, world.Height];

            for (var y = 0; y < world.Height; y++)
            for (var x = 0; x < world.Width; x++)
            {
                double sum = 0;
                var count = 0;

                for (var dy = -1; dy <= 1; dy++)
                for (var dx = -1; dx <= 1; dx++)
                {
                    var nx = x + dx;
                    var ny = y + dy;

                    if (nx < 0 || ny < 0 || nx >= world.Width || ny >= world.Height)
                        continue;

                    sum += world.Cells[nx, ny].Height;
                    count++;
                }

                newHeights[x, y] = sum / count;
            }

            for (var y = 0; y < world.Height; y++)
            for (var x = 0; x < world.Width; x++)
                world.Cells[x, y].Height = newHeights[x, y];
        }
    }
}