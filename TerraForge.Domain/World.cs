namespace TerraForge.Domain;

public class World
{
    public World(int width, int height)
    {
        Width = width;
        Height = height;
        Cells = new Cell[width, height];

        for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
            Cells[x, y] = new Cell();
    }

    public int Width { get; }
    public int Height { get; }
    public Cell[,] Cells { get; }
}