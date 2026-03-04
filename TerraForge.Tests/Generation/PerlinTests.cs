using TerraForge.Generation;

namespace TerraForge.Tests.Generation;

public class PerlinTests
{
    [Fact]
    public void Same_Seed_Produces_Same_Noise()
    {
        var p1 = new Perlin(12345);
        var p2 = new Perlin(12345);

        for (var i = 0; i < 20; i++)
        {
            var x = i * 0.1;
            var y = i * 0.2;

            var a = p1.Noise(x, y);
            var b = p2.Noise(x, y);

            Assert.Equal(a, b, 10);
        }
    }

    [Fact]
    public void Different_Seeds_Produce_Different_Noise()
    {
        var p1 = new Perlin(111);
        var p2 = new Perlin(222);

        var equalCount = 0;

        for (var i = 0; i < 20; i++)
        {
            var x = i * 0.15;
            var y = i * 0.35;

            if (Math.Abs(p1.Noise(x, y) - p2.Noise(x, y)) < 1e-12)
                equalCount++;
        }

        Assert.True(equalCount < 5);
    }

    [Fact]
    public void Noise_Is_In_Range_Negative1_To_1()
    {
        var p = new Perlin(999);

        for (var y = 0; y < 10; y++)
        for (var x = 0; x < 10; x++)
        {
            var v = p.Noise(x * 0.1, y * 0.1);
            Assert.InRange(v, -1.0, 1.0);
        }
    }

    [Fact]
    public void Noise_Is_Continuous()
    {
        var p = new Perlin(555);

        var v1 = p.Noise(10.0, 20.0);
        var v2 = p.Noise(10.01, 20.01);

        Assert.True(Math.Abs(v1 - v2) < 0.2);
    }

    [Fact]
    public void Does_Not_Throw_On_Large_Coordinates()
    {
        var p = new Perlin(42);

        var ex = Record.Exception(() =>
        {
            for (var i = 0; i < 1000; i++)
                p.Noise(i * 10_000, i * 10_000);
        });

        Assert.Null(ex);
    }
}