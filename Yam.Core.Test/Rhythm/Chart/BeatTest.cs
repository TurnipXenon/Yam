using JetBrains.Annotations;
using Moq;
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
            var first = BeatUtil.NewHoldBeat(0f, 1f);
            var second = BeatUtil.NewHoldBeat(2f, 3f);
            Assert.False(first.Overlaps(second));
        }

        [Fact]
        public void EarlyTailOverlaps()
        {
            var first = BeatUtil.NewHoldBeat(2.5f, 3f);
            var second = BeatUtil.NewHoldBeat(2f, 3f);
            Assert.True(first.Overlaps(second));
            
            var first2 = BeatUtil.NewHoldBeat(2f, 4f);
            var second2 = BeatUtil.NewHoldBeat(2f, 3f);
            Assert.True(first2.Overlaps(second2));
        }

        [Fact]
        public void LateTailOverlaps()
        {
            var first = BeatUtil.NewHoldBeat(1f, 4f);
            var second = BeatUtil.NewHoldBeat(2f, 3f);
            Assert.True(first.Overlaps(second));
            
            var first2 = BeatUtil.NewHoldBeat(1f, 3f);
            var second2 = BeatUtil.NewHoldBeat(2f, 3f);
            Assert.True(first2.Overlaps(second2));
            
            var first3 = BeatUtil.NewHoldBeat(1f, 2.1f);
            var second3 = BeatUtil.NewHoldBeat(2f, 3f);
            Assert.True(first2.Overlaps(second3));
        }
    }

    public class SimulateSingleBeat
    {
        [Fact]
        public void BeatLifecycle()
        {
            var beatTime = 10f;
            var beat = BeatUtil.NewSingleBeat(new BeatEntity
            {
                Time = beatTime,
            });

            var rhythmPlayer = new Mock<IRhythmPlayer>();
            var playerInput = new Mock<IRhythmInput>();
            playerInput.Setup(i => i.GetRhythmActionType()).Returns(RhythmActionType.Singular);
            
            // start: too far
            rhythmPlayer.Setup(r => r.GetCurrentSongTime()).Returns(beatTime - (Beat.DefaultTooEarlyRadius + Beat.FrameEpsilon));
            var tooFar = beat.SimulateInput(rhythmPlayer.Object, playerInput.Object);
            Assert.Equal(BeatInputResult.Idle, tooFar);

            // within range but no inputs yet
            rhythmPlayer.Setup(r => r.GetCurrentSongTime()).Returns(beatTime - Beat.DefaultOkRadius);
            var noInput = beat.SimulateInput(rhythmPlayer.Object, SpecialInput.GameInput);
            Assert.Equal(BeatInputResult.Anticipating, noInput);
            
            // within range of excellent with input
            rhythmPlayer.Setup(r => r.GetCurrentSongTime()).Returns(beatTime - Beat.DefaultExcellentRadius + Beat.FrameEpsilon);
            var excellentInput = beat.SimulateInput(rhythmPlayer.Object, playerInput.Object);
            Assert.Equal(BeatInputResult.Excellent, excellentInput);
            
            // check the beat after it's done
            Assert.Equal(BeatInputResult.Done, beat.SimulateInput(rhythmPlayer.Object, playerInput.Object));
        }
        
        // todo(turnip): no input and miss
        
        // todo(turnip): too early, good, ok
        
        // todo(turnip): input is already claimed and miss
    }
    
    // todo(turnip) create tests for the input provider
}