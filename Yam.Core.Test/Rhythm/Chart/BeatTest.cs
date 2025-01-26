using JetBrains.Annotations;
using Moq;
using Xunit.Abstractions;
using Yam.Core.Common;
using Yam.Core.Rhythm.Chart;
using Yam.Core.Rhythm.Input;
using Yam.Core.Test.Utility;

namespace Yam.Core.Test.Rhythm.Chart;

[TestSubject(typeof(Beat))]
public abstract class BeatTest
{
    public class Overlaps
    {
        [Fact]
        public void DoesNotOverlap()
        {
            var first = BeatUtil.NewHoldBeat_OverlapTest(0f, 1f);
            var second = BeatUtil.NewHoldBeat_OverlapTest(2f, 3f);
            Assert.False(first.Overlaps(second));
        }

        [Fact]
        public void EarlyTailOverlaps()
        {
            var first = BeatUtil.NewHoldBeat_OverlapTest(2.5f, 3f);
            var second = BeatUtil.NewHoldBeat_OverlapTest(2f, 3f);
            Assert.True(first.Overlaps(second));

            var first2 = BeatUtil.NewHoldBeat_OverlapTest(2f, 4f);
            var second2 = BeatUtil.NewHoldBeat_OverlapTest(2f, 3f);
            Assert.True(first2.Overlaps(second2));
        }

        [Fact]
        public void LateTailOverlaps()
        {
            var first = BeatUtil.NewHoldBeat_OverlapTest(1f, 4f);
            var second = BeatUtil.NewHoldBeat_OverlapTest(2f, 3f);
            Assert.True(first.Overlaps(second));

            var first2 = BeatUtil.NewHoldBeat_OverlapTest(1f, 3f);
            var second2 = BeatUtil.NewHoldBeat_OverlapTest(2f, 3f);
            Assert.True(first2.Overlaps(second2));

            var first3 = BeatUtil.NewHoldBeat_OverlapTest(1f, 2.1f);
            var second3 = BeatUtil.NewHoldBeat_OverlapTest(2f, 3f);
            Assert.True(first3.Overlaps(second3));
        }
    }

    public class SimulateSingleBeat : BaseTest
    {
        public SimulateSingleBeat(ITestOutputHelper xUnitLogger) : base(xUnitLogger)
        {
        }

        [Fact]
        public void BeatLifecycle()
        {
            var beatTime = 10f;
            var beat = BeatUtil.NewSingleBeat(new BeatEntity(time: beatTime), XUnitLogger);

            var rhythmSimulator = new Mock<IRhythmSimulator>();
            var playerInput = new Mock<IRhythmInput>();
            playerInput.Setup(i => i.GetRhythmActionType()).Returns(RhythmActionType.Singular);
            playerInput.Setup(i => i.GetSource()).Returns(InputSource.Player);

            // start: too far
            rhythmSimulator.Setup(r => r.GetCurrentSongTime())
                .Returns(beatTime - (Beat.DefaultTooEarlyRadius + Globals.FrameEpsilon));
            var tooFar = beat.SimulateInput(rhythmSimulator.Object, playerInput.Object);
            Assert.Equal(BeatInputResult.Idle, tooFar);

            // within range but no inputs yet
            rhythmSimulator.Setup(r => r.GetCurrentSongTime()).Returns(beatTime - Beat.DefaultOkRadius);
            var noInput = beat.SimulateInput(rhythmSimulator.Object, SpecialInput.GameInput);
            Assert.Equal(BeatInputResult.Anticipating, noInput);

            // within range of excellent with input
            rhythmSimulator.Setup(r => r.GetCurrentSongTime())
                .Returns(beatTime - Beat.DefaultExcellentRadius + Globals.FrameEpsilon);
            playerInput.Setup(i => i.ClaimOnStart(beat)).Returns(true);
            var excellentInput = beat.SimulateInput(rhythmSimulator.Object, playerInput.Object);
            Assert.Equal(BeatInputResult.Excellent, excellentInput);

            // check the beat after it's done
            Assert.Equal(BeatInputResult.Done, beat.SimulateInput(rhythmSimulator.Object, playerInput.Object));
        }


        [Fact]
        public void InputCannotBeClaimedTwice()
        {
            var beatTime = 10f;
            var beat1 = BeatUtil.NewSingleBeat(new BeatEntity(time: beatTime), XUnitLogger);
            var beat2 = BeatUtil.NewSingleBeat(new BeatEntity(time: beatTime), XUnitLogger);

            var simulator = new Mock<IRhythmSimulator>();
            var playerInput = new Mock<IRhythmInput>();
            playerInput.Setup(i => i.GetRhythmActionType()).Returns(RhythmActionType.Singular);
            playerInput.Setup(i => i.GetSource()).Returns(InputSource.Player);

            // within range of excellent with input
            simulator.Setup(r => r.GetCurrentSongTime())
                .Returns(beatTime - Beat.DefaultExcellentRadius + Globals.FrameEpsilon);
            playerInput.Setup(i => i.ClaimOnStart(beat1)).Returns(true);
            var excellentInput = beat1.SimulateInput(simulator.Object, playerInput.Object);
            Assert.Equal(BeatInputResult.Excellent, excellentInput);
            playerInput.Setup(i => i.GetClaimingChannel(It.IsAny<IBeat>())).Returns(beat1);

            var anticipating = beat2.SimulateInput(simulator.Object, playerInput.Object);
            Assert.Equal(BeatInputResult.Anticipating, anticipating);

            // check the beat after it's done
            Assert.Equal(BeatInputResult.Done, beat1.SimulateInput(simulator.Object, playerInput.Object));
        }

