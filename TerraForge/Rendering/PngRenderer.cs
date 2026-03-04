using SkiaSharp;
using TerraForge.Domain;

namespace TerraForge.Rendering;

public static class PngRenderer
{
    public static void Render(World world, string path, int scale = 4)
    {
        var w = world.Width;
        var h = world.Height;

        using var small = new SKBitmap(w, h);
        using var canvasSmall = new SKCanvas(small);

        for (var y = 0; y < h; y++)
        {
            for (var x = 0; x < w; x++)
            {
                var cell = world.Cells[x, y];

                var biomeColor = BiomeColor(cell.Biome);
                var shaded = ApplyHeightShading(biomeColor, cell.Height);
                var final = ApplyHillshade(world, x, y, shaded);

                canvasSmall.DrawPoint(x, y, final);
            }
        }

        using var large = new SKBitmap(w * scale, h * scale);
        using var canvasLarge = new SKCanvas(large);

        canvasLarge.Scale(scale, scale);
        canvasLarge.DrawBitmap(small, 0, 0);

        using var image = SKImage.FromBitmap(large);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = File.OpenWrite(path);
        data.SaveTo(stream);
    }

    private static SKColor ApplyHeightShading(SKColor baseColor, double height)
    {
        var brightness = (float)(0.7 + height * 0.6);

        var r = (byte)Math.Clamp(baseColor.Red   * brightness, 0, 255);
        var g = (byte)Math.Clamp(baseColor.Green * brightness, 0, 255);
        var b = (byte)Math.Clamp(baseColor.Blue  * brightness, 0, 255);

        return new SKColor(r, g, b);
    }

    private static SKColor ApplyHillshade(World world, int x, int y, SKColor baseColor)
    {
        var w = world.Width;
        var h = world.Height;

        var hL = world.Cells[Math.Max(x - 1, 0), y].Height;
        var hR = world.Cells[Math.Min(x + 1, w - 1), y].Height;
        var hU = world.Cells[x, Math.Max(y - 1, 0)].Height;
        var hD = world.Cells[x, Math.Min(y + 1, h - 1)].Height;

        var dx = hR - hL;
        var dy = hD - hU;

        double lightX = -1;
        double lightY = -1;
        double lightZ = 1;

        var len = Math.Sqrt(lightX * lightX + lightY * lightY + lightZ * lightZ);
        lightX /= len; lightY /= len; lightZ /= len;

        var nx = -dx;
        var ny = -dy;
        double nz = 1;

        var nlen = Math.Sqrt(nx * nx + ny * ny + nz * nz);
        nx /= nlen; ny /= nlen; nz /= nlen;

        var dot = Math.Max(0.0, nx * lightX + ny * lightY + nz * lightZ);

        var shade = (float)(0.5 + dot * 0.5);

        var r = (byte)Math.Clamp(baseColor.Red   * shade, 0, 255);
        var g = (byte)Math.Clamp(baseColor.Green * shade, 0, 255);
        var b = (byte)Math.Clamp(baseColor.Blue  * shade, 0, 255);

        return new SKColor(r, g, b);
    }

    private static SKColor BiomeColor(Biome b) => b switch
    {
        Biome.Ocean     => new SKColor(30, 60, 200),
        Biome.Beach     => new SKColor(240, 230, 140),
        Biome.Desert    => new SKColor(237, 201, 175),
        Biome.Savanna   => new SKColor(181, 189, 104),
        Biome.Forest    => new SKColor(34, 139, 34),
        Biome.Taiga     => new SKColor(0, 100, 0),
        Biome.Hills     => new SKColor(120, 120, 120),
        Biome.Mountains => new SKColor(220, 220, 220),
        _               => SKColors.Magenta
    };
}