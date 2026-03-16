using TerraForge.Domain;
using TerraForge.Generation;
using TerraForge.Rendering.Rendering;
using TerraForge.Settings;
using Xunit;

namespace TerraForge.Rendering.Skia.Tests;

public class PngRendererTemperatureTests
{
    private static World CreateWorld(int w, int h, double temp)
    {
        var world = new World(w, h);

        for (var y = 0; y < h; y++)
        for (var x = 0; x < w; x++)
            world.Cells[x, y].Temperature = temp;

        return world;
    }

    [Fact]
    public void RenderTemperatureMap_Creates_Png_File()
    {
        var world = CreateWorld(4, 4, 0.5);

        var path = Path.GetTempFileName();
        PngRenderer.RenderTemperatureMap(world, path);

        Assert.True(File.Exists(path));
        Assert.True(new FileInfo(path).Length > 0);

        File.Delete(path);
    }
    
    [Fact]
    public void Temperature_Increases_Toward_South()
    {
        var settings = new WorldGenerationSettings
        {
            Width = 50,
            Height = 50,
            Seed = 123
        };

        var generator = new WorldGenerator([
            new TemperatureMapGenerator(seedOffset: 0)
        ]);


        var world = generator.Generate(settings);

        var top = Enumerable.Range(0, settings.Width)
            .Select(x => world.Cells[x, 0].Temperature)
            .Average();

        var bottom = Enumerable.Range(0, settings.Width)
            .Select(x => world.Cells[x, settings.Height - 1].Temperature)
            .Average();

        Assert.True(bottom > top);
    }
    
    [Fact]
    public void High_Mountains_Are_Colder()
    {
        var world = new World(2, 1);

        world.Cells[0, 0].Height = 0.0;
        world.Cells[1, 0].Height = 1.0;

        var gen = new TemperatureMapGenerator(seedOffset: 0);

        gen.Apply(world, new WorldGenerationSettings
        {
            Width = 2,
            Height = 1,
            Seed = 0
        });

        var low = world.Cells[0, 0].Temperature;
        var high = world.Cells[1, 0].Temperature;

        Assert.True(low > high);
    }

}