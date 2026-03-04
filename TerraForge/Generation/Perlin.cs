namespace TerraForge.Generation;

public class Perlin
{
    private readonly int[] _perm;

    public Perlin(int seed = 0)
    {
        var rand = new Random(seed);
        _perm = Enumerable.Range(0, 256).OrderBy(_ => rand.Next()).ToArray();
        _perm = _perm.Concat(_perm).ToArray();
    }

    private static double Fade(double t) => t * t * t * (t * (t * 6 - 15) + 10);
    private static double Lerp(double a, double b, double t) => a + t * (b - a);
    private static double Grad(int hash, double x, double y)
    {
        var h = hash & 3;
        return (h == 0 ? x : h == 1 ? -x : h == 2 ? y : -y);
    }

    public double Noise(double x, double y)
    {
        var xi = (int)Math.Floor(x) & 255;
        var yi = (int)Math.Floor(y) & 255;

        var xf = x - Math.Floor(x);
        var yf = y - Math.Floor(y);

        var u = Fade(xf);
        var v = Fade(yf);

        var aa = _perm[_perm[xi] + yi];
        var ab = _perm[_perm[xi] + yi + 1];
        var ba = _perm[_perm[xi + 1] + yi];
        var bb = _perm[_perm[xi + 1] + yi + 1];

        var x1 = Lerp(Grad(aa, xf, yf), Grad(ba, xf - 1, yf), u);
        var x2 = Lerp(Grad(ab, xf, yf - 1), Grad(bb, xf - 1, yf - 1), u);

        return Lerp(x1, x2, v);
    }
}