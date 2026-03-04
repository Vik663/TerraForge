using TerraForge.Domain;
using TerraForge.Generation;
using TerraForge.Settings;

namespace TerraForge.Tests.Generation;

public class HeightMapGeneratorTests
{
    private static World Generate(int w, int h, int seed)
    {
        var world = new World(w, h);
        var settings = new WorldGenerationSettings { Seed = seed };
        var step = new HeightMapGenerator();
        step.Apply(world, settings);
        return world;
    }

    [Fact]
    public void Generates_Deterministic_Output_For_Same_Seed()
    {
        var w1 = Generate(20, 20, 123);
        var w2 = Generate(20, 20, 123);

        for (var y = 0; y < 20; y++)
        for (var x = 0; x < 20; x++)
            Assert.Equal(w1.Cells[x, y].Height, w2.Cells[x, y].Height, 10);
    }

    [Fact]
    public void Generates_Different_Output_For_Different_Seeds()
    {
        var w1 = Generate(20, 20, 111);
        var w2 = Generate(20, 20, 222);

        var equalCount = 0;

        for (var y = 0; y < 20; y++)
        for (var x = 0; x < 20; x++)
            if (Math.Abs(w1.Cells[x, y].Height - w2.Cells[x, y].Height) < 1e-9)
                equalCount++;

        Assert.True(equalCount < 20);
    }

    [Fact]
    public void All_Heights_Are_In_Range_0_1()
    {
        var world = Generate(50, 50, 999);

        foreach (var cell in world.Cells)
            Assert.InRange(cell.Height, 0.0, 1.0);
    }

    [Fact]
    public void Fills_All_Cells()
    {
        var world = Generate(10, 10, 42);

        foreach (var cell in world.Cells)
            Assert.NotEqual(0.0, cell.Height);
    }

    [Fact]
    public void Does_Not_Throw_On_Large_World()
    {
        var ex = Record.Exception(() => Generate(500, 500, 123));
        Assert.Null(ex);
    }
}