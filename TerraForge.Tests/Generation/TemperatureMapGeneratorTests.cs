using TerraForge.Domain;
using TerraForge.Generation;
using TerraForge.Settings;

namespace TerraForge.Tests.Generation;

public class TemperatureMapGeneratorTests
{
    [Fact]
    public void Temperature_Increases_Toward_South()
    {
        var world = new World(1, 3);

        world.Cells[0, 0].Height = 0;
        world.Cells[0, 1].Height = 0;
        world.Cells[0, 2].Height = 0;

        var gen = new TemperatureMapGenerator(seedOffset: 0);
        gen.Apply(world, new WorldGenerationSettings { Width = 1, Height = 3 });

        var north = world.Cells[0, 0].Temperature;
        var mid   = world.Cells[0, 1].Temperature;
        var south = world.Cells[0, 2].Temperature;

        Assert.True(north < mid);
        Assert.True(mid < south);
    }
    
    [Fact]
    public void HighAltitude_Is_Colder()
    {
        var world = new World(2, 1);

        world.Cells[0, 0].Height = 0.0;
        world.Cells[1, 0].Height = 1.0;

        var gen = new TemperatureMapGenerator(seedOffset: 0);
        gen.Apply(world, new WorldGenerationSettings { Width = 2, Height = 1 });

        var low  = world.Cells[0, 0].Temperature;
        var high = world.Cells[1, 0].Temperature;

        Assert.True(low >= high);
    }
    
    [Fact]
    public void Temperature_Is_Clamped_Between_0_And_1()
    {
        var world = new World(3, 3);

        foreach (var cell in world.Cells)
            cell.Height = 10;

        var gen = new TemperatureMapGenerator(seedOffset: 0);
        gen.Apply(world, new WorldGenerationSettings { Width = 3, Height = 3 });

        foreach (var cell in world.Cells)
            Assert.InRange(cell.Temperature, 0.0, 1.0);
    }
    
    [Fact]
    public void Noise_Does_Not_Invert_Latitude_Trend()
    {
        var world = new World(5, 5);

        foreach (var cell in world.Cells)
            cell.Height = 0;

        var gen = new TemperatureMapGenerator(seedOffset: 0);
        gen.Apply(world, new WorldGenerationSettings { Width = 5, Height = 5 });

        var northAvg = Enumerable.Range(0, 5).Select(x => world.Cells[x, 0].Temperature).Average();
        var southAvg = Enumerable.Range(0, 5).Select(x => world.Cells[x, 4].Temperature).Average();

        Assert.True(southAvg > northAvg);
    }
    
    [Fact]
    public void Does_Not_Throw_On_Large_World()
    {
        var world = new World(200, 200);

        foreach (var cell in world.Cells)
            cell.Height = 0.5;

        var gen = new TemperatureMapGenerator();
        var settings = new WorldGenerationSettings { Width = 200, Height = 200 };

        var ex = Record.Exception(() => gen.Apply(world, settings));

        Assert.Null(ex);
    }
}