        // todo(turnip): cannot claim a beat when in held or release state

        // todo(turnip): no input and miss

        // todo(turnip): too early, good, ok

        // todo(turnip): input is already claimed and miss
    }

    public class SimulateHoldBeat : BaseTest
    {
        public SimulateHoldBeat(ITestOutputHelper xUnitLogger) : base(xUnitLogger)
        {
        }

        [Fact]
        public void SimpleHoldLifecycle()
        {
            var startTime = 10f;
            var endTime = 15f;
            var beat = BeatUtil.NewHoldBeat([
                new BeatEntity(startTime),
                new BeatEntity(endTime)
            ], XUnitLogger);
            beat.Logger.XUnitLogger = XUnitLogger;


            var simulator = new Mock<IRhythmSimulator>();
            var playerInput = new Mock<IRhythmInput>();
            playerInput.Setup(i => i.GetRhythmActionType()).Returns(RhythmActionType.Singular);

            // assert we're doing hold beats
            Assert.Equal(BeatType.Hold, beat.GetBeatType());

            // start: too far
            simulator.Setup(r => r.GetCurrentSongTime())
                .Returns(startTime - (Beat.DefaultTooEarlyRadius + Globals.FrameEpsilon));
            var tooFar = beat.SimulateInput(simulator.Object, playerInput.Object);
            Assert.Equal(BeatInputResult.Idle, tooFar);

            // within range but no inputs yet
            simulator.Setup(r => r.GetCurrentSongTime()).Returns(startTime - Beat.DefaultOkRadius);
            var noInput = beat.SimulateInput(simulator.Object, SpecialInput.GameInput);
            Assert.Equal(BeatInputResult.Anticipating, noInput);

            // within range of excellent with input
            simulator.Setup(r => r.GetCurrentSongTime())
                .Returns(startTime - Beat.DefaultExcellentRadius + Globals.FrameEpsilon);
            playerInput.Setup(i => i.ClaimOnStart(beat)).Returns(true);
            var holding = beat.SimulateInput(simulator.Object, playerInput.Object);
            Assert.Equal(BeatInputResult.Holding, holding);

            // check the beat after it's done
            Assert.Equal(BeatInputResult.Holding, beat.SimulateInput(simulator.Object, SpecialInput.GameInput));

            simulator.Setup(r => r.GetCurrentSongTime()).Returns((startTime + endTime) / 2f);
            Assert.Equal(BeatInputResult.Holding, beat.SimulateInput(simulator.Object, SpecialInput.GameInput));

            simulator.Setup(r => r.GetCurrentSongTime()).Returns(endTime);
            Assert.Equal(BeatInputResult.Holding, beat.SimulateInput(simulator.Object, SpecialInput.GameInput));

            beat.OnInputRelease();
            Assert.Equal(BeatInputResult.Excellent, beat.HoldReleaseResult);
        }

