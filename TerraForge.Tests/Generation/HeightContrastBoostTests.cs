using TerraForge.Domain;
using TerraForge.Generation;
using TerraForge.Settings;

namespace TerraForge.Tests.Generation;

public class HeightContrastBoostTests
{
    private static World CreateWorld(double[,] heights)
    {
        var w = heights.GetLength(0);
        var h = heights.GetLength(1);

        var world = new World(w, h);

        for (var y = 0; y < h; y++)
        for (var x = 0; x < w; x++)
            world.Cells[x, y].Height = heights[x, y];

        return world;
    }

    private static double Apply(double height, double power)
    {
        var world = CreateWorld(new[,] { { height } });
        var step = new HeightContrastBoost(power);
        step.Apply(world, new WorldGenerationSettings());
        return world.Cells[0, 0].Height;
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.25)]
    [InlineData(0.5)]
    [InlineData(0.75)]
    [InlineData(1.0)]
    public void Height_Is_Raised_To_Power(double h)
    {
        var result = Apply(h, 0.7);
        var expected = Math.Pow(h, 0.7);

        Assert.Equal(expected, result, 10);
    }

    [Fact]
    public void Power_Less_Than_One_Should_Flatten_Heights()
    {
        var a = Apply(0.2, 0.7);
        var b = Apply(0.8, 0.7);

        Assert.True(a > 0.2);
        Assert.True(b > 0.8);
        Assert.True(b - a < 0.8 - 0.2);
    }

    [Fact]
    public void Power_Greater_Than_One_Should_Increase_Curvature()
    {
        var a = Apply(0.2, 2.0);
        var b = Apply(0.8, 2.0);

        Assert.True(a < 0.2);

        Assert.True(b < 0.8);

        var lowDrop = 0.2 - a;
        var highDrop = 0.8 - b;

        Assert.True(lowDrop > highDrop);
    }

    [Fact]
    public void Zero_Height_Stays_Zero()
    {
        Assert.Equal(0.0, Apply(0.0, 0.7), 10);
    }

    [Fact]
    public void One_Height_Stays_One()
    {
        Assert.Equal(1.0, Apply(1.0, 0.7), 10);
    }

    [Fact]
    public void Does_Not_Throw_On_Large_World()
    {
        var world = new World(200, 200);

        for (var y = 0; y < world.Height; y++)
        for (var x = 0; x < world.Width; x++)
            world.Cells[x, y].Height = (x + y) % 100 / 100.0;

        var step = new HeightContrastBoost();

        var ex = Record.Exception(() => step.Apply(world, new WorldGenerationSettings()));

        Assert.Null(ex);
    }

    [Fact]
    public void All_Heights_Are_In_Valid_Range()
    {
        var world = new World(50, 50);

        for (var y = 0; y < world.Height; y++)
        for (var x = 0; x < world.Width; x++)
            world.Cells[x, y].Height = x * y % 100 / 100.0;

        var step = new HeightContrastBoost();
        step.Apply(world, new WorldGenerationSettings());

        foreach (var cell in world.Cells) Assert.InRange(cell.Height, 0.0, 1.0);
    }

    [Fact]
    public void Monotonicity_Is_Preserved()
    {
        var a = Apply(0.3, 0.7);
        var b = Apply(0.6, 0.7);

        Assert.True(a < b);
    }
}