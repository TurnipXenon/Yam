using JetBrains.Annotations;
using Moq;
using Xunit.Abstractions;
using Yam.Core.Rhythm.Chart;
using Yam.Core.Rhythm.Input;
using Yam.Core.Test.Utility;

namespace Yam.Core.Test.Rhythm.Chart;

[TestSubject(typeof(Core.Rhythm.Chart.Chart))]
public abstract class ChartTest
{
    public class SingleBeat(ITestOutputHelper xUnitLogger) : BaseTest(xUnitLogger)
    {
        [Fact]
        public void SimultaneousSingleBeat()
        {
            var chart = BeatUtil.NewChart([
                new(10f) { UCoord = 1f },
                new(10f) { UCoord = 2f }
            ], XUnitLogger);

            var simulator = new Mock<IRhythmSimulator>();
            var input = new Mock<IRhythmInput>();

            // channels are reorganized
            simulator.Setup(s => s.GetCurrentSongTime()).Returns(0f);
            input.Setup(i => i.GetSource()).Returns(InputSource.Player);
            Assert.Equal(10f, chart.ChannelList[^2].TryToGetBeatForInput()?.Time);
            Assert.Equal(10f, chart.ChannelList.Last().TryToGetBeatForInput()?.Time);

            // at the exact time, let's do an idle
            simulator.Setup(s => s.GetCurrentSongTime()).Returns(10f);
            input.Setup(i => i.GetSource()).Returns(InputSource.Game);
            chart.SimulateBeatInput(simulator.Object, input.Object);
            Assert.Equal(10f, chart.ChannelList[^2].TryToGetBeatForInput()?.Time);
            Assert.Equal(10f, chart.ChannelList.Last().TryToGetBeatForInput()?.Time);

            // try one input first
            input.Setup(i => i.GetSource()).Returns(InputSource.Player);
            input.Setup(i => i.ClaimOnStart(It.IsAny<IBeat>()))
                .Returns(true)
                .Callback<IBeat>((beat) =>
                {
                    input.Setup(i => i.ClaimOnStart(It.IsAny<IBeat>())).Returns(false);
                });
            chart.SimulateBeatInput(simulator.Object, input.Object);
            Assert.Null(chart.ChannelList[^2].TryToGetBeatForInput()?.Time);
            Assert.Equal(10f, chart.ChannelList.Last().TryToGetBeatForInput()?.Time);

            // try second input
            input.Setup(i => i.GetSource()).Returns(InputSource.Player);
            input.Setup(i => i.ClaimOnStart(It.IsAny<IBeat>()))
                .Returns(true)
                .Callback<IBeat>((beat) =>
                {
                    input.Setup(i => i.ClaimOnStart(It.IsAny<IBeat>())).Returns(false);
                });
            chart.SimulateBeatInput(simulator.Object, input.Object);
            Assert.Null(chart.ChannelList[^2].TryToGetBeatForInput()?.Time);
            Assert.Null(chart.ChannelList.Last().TryToGetBeatForInput()?.Time);
        }
    }

    public class MultiHold
    {
        [Fact]
        public void SimpleTwoHoldSimulation()
        {
        }

        // todo(turnip): hold + tap (released)

        // todo(turnip): hold + tap (unreleased)

        // todo(turnip): hold + slide
    }
}