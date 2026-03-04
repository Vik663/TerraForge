using TerraForge.Domain;

namespace TerraForge.Tests.Domain;

public class WorldTests
{
    [Fact]
    public void World_Has_Correct_Dimensions()
    {
        var world = new World(10, 20);

        Assert.Equal(10, world.Width);
        Assert.Equal(20, world.Height);
    }

    [Fact]
    public void Cells_Array_Has_Correct_Size()
    {
        var world = new World(7, 9);

        Assert.Equal(7, world.Cells.GetLength(0));
        Assert.Equal(9, world.Cells.GetLength(1));
    }

    [Fact]
    public void All_Cells_Are_Not_Null()
    {
        var world = new World(5, 5);

        for (var y = 0; y < world.Height; y++)
        for (var x = 0; x < world.Width; x++)
            Assert.NotNull(world.Cells[x, y]);
    }

    [Fact]
    public void All_Cells_Are_Distinct_Instances()
    {
        var world = new World(3, 3);

        var seen = new HashSet<Cell>();

        for (var y = 0; y < world.Height; y++)
        for (var x = 0; x < world.Width; x++)
            seen.Add(world.Cells[x, y]);

        Assert.Equal(9, seen.Count);
    }

    [Fact]
    public void Cells_Are_Initialized_With_Default_Values()
    {
        var world = new World(4, 4);

        foreach (var cell in world.Cells)
        {
            Assert.Equal(0.0, cell.Height);
            Assert.Equal(0.0, cell.Moisture);
            Assert.Equal(default, cell.Biome);
        }
    }

    [Fact]
    public void Changing_One_Cell_Does_Not_Affect_Others()
    {
        var world = new World(3, 3);

        world.Cells[1, 1].Height = 0.9;

        Assert.Equal(0.9, world.Cells[1, 1].Height);

        for (var y = 0; y < world.Height; y++)
        for (var x = 0; x < world.Width; x++)
        {
            if (x == 1 && y == 1)
                continue;

            Assert.Equal(0.0, world.Cells[x, y].Height);
        }
    }

    [Fact]
    public void World_Can_Be_Minimal_Size()
    {
        var world = new World(1, 1);

        Assert.Equal(1, world.Width);
        Assert.Equal(1, world.Height);
        Assert.NotNull(world.Cells[0, 0]);
    }

    [Fact]
    public void World_Creates_New_Cells_On_Each_Construction()
    {
        var w1 = new World(2, 2);
        var w2 = new World(2, 2);

        Assert.NotSame(w1.Cells[0, 0], w2.Cells[0, 0]);
    }
}