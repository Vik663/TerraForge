using TerraForge.Domain;
using TerraForge.Generation;
using TerraForge.Settings;

namespace TerraForge.Tests.Generation;

public class HeightNormalizerTests
{
    private static World CreateWorld(double[,] heights)
    {
        var width = heights.GetLength(0);
        var height = heights.GetLength(1);

        var world = new World(width, height);

        for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
            world.Cells[x, y].Height = heights[x, y];

        return world;
    }

    private static World Normalize(double[,] heights)
    {
        var world = CreateWorld(heights);
        var step = new HeightNormalizer();
        step.Apply(world, new WorldGenerationSettings());
        return world;
    }

    [Fact]
    public void Normalizes_Simple_Range()
    {
        // width=1, height=2
        var world = Normalize(new[,]
        {
            { 0.0, 5.0 }
        });

        Assert.Equal(0.0, world.Cells[0, 0].Height, 10);
        Assert.Equal(1.0, world.Cells[0, 1].Height, 10);
    }

    [Fact]
    public void Preserves_Order()
    {
        var world = Normalize(new[,]
        {
            { 2.0, 4.0, 8.0 }
        });

        var a = world.Cells[0, 0].Height;
        var b = world.Cells[0, 1].Height;
        var c = world.Cells[0, 2].Height;

        Assert.True(a < b);
        Assert.True(b < c);
    }

    [Fact]
    public void All_Values_Are_In_0_1_Range()
    {
        var world = Normalize(new[,]
        {
            { -10.0, 0.0, 10.0 },
            { 5.0, 7.5, -2.5 }
        });

        foreach (var cell in world.Cells)
            Assert.InRange(cell.Height, 0.0, 1.0);
    }

    [Fact]
    public void Handles_Uniform_Height_Without_Exception()
    {
        var world = CreateWorld(new[,]
        {
            { 5.0, 5.0, 5.0 }
        });

        var step = new HeightNormalizer();
        var ex = Record.Exception(() => step.Apply(world, new WorldGenerationSettings()));

        Assert.Null(ex);
    }

    [Fact]
    public void Uniform_Height_Results_In_All_Zeros()
    {
        var world = Normalize(new[,]
        {
            { 5.0, 5.0, 5.0 }
        });

        foreach (var cell in world.Cells)
            Assert.Equal(0.0, cell.Height, 10);
    }

    [Fact]
    public void Large_World_Does_Not_Throw()
    {
        var world = new World(300, 300);

        for (var y = 0; y < world.Height; y++)
        for (var x = 0; x < world.Width; x++)
            world.Cells[x, y].Height = (x + y) % 100;

        var step = new HeightNormalizer();
        var ex = Record.Exception(() => step.Apply(world, new WorldGenerationSettings()));

        Assert.Null(ex);
    }

    [Fact]
    public void Normalization_Is_Correct_For_Mixed_Values()
    {
        var world = Normalize(new[,]
        {
            { -2.0, 3.0, 8.0 }
        });

        var a = world.Cells[0, 0].Height;
        var b = world.Cells[0, 1].Height;
        var c = world.Cells[0, 2].Height;

        Assert.Equal(0.0, a, 10);
        Assert.Equal(0.5, b, 10);
        Assert.Equal(1.0, c, 10);
    }
}