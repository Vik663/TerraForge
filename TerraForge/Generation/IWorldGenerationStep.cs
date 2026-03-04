using TerraForge.Domain;
using TerraForge.Settings;

namespace TerraForge.Generation;

public interface IWorldGenerationStep
{
    void Apply(World world, WorldGenerationSettings settings);
}