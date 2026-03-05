using TerraForge.Domain;
using TerraForge.Generation;
using TerraForge.Settings;

namespace TerraForge.Tests.Generation;

public class PerlinHeightMapTests
{
    private static World Generate(int w, int h, int seed)
    {
        var world = new World(w, h);
        var settings = new WorldGenerationSettings { Seed = 123 };
        var step = new PerlinHeightMap(seed);
        step.Apply(world, settings);
        return world;
    }

    [Fact]
    public void Same_Seed_Produces_Same_HeightMap()
    {
        var w1 = Generate(40, 40, 999);
        var w2 = Generate(40, 40, 999);

        for (var y = 0; y < 40; y++)
        for (var x = 0; x < 40; x++)
            Assert.Equal(w1.Cells[x, y].Height, w2.Cells[x, y].Height, 10);
    }

    [Fact]
    public void Different_Seeds_Produce_Different_HeightMaps()
    {
        var w1 = Generate(40, 40, 111);
        var w2 = Generate(40, 40, 222);

        var equalCount = 0;

        for (var y = 0; y < 40; y++)
        for (var x = 0; x < 40; x++)
            if (Math.Abs(w1.Cells[x, y].Height - w2.Cells[x, y].Height) < 1e-12)
                equalCount++;

        Assert.True(equalCount < 50);
    }

    [Fact]
    public void Height_Is_In_0_1_Range()
    {
        var world = Generate(50, 50, 1234);

        foreach (var cell in world.Cells)
            Assert.InRange(cell.Height, 0.0, 1.0);
    }

    [Fact]
    public void Fills_All_Cells()
    {
        var world = Generate(20, 20, 777);

        foreach (var cell in world.Cells)
            Assert.NotEqual(0.0, cell.Height);
    }

    [Fact]
    public void HeightMap_Is_Smooth()
    {
        var world = Generate(20, 20, 555);

        var a = world.Cells[10, 10].Height;
        var b = world.Cells[11, 10].Height;

        Assert.True(Math.Abs(a - b) < 0.3);
    }

    [Fact]
    public void Does_Not_Throw_On_Large_World()
    {
        var ex = Record.Exception(() => Generate(300, 300, 42));
        Assert.Null(ex);
    }
}