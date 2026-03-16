using TerraForge.Domain;
using TerraForge.Settings;

namespace TerraForge.Generation;

public class ClimateBiomeAssigner : IWorldGenerationStep
{
    public void Apply(World world, WorldGenerationSettings settings)
    {
        for (var y = 0; y < world.Height; y++)
        for (var x = 0; x < world.Width; x++)
        {
            var cell = world.Cells[x, y];
            var h = cell.Height;
            var m = cell.Moisture;
            var t = cell.Temperature;
            
            switch (t)
            {
                case < 0.15:
                    cell.Biome = Biome.Snow;
                    continue;
                case < 0.30:
                    cell.Biome = Biome.Taiga;
                    continue;
                case > 0.85 when h < 0.6:
                    cell.Biome = Biome.Desert;
                    continue;
            }

            switch (h)
            {
                case < 0.3:
                    cell.Biome = Biome.Ocean;
                    continue;
                case < 0.35:
                    cell.Biome = Biome.Beach;
                    continue;
                
                case < 0.6 when t < 0.4:
                    cell.Biome = Biome.Taiga;
                    continue;
                case < 0.6:
                    cell.Biome = m switch
                    {
                        < 0.3 => Biome.Desert,
                        < 0.6 => Biome.Savanna,
                        _ => Biome.Forest
                    };
                    continue;
                case < 0.8:
                    cell.Biome = t < 0.4 ? Biome.Taiga : Biome.Hills;
                    continue;
                default:
                    cell.Biome = Biome.Mountains;
                    break;
            }
        }
    }
}