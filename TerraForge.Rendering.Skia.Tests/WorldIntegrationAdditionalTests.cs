using TerraForge.Domain;
using TerraForge.Generation;
using TerraForge.Rendering.Rendering;
using TerraForge.Settings;
using Xunit;

namespace TerraForge.Rendering.Skia.Tests;

public class WorldIntegrationAdditionalTests
{
    [Fact]
    public void PngRenderer_Produces_Valid_Png_Header()
    {
        var world = GenerateFull(8, 8, 123);

        var path = Path.GetTempFileName();
        PngRenderer.Render(world, path);

        var bytes = File.ReadAllBytes(path);

        // PNG signature: 89 50 4E 47 0D 0A 1A 0A
        Assert.Equal(0x89, bytes[0]);
        Assert.Equal(0x50, bytes[1]);
        Assert.Equal(0x4E, bytes[2]);
        Assert.Equal(0x47, bytes[3]);

        File.Delete(path);
    }

    [Fact]
    public void PngRenderer_Produces_Different_Images_For_Different_Seeds()
    {
        var w1 = GenerateFull(16, 16, 111);
        var w2 = GenerateFull(16, 16, 222);

        var p1 = Path.GetTempFileName();
        var p2 = Path.GetTempFileName();

        PngRenderer.Render(w1, p1);
        PngRenderer.Render(w2, p2);

        var b1 = File.ReadAllBytes(p1);
        var b2 = File.ReadAllBytes(p2);

        File.Delete(p1);
        File.Delete(p2);

        Assert.NotEqual(b1, b2);
    }
    
    private static World GenerateFull(int width, int height, int seed)
    {
        var steps = new IWorldGenerationStep[]
        {
            new PerlinHeightMap(seed),
            new MoistureMapGenerator(seed + 1000),
            new HeightNormalizer()
        };

        var generator = new WorldGenerator(steps);

        var settings = new WorldGenerationSettings
        {
            Width = width,
            Height = height,
            Seed = seed
        };

        return generator.Generate(settings);
    }
}