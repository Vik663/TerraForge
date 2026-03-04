using TerraForge.Domain;
using TerraForge.Settings;

namespace TerraForge.Generation;

public class ClimateBiomeAssigner : IWorldGenerationStep
{
    public void Apply(World world, WorldGenerationSettings settings)
    {
        for (var y = 0; y < world.Height; y++)
        {
            for (var x = 0; x < world.Width; x++)
            {
                var cell = world.Cells[x, y];
                var h = cell.Height;
                var m = cell.Moisture;

                if (h < 0.3)
                {
                    cell.Biome = Biome.Ocean;
                    continue;
                }

                if (h < 0.35)
                {
                    cell.Biome = Biome.Beach;
                    continue;
                }

                if (h < 0.6)
                {
                    cell.Biome = m switch
                    {
                        < 0.3 => Biome.Desert,
                        < 0.6 => Biome.Savanna,
                        _ => Biome.Forest
                    };
                    continue;
                }

                if (h < 0.8)
                {
                    cell.Biome = m < 0.4 ? Biome.Hills : Biome.Taiga;
                    continue;
                }

                cell.Biome = Biome.Mountains;
            }
        }
    }
}