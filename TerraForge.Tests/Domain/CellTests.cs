using TerraForge.Domain;

namespace TerraForge.Tests.Domain;

public class CellTests
{
    [Fact]
    public void Default_Cell_Has_Zero_Height_And_Moisture_And_Default_Biome()
    {
        var cell = new Cell();

        Assert.Equal(0.0, cell.Height);
        Assert.Equal(0.0, cell.Moisture);
        Assert.Equal(default, cell.Biome);
    }

    [Fact]
    public void Height_Can_Be_Set_And_Retrieved()
    {
        var cell = new Cell { Height = 0.75 };
        Assert.Equal(0.75, cell.Height);
    }

    [Fact]
    public void Moisture_Can_Be_Set_And_Retrieved()
    {
        var cell = new Cell { Moisture = 0.42 };
        Assert.Equal(0.42, cell.Moisture);
    }

    [Fact]
    public void Biome_Can_Be_Set_And_Retrieved()
    {
        var cell = new Cell { Biome = Biome.Forest };
        Assert.Equal(Biome.Forest, cell.Biome);
    }

    [Fact]
    public void Properties_Are_Independent()
    {
        var cell = new Cell
        {
            Height = 0.3,
            Moisture = 0.8,
            Biome = Biome.Desert
        };

        Assert.Equal(0.3, cell.Height);
        Assert.Equal(0.8, cell.Moisture);
        Assert.Equal(Biome.Desert, cell.Biome);
    }

    [Fact]
    public void Can_Assign_All_Biome_Values()
    {
        foreach (var biome in Enum.GetValues<Biome>())
        {
            var cell = new Cell { Biome = biome };
            Assert.Equal(biome, cell.Biome);
        }
    }
}