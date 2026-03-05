using TerraForge.Domain;
using TerraForge.Generation;
using TerraForge.Settings;

namespace TerraForge.Tests.Generation;

public class ContinentShaperTests
{
    private static World CreateWorld(int w, int h, double initialHeight)
    {
        var world = new World(w, h);

        for (var y = 0; y < h; y++)
        for (var x = 0; x < w; x++)
            world.Cells[x, y].Height = initialHeight;

        return world;
    }

    [Fact]
    public void Center_Cell_Should_Have_Maximum_Height()
    {
        var world = CreateWorld(5, 5, 1.0);
        var step = new ContinentShaper();

        step.Apply(world, new WorldGenerationSettings());

        var center = world.Cells[2, 2].Height;
        var all = world.Cells.Cast<Cell>().Select(c => c.Height);

        Assert.Equal(center, all.Max(), 5);
    }

    [Fact]
    public void Corner_Cells_Should_Be_Lowered()
    {
        var world = CreateWorld(5, 5, 1.0);
        var step = new ContinentShaper();

        step.Apply(world, new WorldGenerationSettings());

        var corner = world.Cells[0, 0].Height;

        Assert.True(corner < 1.0);
    }

    [Fact]
    public void Edge_Cells_Should_Have_Intermediate_Height()
    {
        var world = CreateWorld(5, 5, 1.0);
        var step = new ContinentShaper();

        step.Apply(world, new WorldGenerationSettings());

        var edge = world.Cells[0, 2].Height; // middle of left edge
        var center = world.Cells[2, 2].Height;
        var corner = world.Cells[0, 0].Height;

        Assert.True(corner < edge);
        Assert.True(edge < center);
    }

    [Fact]
    public void Factor_Should_Be_Zero_At_MaxDistance()
    {
        var world = CreateWorld(3, 3, 1.0);
        var step = new ContinentShaper();

        step.Apply(world, new WorldGenerationSettings());

        // farthest from center (1,1) is (0,0)
        var corner = world.Cells[0, 0].Height;

        Assert.Equal(0.0, corner, 5);
    }

    [Fact]
    public void Does_Not_Throw_On_Large_World()
    {
        var world = CreateWorld(200, 200, 0.5);
        var step = new ContinentShaper();

        var ex = Record.Exception(() => step.Apply(world, new WorldGenerationSettings()));

        Assert.Null(ex);
    }

    [Fact]
    public void Height_Should_Not_Increase()
    {
        var world = CreateWorld(10, 10, 0.5);
        var step = new ContinentShaper();

        step.Apply(world, new WorldGenerationSettings());

        foreach (var cell in world.Cells)
            Assert.True(cell.Height <= 0.5);
    }

    [Fact]
    public void Average_Height_Should_Decrease_With_Ring_Distance()
    {
        var world = CreateWorld(21, 21, 1.0);
        var step = new ContinentShaper();
        step.Apply(world, new WorldGenerationSettings());

        var cx = world.Width / 2.0;
        var cy = world.Height / 2.0;

        var inner = AvgHeight(0, 3);
        var mid = AvgHeight(3, 7);
        var outer = AvgHeight(7, 11);

        Assert.True(inner > mid);
        Assert.True(mid > outer);
        return;

        double AvgHeight(double minD, double maxD)
        {
            var cells = new List<double>();

            for (var y = 0; y < world.Height; y++)
            for (var x = 0; x < world.Width; x++)
            {
                var dx = x - cx;
                var dy = y - cy;
                var dist = Math.Sqrt(dx * dx + dy * dy);

                if (dist >= minD && dist < maxD)
                    cells.Add(world.Cells[x, y].Height);
            }

            return cells.Average();
        }
    }
}