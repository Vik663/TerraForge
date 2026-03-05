using TerraForge.Domain;
using TerraForge.Generation;
using TerraForge.Settings;

namespace TerraForge.Tests.Generation;

public class ClimateBiomeAssignerTests
{
    private static World CreateWorld(double height, double moisture)
    {
        var world = new World(1, 1);
        world.Cells[0, 0].Height = height;
        world.Cells[0, 0].Moisture = moisture;
        return world;
    }

    private static Biome Assign(double h, double m)
    {
        var world = CreateWorld(h, m);
        var step = new ClimateBiomeAssigner();
        step.Apply(world, new WorldGenerationSettings());
        return world.Cells[0, 0].Biome;
    }

    [Theory]
    [InlineData(0.0, 0.0)]
    [InlineData(0.15, 0.5)]
    [InlineData(0.2999, 0.99)]
    public void Assigns_Ocean_When_Height_Below_0_3(double h, double m)
    {
        Assert.Equal(Biome.Ocean, Assign(h, m));
    }

    [Theory]
    [InlineData(0.3001, 0.0)]
    [InlineData(0.32, 0.5)]
    [InlineData(0.3499, 0.99)]
    public void Assigns_Beach_When_Height_Between_0_3_And_0_35(double h, double m)
    {
        Assert.Equal(Biome.Beach, Assign(h, m));
    }

    [Theory]
    [InlineData(0.35, 0.0, Biome.Desert)]
    [InlineData(0.50, 0.1, Biome.Desert)]
    [InlineData(0.50, 0.4, Biome.Savanna)]
    [InlineData(0.50, 0.59, Biome.Savanna)]
    [InlineData(0.50, 0.6, Biome.Forest)]
    [InlineData(0.59, 0.99, Biome.Forest)]
    public void Assigns_MidElevation_Biomes_Based_On_Moisture(double h, double m, Biome expected)
    {
        Assert.Equal(expected, Assign(h, m));
    }

    [Theory]
    [InlineData(0.60, 0.0, Biome.Hills)]
    [InlineData(0.70, 0.39, Biome.Hills)]
    [InlineData(0.60, 0.4, Biome.Taiga)]
    [InlineData(0.75, 0.99, Biome.Taiga)]
    public void Assigns_HighElevation_Biomes(double h, double m, Biome expected)
    {
        Assert.Equal(expected, Assign(h, m));
    }

    [Theory]
    [InlineData(0.80)]
    [InlineData(0.90)]
    [InlineData(1.00)]
    public void Assigns_Mountains_When_Height_Above_0_8(double h)
    {
        Assert.Equal(Biome.Mountains, Assign(h, 0.0));
    }

    [Fact]
    public void Does_Not_Throw_On_Larger_World()
    {
        var world = new World(50, 50);

        for (var y = 0; y < world.Height; y++)
        for (var x = 0; x < world.Width; x++)
        {
            world.Cells[x, y].Height = (x + y) % 100 / 100.0;
            world.Cells[x, y].Moisture = x * y % 100 / 100.0;
        }

        var step = new ClimateBiomeAssigner();
        var settings = new WorldGenerationSettings();

        var exception = Record.Exception(() => step.Apply(world, settings));

        Assert.Null(exception);
    }
}