using TerraForge.Domain;
using TerraForge.Generation;
using TerraForge.Settings;

namespace TerraForge.Tests.Generation;

public class WorldGeneratorTests
{
    [Fact]
    public void Creates_World_With_Correct_Size()
    {
        IEnumerable<IWorldGenerationStep> steps = [];
        var generator = new WorldGenerator(steps);

        var settings = new WorldGenerationSettings { Width = 42, Height = 37 };
        var world = generator.Generate(settings);

        Assert.Equal(42, world.Width);
        Assert.Equal(37, world.Height);
    }

    [Fact]
    public void Calls_All_Steps_In_Order()
    {
        var s1 = new TestStep();
        var s2 = new TestStep();
        var s3 = new TestStep();

        var generator = new WorldGenerator([s1, s2, s3]);

        var settings = new WorldGenerationSettings { Width = 10, Height = 10 };
        generator.Generate(settings);

        Assert.Equal(1, s1.CallCount);
        Assert.Equal(1, s2.CallCount);
        Assert.Equal(1, s3.CallCount);

        Assert.Equal((10, 10), s1.AppliedWorldSizes[0]);
        Assert.Equal((10, 10), s2.AppliedWorldSizes[0]);
        Assert.Equal((10, 10), s3.AppliedWorldSizes[0]);
    }

    [Fact]
    public void Passes_Settings_To_All_Steps()
    {
        var s1 = new TestStep();
        var s2 = new TestStep();

        var generator = new WorldGenerator([s1, s2]);

        var settings = new WorldGenerationSettings { Width = 5, Height = 5, Seed = 777 };
        generator.Generate(settings);

        Assert.Equal(777, s1.ReceivedSettings[0].Seed);
        Assert.Equal(777, s2.ReceivedSettings[0].Seed);
    }

    [Fact]
    public void Steps_Are_Applied_In_Sequence()
    {
        var step1 = new HeightSetterStep(0.2);
        var step2 = new MoistureSetterStep(0.8);

        var generator = new WorldGenerator([step1, step2]);

        var settings = new WorldGenerationSettings { Width = 3, Height = 3 };
        var world = generator.Generate(settings);

        foreach (var cell in world.Cells)
        {
            Assert.Equal(0.2, cell.Height);
            Assert.Equal(0.8, cell.Moisture);
        }
    }

    [Fact]
    public void Later_Steps_Override_Earlier_Steps()
    {
        var step1 = new HeightSetterStep(0.1);
        var step2 = new HeightSetterStep(0.9);

        var generator = new WorldGenerator([step1, step2]);

        var settings = new WorldGenerationSettings { Width = 4, Height = 4 };
        var world = generator.Generate(settings);

        foreach (var cell in world.Cells)
            Assert.Equal(0.9, cell.Height);
    }

    [Fact]
    public void Works_With_Empty_Step_List()
    {
        var generator = new WorldGenerator([]);

        var settings = new WorldGenerationSettings { Width = 7, Height = 7 };
        var world = generator.Generate(settings);

        foreach (var cell in world.Cells)
        {
            Assert.Equal(0.0, cell.Height);
            Assert.Equal(0.0, cell.Moisture);
        }
    }

    [Fact]
    public void Does_Not_Throw_On_Large_World()
    {
        var steps = new IWorldGenerationStep[] { new HeightSetterStep(0.5), new MoistureSetterStep(0.3) };
        var generator = new WorldGenerator(steps);

        var settings = new WorldGenerationSettings { Width = 400, Height = 400 };

        var ex = Record.Exception(() => generator.Generate(settings));
        Assert.Null(ex);
    }

    private class TestStep : IWorldGenerationStep
    {
        public int CallCount { get; private set; }
        public List<(int W, int H)> AppliedWorldSizes { get; } = [];
        public List<WorldGenerationSettings> ReceivedSettings { get; } = [];

        public void Apply(World world, WorldGenerationSettings settings)
        {
            CallCount++;
            AppliedWorldSizes.Add((world.Width, world.Height));
            ReceivedSettings.Add(settings);
        }
    }

    private class HeightSetterStep(double value) : IWorldGenerationStep
    {
        public void Apply(World world, WorldGenerationSettings settings)
        {
            for (var y = 0; y < world.Height; y++)
            for (var x = 0; x < world.Width; x++)
                world.Cells[x, y].Height = value;
        }
    }

    private class MoistureSetterStep(double value) : IWorldGenerationStep
    {
        public void Apply(World world, WorldGenerationSettings settings)
        {
            for (var y = 0; y < world.Height; y++)
            for (var x = 0; x < world.Width; x++)
                world.Cells[x, y].Moisture = value;
        }
    }
}