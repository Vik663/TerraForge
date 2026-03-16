using TerraForge.Domain;

namespace TerraForge.Tests.Domain;

public class BiomeTests
{
    [Fact]
    public void Biome_Contains_All_Expected_Values()
    {
        var expected = new[]
        {
            Biome.Ocean,
            Biome.Beach,
            Biome.Desert,
            Biome.Savanna,
            Biome.Forest,
            Biome.Taiga,
            Biome.Hills,
            Biome.Mountains,
            Biome.Snow
        };

        var actual = Enum.GetValues<Biome>();

        Assert.Equal(expected.Length, actual.Length);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Biome_Can_Be_Converted_To_String_And_Back()
    {
        foreach (var biome in Enum.GetValues<Biome>())
        {
            var s = biome.ToString();
            var parsed = Enum.Parse<Biome>(s);

            Assert.Equal(biome, parsed);
        }
    }

    [Fact]
    public void Biome_Has_Stable_Order()
    {
        Assert.Equal(0, (int)Biome.Ocean);
        Assert.Equal(1, (int)Biome.Beach);
        Assert.Equal(2, (int)Biome.Desert);
        Assert.Equal(3, (int)Biome.Savanna);
        Assert.Equal(4, (int)Biome.Forest);
        Assert.Equal(5, (int)Biome.Taiga);
        Assert.Equal(6, (int)Biome.Hills);
        Assert.Equal(7, (int)Biome.Mountains);
    }
}