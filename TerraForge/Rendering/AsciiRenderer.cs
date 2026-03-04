using TerraForge.Domain;

namespace TerraForge.Rendering;

public static class AsciiRenderer
{
    public static void Render(World world)
    {
        for (var y = 0; y < world.Height; y++)
        {
            for (var x = 0; x < world.Width; x++)
            {
                Console.Write(Symbol(world.Cells[x, y].Biome));
            }
            Console.WriteLine();
        }
    }
    
    public static void RenderColored(World world)
    {
        for (var y = 0; y < world.Height; y++)
        {
            for (var x = 0; x < world.Width; x++)
            {
                var cell = world.Cells[x, y];
                Console.Write(Color(cell.Biome));
                Console.Write(Symbol(cell.Biome));
            }
            Console.Write("\e[0m\n");
        }
    }
    
    private static char Symbol(Biome biome) => biome switch
    {
        Biome.Ocean     => '~',
        Biome.Beach     => '.',
        Biome.Desert    => ':',
        Biome.Savanna   => ';',
        Biome.Forest    => '*',
        Biome.Taiga     => '+',
        Biome.Hills     => '^',
        Biome.Mountains => 'A',
        _               => '?'
    };
    
    private static string Color(Biome b) => b switch
    {
        Biome.Ocean     => "\e[34m",
        Biome.Beach     => "\e[33m",
        Biome.Desert    => "\e[93m",
        Biome.Savanna   => "\e[32m",
        Biome.Forest    => "\e[32;1m",
        Biome.Taiga     => "\e[32m",
        Biome.Hills     => "\e[37m",
        Biome.Mountains => "\e[97m",
        _               => "\e[0m"
    };
}