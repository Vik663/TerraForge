using TerraForge.Domain;
using TerraForge.Generation;
using TerraForge.Rendering;
using TerraForge.Settings;

namespace TerraForge.Tests.Integration;

public class WorldIntegrationAdditionalTests
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
    public void FullPipeline_Produces_No_NaN_Or_Infinity()
    {
        var world = GenerateFull(50, 50, 123);

        foreach (var cell in world.Cells)
        {
            Assert.False(double.IsNaN(cell.Height));
            Assert.False(double.IsNaN(cell.Moisture));
            Assert.False(double.IsInfinity(cell.Height));
            Assert.False(double.IsInfinity(cell.Moisture));
        }
    }

    [Fact]
    public void FullPipeline_Produces_Smooth_Height_Transitions()
    {
        var world = GenerateFull(30, 30, 123);

        for (var y = 1; y < world.Height; y++)
        for (var x = 1; x < world.Width; x++)
        {
            var diff = Math.Abs(world.Cells[x, y].Height - world.Cells[x - 1, y - 1].Height);
            Assert.True(diff < 0.5);
        }
    }

    [Fact]
    public void FullPipeline_Produces_Smooth_Moisture_Transitions()
    {
        var world = GenerateFull(30, 30, 123);

        for (var y = 1; y < world.Height; y++)
        for (var x = 1; x < world.Width; x++)
        {
            var diff = Math.Abs(world.Cells[x, y].Moisture - world.Cells[x - 1, y - 1].Moisture);
            Assert.True(diff < 0.5);
        }
    }

    [Fact]
    public void AsciiRenderer_Does_Not_Throw_On_FullPipeline_World()
    {
        var world = GenerateFull(20, 10, 123);

        var ex = Record.Exception(() => AsciiRenderer.Render(world));

        Assert.Null(ex);
    }

    [Fact]
    public void AsciiRenderer_Produces_Correct_Number_Of_Characters()
    {
        var world = GenerateFull(5, 3, 123);

        var sw = new StringWriter();
        var original = Console.Out;
        Console.SetOut(sw);

        AsciiRenderer.Render(world);

        Console.SetOut(original);

        var output = sw.ToString().Replace("\n", "");

        Assert.Equal(5 * 3, output.Length);
    }

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

    [Fact]
    public void FullPipeline_Works_On_Minimal_World()
    {
        var world = GenerateFull(1, 1, 123);

        Assert.InRange(world.Cells[0, 0].Height, 0.0, 1.0);
        Assert.InRange(world.Cells[0, 0].Moisture, 0.0, 1.0);
    }

    [Fact]
    public void FullPipeline_Works_On_NonSquare_World()
    {
        var world = GenerateFull(5, 12, 123);

        Assert.Equal(5, world.Width);
        Assert.Equal(12, world.Height);
    }
}