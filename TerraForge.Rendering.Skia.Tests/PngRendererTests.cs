using SkiaSharp;
using TerraForge.Domain;
using TerraForge.Rendering.Rendering;
using Xunit;

namespace TerraForge.Rendering.Skia.Tests;

[Trait("Category", "Rendering")]
public class PngRendererTests
{
    private static World CreateWorld(int w, int h, Biome biome, double height = 0.0)
    {
        var world = new World(w, h);

        for (var y = 0; y < h; y++)
        for (var x = 0; x < w; x++)
        {
            world.Cells[x, y].Biome = biome;
            world.Cells[x, y].Height = height;
        }

        return world;
    }

    private static SKBitmap LoadBitmap(byte[] png)
    {
        using var ms = new MemoryStream(png);
        using var skData = SKData.Create(ms);
        using var img = SKImage.FromEncodedData(skData);
        return SKBitmap.FromImage(img);
    }

    [Fact]
    public void Render_Creates_Png_File()
    {
        var world = CreateWorld(4, 4, Biome.Forest);

        using var ms = new MemoryStream();
        var path = Path.GetTempFileName();

        PngRenderer.Render(world, path);

        Assert.True(File.Exists(path));
        Assert.True(new FileInfo(path).Length > 0);

        File.Delete(path);
    }

    [Fact]
    public void Render_Produces_Image_With_Correct_Size()
    {
        var world = CreateWorld(3, 2, Biome.Ocean);

        var path = Path.GetTempFileName();
        PngRenderer.Render(world, path, 5);

        var bytes = File.ReadAllBytes(path);
        var bmp = LoadBitmap(bytes);

        Assert.Equal(3 * 5, bmp.Width);
        Assert.Equal(2 * 5, bmp.Height);

        File.Delete(path);
    }

    [Fact]
    public void Render_Uses_Biome_Color_Tone()
    {
        var world = CreateWorld(1, 1, Biome.Desert);

        var path = Path.GetTempFileName();
        PngRenderer.Render(world, path);

        var bytes = File.ReadAllBytes(path);
        var bmp = LoadBitmap(bytes);

        var pixel = bmp.GetPixel(0, 0);

        var baseColor = new SKColor(237, 201, 175);

        Assert.True(pixel.Red < baseColor.Red);
        Assert.True(pixel.Green < baseColor.Green);
        Assert.True(pixel.Blue < baseColor.Blue);

        var ratioR = pixel.Red / (double)baseColor.Red;
        var ratioG = pixel.Green / (double)baseColor.Green;
        var ratioB = pixel.Blue / (double)baseColor.Blue;

        Assert.InRange(ratioR, 0.4, 0.9);
        Assert.InRange(ratioG, 0.4, 0.9);
        Assert.InRange(ratioB, 0.4, 0.9);

        File.Delete(path);
    }


    [Fact]
    public void Render_Applies_Height_Shading()
    {
        var world = CreateWorld(1, 1, Biome.Forest, 1.0);

        var path = Path.GetTempFileName();
        PngRenderer.Render(world, path);

        var bytes = File.ReadAllBytes(path);
        var bmp = LoadBitmap(bytes);

        var pixel = bmp.GetPixel(0, 0);

        // Forest base color = (34, 139, 34)
        // Height shading brightens it
        Assert.True(pixel.Red >= 34);
        Assert.True(pixel.Green >= 139);
        Assert.True(pixel.Blue >= 34);

        File.Delete(path);
    }

    [Fact]
    public void Render_Applies_Hillshade()
    {
        var world = new World(3, 3);

        // center high, neighbors low -> should be bright
        world.Cells[1, 1].Height = 1.0;

        var path = Path.GetTempFileName();
        PngRenderer.Render(world, path);

        var bytes = File.ReadAllBytes(path);
        var bmp = LoadBitmap(bytes);

        var center = bmp.GetPixel(1, 1);
        var corner = bmp.GetPixel(0, 0);

        Assert.True(center.Red >= corner.Red);
        Assert.True(center.Green >= corner.Green);
        Assert.True(center.Blue >= corner.Blue);

        File.Delete(path);
    }

    [Fact]
    public void Render_Does_Not_Throw_On_Large_World()
    {
        var world = CreateWorld(200, 200, Biome.Hills);

        var path = Path.GetTempFileName();
        var ex = Record.Exception(() => PngRenderer.Render(world, path));

        Assert.Null(ex);

        File.Delete(path);
    }
}