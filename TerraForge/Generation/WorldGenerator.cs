using TerraForge.Domain;
using TerraForge.Settings;

namespace TerraForge.Generation;

public class WorldGenerator(IEnumerable<IWorldGenerationStep> steps)
{
    private readonly IReadOnlyList<IWorldGenerationStep> _steps = steps.ToList();

    public World Generate(WorldGenerationSettings settings)
    {
        var world = new World(settings.Width, settings.Height);

        foreach (var step in _steps)
        {
            step.Apply(world, settings);
        }

        return world;
    }
}