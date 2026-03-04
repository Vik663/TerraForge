using TerraForge.Generation;
using TerraForge.Rendering;
using TerraForge.Settings;

var settings = new WorldGenerationSettings
{
    Width = 300,
    Height = 200,
    Seed = 123
};

var generator = new WorldGenerator([
    new PerlinHeightMap(settings.Seed),
    new HeightContrastBoost(),
    new WorldSmoother(1),
    new ContinentShaper(),
    new HeightNormalizer(),

    new MoistureMapGenerator(settings.Seed + 1000),
    new ClimateBiomeAssigner()
]);

var world = generator.Generate(settings);

AsciiRenderer.RenderColored(world);

PngRenderer.Render(world, "map.png");