using SkiaSharp;
using TerraForge.Domain;
using TerraForge.Generation;
using TerraForge.Rendering.Rendering;
using TerraForge.Settings;
using Xunit;

namespace TerraForge.Rendering.Skia.Tests;

public class WorldIntegrationTests
{
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
    
    [Fact]
    public void PngRenderer_Can_Render_FullPipeline_World()
    {
        var world = GenerateFull(32, 32, 123);

        var path = Path.GetTempFileName();

        var ex = Record.Exception(() => PngRenderer.Render(world, path));

        Assert.Null(ex);
        Assert.True(File.Exists(path));
        Assert.True(new FileInfo(path).Length > 0);

        File.Delete(path);
    }

    [Fact]
    public void PngRenderer_Output_Has_Correct_Size()
    {
        var world = GenerateFull(16, 16, 123);

        var path = Path.GetTempFileName();
        PngRenderer.Render(world, path, 3);

        var bytes = File.ReadAllBytes(path);

        using var ms = new MemoryStream(bytes);
        using var data = SKData.Create(ms);
        using var img = SKImage.FromEncodedData(data);
        using var bmp = SKBitmap.FromImage(img);

        Assert.Equal(16 * 3, bmp.Width);
        Assert.Equal(16 * 3, bmp.Height);

        File.Delete(path);
    }
}