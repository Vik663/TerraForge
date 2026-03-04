using TerraForge.Domain;
using TerraForge.Rendering;

namespace TerraForge.Tests.Rendering;

public class AsciiRendererTests
{
    private static World CreateWorld(Biome[,] biomes)
    {
        var width = biomes.GetLength(0);
        var height = biomes.GetLength(1);

        var world = new World(width, height);

        for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
            world.Cells[x, y].Biome = biomes[x, y];

        return world;
    }

    private static string CaptureOutput(Action action)
    {
        var sw = new StringWriter();
        var original = Console.Out;
        Console.SetOut(sw);

        try
        {
            action();
        }
        finally
        {
            Console.SetOut(original);
        }

        return sw.ToString();
    }

    [Fact]
    public void Render_Prints_Correct_Symbols()
    {
        var world = CreateWorld(new[,]
        {
            { Biome.Ocean, Biome.Forest },
            { Biome.Beach, Biome.Mountains }
        });

        var output = CaptureOutput(() => AsciiRenderer.Render(world));

        const string expected = "~.\n" +
                                "*A\n";

        Assert.Equal(expected, output);
    }

    [Fact]
    public void RenderColored_Prints_Colors_And_Symbols()
    {
        var world = CreateWorld(new[,]
        {
            { Biome.Ocean, Biome.Hills },
            { Biome.Desert, Biome.Mountains }
        });

        var output = CaptureOutput(() => AsciiRenderer.RenderColored(world));

        const string expected = "\e[34m~\e[93m:\e[0m\n" +
                                "\e[37m^\e[97mA\e[0m\n";

        Assert.Equal(expected, output);
    }


    [Fact]
    public void Render_Produces_Correct_Number_Of_Lines()
    {
        var world = CreateWorld(new[,]
        {
            { Biome.Ocean, Biome.Ocean, Biome.Ocean }
        });

        var output = CaptureOutput(() => AsciiRenderer.Render(world));

        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal(3, lines.Length);
    }

    [Fact]
    public void RenderColored_Ends_Each_Line_With_Reset()
    {
        var world = CreateWorld(new[,]
        {
            { Biome.Forest, Biome.Forest }
        });

        var output = CaptureOutput(() => AsciiRenderer.RenderColored(world));

        Assert.Contains("\e[0m\n", output);
    }

    [Fact]
    public void Render_Does_Not_Throw_On_Empty_World()
    {
        var world = new World(0, 0);

        var ex = Record.Exception(() => AsciiRenderer.Render(world));

        Assert.Null(ex);
    }

    [Fact]
    public void RenderColored_Does_Not_Throw_On_Empty_World()
    {
        var world = new World(0, 0);

        var ex = Record.Exception(() => AsciiRenderer.RenderColored(world));

        Assert.Null(ex);
    }
}