using TerraForge.Domain;
using TerraForge.Generation;
using TerraForge.Settings;

namespace TerraForge.Tests.Generation;

public class MoistureMapGeneratorTests
{
    private static World Generate(int w, int h, int seedOffset)
    {
        var world = new World(w, h);
        var settings = new WorldGenerationSettings { Seed = 123 };
        var step = new MoistureMapGenerator(seedOffset);
        step.Apply(world, settings);
        return world;
    }

    [Fact]
    public void Generates_Deterministic_Output_For_Same_SeedOffset()
    {
        var w1 = Generate(30, 30, 10000);
        var w2 = Generate(30, 30, 10000);

        for (var y = 0; y < 30; y++)
        for (var x = 0; x < 30; x++)
            Assert.Equal(w1.Cells[x, y].Moisture, w2.Cells[x, y].Moisture, 10);
    }

    [Fact]
    public void Generates_Different_Output_For_Different_SeedOffsets()
    {
        var w1 = Generate(30, 30, 10000);
        var w2 = Generate(30, 30, 20000);

        var equalCount = 0;

        for (var y = 0; y < 30; y++)
        for (var x = 0; x < 30; x++)
            if (Math.Abs(w1.Cells[x, y].Moisture - w2.Cells[x, y].Moisture) < 1e-9)
                equalCount++;

        Assert.True(equalCount < 50);
    }

    [Fact]
    public void Moisture_Is_In_0_1_Range()
    {
        var world = Generate(50, 50, 10000);

        foreach (var cell in world.Cells)
            Assert.InRange(cell.Moisture, 0.0, 1.0);
    }

    [Fact]
    public void Fills_All_Cells()
    {
        var world = Generate(20, 20, 10000);

        foreach (var cell in world.Cells)
            Assert.NotEqual(0.0, cell.Moisture);
    }

    [Fact]
    public void Does_Not_Throw_On_Large_World()
    {
        var ex = Record.Exception(() => Generate(300, 300, 10000));
        Assert.Null(ex);
    }
}