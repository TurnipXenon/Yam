using JetBrains.Annotations;
using Xunit.Abstractions;
using Yam.Core.Common;
using Yam.Core.Test.Utility;
using Yam.Core.Twenty48;

namespace Yam.Core.Test.Twenty48;

[TestSubject(typeof(Twenty48Simulator))]
public class Twenty48SimulatorTest(ITestOutputHelper xUnitLogger): BaseTest(xUnitLogger)
{
    [Fact]
    public void SimulateSimpleGame()
    {
        var simulator = new Twenty48Simulator(28);
        Assert.Empty(simulator.NodeList);

        simulator.NewGame();
        Assert.Single(simulator.NodeList);
        var startingNode = simulator.NodeList[0];
        Assert.Empty(startingNode.neighbors);
        Assert.True(startingNode is { PosX: 4, PosY: 0 }, "Starting Node should be (4, 0)");

        simulator.Move(BitwiseDirection.Left);
        Assert.True(startingNode is { PosX: 0, PosY: 0 }, "Starting Node should have moved to (0, 0)");
        Assert.True(simulator.NodeList.Count == 2, "There should be new node generated");
    }
}