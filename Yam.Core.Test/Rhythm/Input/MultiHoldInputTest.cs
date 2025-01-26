using JetBrains.Annotations;
using Moq;
using Xunit.Abstractions;
using Yam.Core.Rhythm.Chart;
using Yam.Core.Rhythm.Input;
using Yam.Core.Test.Utility;

namespace Yam.Core.Test.Rhythm.Input;

[TestSubject(typeof(MultiHoldInput))]
public abstract class MultiHoldInputTest
{
    
    public class MultiHold(ITestOutputHelper xUnitLogger) : BaseTest(xUnitLogger)
    {
        [Fact]
        public void SimpleTwoHoldSimulation()
        {
            var startTime = 10f;
            var earlierEnd = 15f;
            var laterEnd = earlierEnd + Beat.DefaultGoodRadius;
            var laterBeat = BeatUtil.NewHoldBeat([
                new BeatEntity(startTime, 1f),
                new BeatEntity(laterEnd, 1f)
            ], XUnitLogger);
            var earlierBeat = BeatUtil.NewHoldBeat([
                new BeatEntity(startTime, 1f),
                new BeatEntity(earlierEnd, 1f)
            ], XUnitLogger);

            var simulator = new Mock<IRhythmSimulator>();
            var input1 = new Mock<IRhythmInput>();
            var input2 = new Mock<IRhythmInput>();
            
            // initialize simulator
            simulator.Setup(s => s.GetCurrentSongTime()).Returns(startTime);
            var multiHoldInput = new MultiHoldInput(simulator.Object, input1.Object);
            multiHoldInput.Logger.XUnitLogger = XUnitLogger;

            // try one input first
            input1.Setup(i => i.GetSource()).Returns(InputSource.Player);
            input1.Setup(i => i.ClaimOnStart(It.IsAny<IBeat>()))
                .Returns(true)
                .Callback<IBeat>((beat) => { input1.Setup(i => i.ClaimOnStart(It.IsAny<IBeat>())).Returns(false); });
            Assert.Equal(BeatInputResult.Holding, laterBeat.SimulateInput(simulator.Object, multiHoldInput));
            input1.Verify(i => i.ClaimOnStart(multiHoldInput), Times.Once);
            input2.Verify(i => i.ClaimOnStart(multiHoldInput), Times.Never);
            
            // try second input
            multiHoldInput.AddInput(input2.Object);
            input2.Setup(i => i.GetSource()).Returns(InputSource.Player);
            input2.Setup(i => i.ClaimOnStart(It.IsAny<IBeat>()))
                .Returns(true)
                .Callback<IBeat>((beat) => { input1.Setup(i => i.ClaimOnStart(It.IsAny<IBeat>())).Returns(false); });
            Assert.Equal(BeatInputResult.Holding, earlierBeat.SimulateInput(simulator.Object, multiHoldInput));
            input1.Verify(i => i.ClaimOnStart(multiHoldInput), Times.Once);
            input2.Verify(i => i.ClaimOnStart(multiHoldInput), Times.Once);
            
            // check results
            Assert.Equal(BeatInputResult.Holding, earlierBeat.HoldReleaseResult);
            Assert.Equal(BeatInputResult.Holding, laterBeat.HoldReleaseResult);
            
            // first, let's release during earlierBeat
            simulator.Setup(s => s.GetCurrentSongTime()).Returns(earlierEnd);
            multiHoldInput.OnInputRelease();
            Assert.Equal(BeatInputResult.Excellent, earlierBeat.HoldReleaseResult);
            Assert.Equal(BeatInputResult.Holding, laterBeat.HoldReleaseResult);
            
            // first, let's release during earlierBeat
            simulator.Setup(s => s.GetCurrentSongTime()).Returns(earlierEnd);
            multiHoldInput.OnInputRelease();
            Assert.Equal(BeatInputResult.Excellent, earlierBeat.HoldReleaseResult);
            Assert.Equal(BeatInputResult.Ok, laterBeat.HoldReleaseResult);
        }
        
        // todo(turnip): second input not registered within visual time

        // todo(turnip): hold + tap (released)

        // todo(turnip): hold + tap (unreleased)

        // todo(turnip): hold + slide
    }
}