using TerraForge.Domain;
using TerraForge.Generation;
using TerraForge.Rendering;
using TerraForge.Settings;

namespace TerraForge.Tests.Integration;

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
    public void FullPipeline_Produces_World_With_Correct_Size()
    {
        var world = GenerateFull(64, 48, 123);

        Assert.Equal(64, world.Width);
        Assert.Equal(48, world.Height);
    }

    [Fact]
    public void FullPipeline_Produces_Valid_Heights()
    {
        var world = GenerateFull(50, 50, 123);

        foreach (var cell in world.Cells)
            Assert.InRange(cell.Height, 0.0, 1.0);
    }

    [Fact]
    public void FullPipeline_Produces_Valid_Moisture()
    {
        var world = GenerateFull(50, 50, 123);

        foreach (var cell in world.Cells)
            Assert.InRange(cell.Moisture, 0.0, 1.0);
    }

    [Fact]
    public void FullPipeline_Is_Deterministic_For_Same_Seed()
    {
        var w1 = GenerateFull(40, 40, 999);
        var w2 = GenerateFull(40, 40, 999);

        for (var y = 0; y < 40; y++)
        for (var x = 0; x < 40; x++)
        {
            Assert.Equal(w1.Cells[x, y].Height, w2.Cells[x, y].Height, 10);
            Assert.Equal(w1.Cells[x, y].Moisture, w2.Cells[x, y].Moisture, 10);
        }
    }

    [Fact]
    public void FullPipeline_Produces_Different_Results_For_Different_Seeds()
    {
        var w1 = GenerateFull(40, 40, 111);
        var w2 = GenerateFull(40, 40, 222);

        var equalCount = 0;

        for (var y = 0; y < 40; y++)
        for (var x = 0; x < 40; x++)
            if (Math.Abs(w1.Cells[x, y].Height - w2.Cells[x, y].Height) < 1e-12)
                equalCount++;

        Assert.True(equalCount < 50);
    }

    [Fact]
    public void AsciiRenderer_Can_Render_FullPipeline_World()
    {
        var world = GenerateFull(10, 5, 123);

        var sw = new StringWriter();
        var original = Console.Out;
        Console.SetOut(sw);

        AsciiRenderer.Render(world);

        Console.SetOut(original);

        var output = Normalize(sw.ToString());

        Assert.Equal(5, output.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length);
    }
    
    private static string Normalize(string s) =>
        s.Replace("\r\n", "\n").Replace("\r", "\n");
}