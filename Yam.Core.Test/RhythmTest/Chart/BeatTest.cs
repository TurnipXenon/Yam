namespace Yam.Core.Test.RhythmTest.Chart;

public abstract class BeatTest
{
    public class Overlaps
    {
        [Fact]
        public void DoesNotOverlap()
        {
            var first = BeatUtil.NewBeat(0f, 1f);
            var second = BeatUtil.NewBeat(2f, 3f);
            Assert.False(first.Overlaps(second));
        }

        [Fact]
        public void EarlyTailOverlaps()
        {
            var first = BeatUtil.NewBeat(2.5f, 3f);
            var second = BeatUtil.NewBeat(2f, 3f);
            Assert.True(first.Overlaps(second));
            
            var first2 = BeatUtil.NewBeat(2f, 4f);
            var second2 = BeatUtil.NewBeat(2f, 3f);
            Assert.True(first2.Overlaps(second2));
        }

        [Fact]
        public void LateTailOverlaps()
        {
            var first = BeatUtil.NewBeat(1f, 4f);
            var second = BeatUtil.NewBeat(2f, 3f);
            Assert.True(first.Overlaps(second));
            
            var first2 = BeatUtil.NewBeat(1f, 3f);
            var second2 = BeatUtil.NewBeat(2f, 3f);
            Assert.True(first2.Overlaps(second2));
            
            var first3 = BeatUtil.NewBeat(1f, 2.1f);
            var second3 = BeatUtil.NewBeat(2f, 3f);
            Assert.True(first2.Overlaps(second3));
        }
    }
}