        [Fact]
        public void LateHoldRelease()
        {
            var startTime = 10f;
            var endTime = 15f;
            var beat = BeatUtil.NewHoldBeat([
                new BeatEntity(startTime),
                new BeatEntity(endTime)
            ], XUnitLogger);
            beat.Logger.XUnitLogger = XUnitLogger;


            var simulator = new Mock<IRhythmSimulator>();
            var playerInput = new Mock<IRhythmInput>();
            playerInput.Setup(i => i.GetRhythmActionType()).Returns(RhythmActionType.Singular);

            // assert we're doing hold beats
            Assert.Equal(BeatType.Hold, beat.GetBeatType());

            // start: too far
            simulator.Setup(r => r.GetCurrentSongTime())
                .Returns(startTime - (Beat.DefaultTooEarlyRadius + Globals.FrameEpsilon));
            var tooFar = beat.SimulateInput(simulator.Object, playerInput.Object);
            Assert.Equal(BeatInputResult.Idle, tooFar);

            // within range but no inputs yet
            simulator.Setup(r => r.GetCurrentSongTime()).Returns(startTime - Beat.DefaultOkRadius);
            var noInput = beat.SimulateInput(simulator.Object, SpecialInput.GameInput);
            Assert.Equal(BeatInputResult.Anticipating, noInput);

            // within range of excellent with input
            simulator.Setup(r => r.GetCurrentSongTime())
                .Returns(startTime - Beat.DefaultExcellentRadius + Globals.FrameEpsilon);
            playerInput.Setup(i => i.ClaimOnStart(beat)).Returns(true);
            var holding = beat.SimulateInput(simulator.Object, playerInput.Object);
            Assert.Equal(BeatInputResult.Holding, holding);

            // check the beat after it's done
            Assert.Equal(BeatInputResult.Holding, beat.SimulateInput(simulator.Object, SpecialInput.GameInput));

            simulator.Setup(r => r.GetCurrentSongTime()).Returns((startTime + endTime) / 2f);
            Assert.Equal(BeatInputResult.Holding, beat.SimulateInput(simulator.Object, SpecialInput.GameInput));

            simulator.Setup(r => r.GetCurrentSongTime()).Returns(endTime);
            Assert.Equal(BeatInputResult.Holding, beat.SimulateInput(simulator.Object, SpecialInput.GameInput));
            Assert.Equal(BeatInputResult.Holding, beat.HoldReleaseResult);

            simulator.Setup(r => r.GetCurrentSongTime()).Returns(endTime + Beat.DefaultOkRadius);
            beat.SimulateHoldingIdleBeat();
            Assert.Equal(BeatInputResult.Miss, beat.HoldReleaseResult);
            
            beat.OnInputRelease();
            Assert.Equal(BeatInputResult.Miss, beat.HoldReleaseResult);
        }
        
        [Fact]
        public void OkHoldRelease()
        {
            var startTime = 10f;
            var endTime = 15f;
            var beat = BeatUtil.NewHoldBeat([
                new BeatEntity(startTime),
                new BeatEntity(endTime)
            ], XUnitLogger);
            beat.Logger.XUnitLogger = XUnitLogger;


            var simulator = new Mock<IRhythmSimulator>();
            var playerInput = new Mock<IRhythmInput>();
            playerInput.Setup(i => i.GetRhythmActionType()).Returns(RhythmActionType.Singular);

            // assert we're doing hold beats
            Assert.Equal(BeatType.Hold, beat.GetBeatType());

            // start: too far
            simulator.Setup(r => r.GetCurrentSongTime())
                .Returns(startTime - (Beat.DefaultTooEarlyRadius + Globals.FrameEpsilon));
            var tooFar = beat.SimulateInput(simulator.Object, playerInput.Object);
            Assert.Equal(BeatInputResult.Idle, tooFar);

            // within range but no inputs yet
            simulator.Setup(r => r.GetCurrentSongTime()).Returns(startTime - Beat.DefaultOkRadius);
            var noInput = beat.SimulateInput(simulator.Object, SpecialInput.GameInput);
            Assert.Equal(BeatInputResult.Anticipating, noInput);

            // within range of excellent with input
            simulator.Setup(r => r.GetCurrentSongTime())
                .Returns(startTime - Beat.DefaultExcellentRadius + Globals.FrameEpsilon);
            playerInput.Setup(i => i.ClaimOnStart(beat)).Returns(true);
            var holding = beat.SimulateInput(simulator.Object, playerInput.Object);
            Assert.Equal(BeatInputResult.Holding, holding);

            // check the beat after it's done
            Assert.Equal(BeatInputResult.Holding, beat.SimulateInput(simulator.Object, SpecialInput.GameInput));

            simulator.Setup(r => r.GetCurrentSongTime()).Returns((startTime + endTime) / 2f);
            Assert.Equal(BeatInputResult.Holding, beat.SimulateInput(simulator.Object, SpecialInput.GameInput));

            simulator.Setup(r => r.GetCurrentSongTime()).Returns(endTime);
            Assert.Equal(BeatInputResult.Holding, beat.SimulateInput(simulator.Object, SpecialInput.GameInput));
            Assert.Equal(BeatInputResult.Holding, beat.HoldReleaseResult);

            simulator.Setup(r => r.GetCurrentSongTime()).Returns(endTime + Beat.DefaultOkRadius - Globals.FrameEpsilon);
            beat.SimulateHoldingIdleBeat();
            Assert.Equal(BeatInputResult.Holding, beat.HoldReleaseResult);
            
            beat.OnInputRelease();
            Assert.Equal(BeatInputResult.Ok, beat.HoldReleaseResult);
        }

        // todo: hold release outside excellent

        // todo: hold beat with multiple ticks

        // todo: hold beat with movements

        // todo: hold beat ignore movements when all the ticks have the same point and no in/out anchors

        // todo(turnip): cannot be claimed twice

        // todo(turnip): cannot claim a beat when in held or release state

        // todo(turnip): no input and miss

        // todo(turnip): too early, good, ok

        // todo(turnip): input is already claimed and miss

        // todo(turnip): handle input switching when there are two holds that end at different times

        // todo(turnip): handle input switching when there is a hold and a beat
    }
}