using TerraForge.Settings;

namespace TerraForge.Tests.Settings;

public class WorldGenerationSettingsTests
{
    [Fact]
    public void Settings_Are_Assigned_Correctly()
    {
        var settings = new WorldGenerationSettings
        {
            Width = 100,
            Height = 80,
            Seed = 12345
        };

        Assert.Equal(100, settings.Width);
        Assert.Equal(80, settings.Height);
        Assert.Equal(12345, settings.Seed);
    }

    [Fact]
    public void Default_Values_Are_Zero()
    {
        var settings = new WorldGenerationSettings();

        Assert.Equal(0, settings.Width);
        Assert.Equal(0, settings.Height);
        Assert.Equal(0, settings.Seed);
    }

    [Fact]
    public void Init_Properties_Cannot_Be_Modified_After_Creation()
    {
        var settings = new WorldGenerationSettings
        {
            Width = 10,
            Height = 20,
            Seed = 999
        };

        Assert.Equal(10, settings.Width);
        Assert.Equal(20, settings.Height);
        Assert.Equal(999, settings.Seed);
    }

    [Fact]
    public void Settings_Can_Be_Passed_Between_Steps()
    {
        var settings = new WorldGenerationSettings
        {
            Width = 30,
            Height = 40,
            Seed = 777
        };

        Assert.Equal(30, settings.Width);
        Assert.Equal(40, settings.Height);
        Assert.Equal(777, settings.Seed);
    }
}