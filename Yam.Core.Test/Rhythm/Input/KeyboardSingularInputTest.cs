using JetBrains.Annotations;
using Moq;
using Yam.Core.Rhythm.Chart;
using Yam.Core.Rhythm.Input;

namespace Yam.Core.Test.Rhythm.Input;

[TestSubject(typeof(KeyboardSingularInput))]
public abstract class KeyboardSingularInputTest
{
    public class Lifecycle
    {
        [Fact]
        public void SimulateClaimingLifecycle()
        {
            var input = new KeyboardSingularInput("keyCode");
            
            // you cannot claim an unpressed button
            var beat = new Mock<IBeat>();
            Assert.False(input.ClaimOnStart(beat.Object));
            Assert.Null(input.GetClaimingChannel());

            // you can claim a button only on start, for now
            input.Activate();
            Assert.Equal(SingularInputState.Started, input.GetState());
            Assert.True(input.ClaimOnStart(beat.Object));
            Assert.Equal(beat.Object, input.GetClaimingChannel());

            // you cannot claim a button that's already claimed
            var differentBeat = new Mock<IBeat>();
            Assert.False(input.ClaimOnStart(differentBeat.Object));

            // on release, all claims are released also, and the claimer will be informed
            // note: that we have to be very careful with this
            input.Release();
            beat.Verify(m => m.InformRelease(), Times.Once());
            differentBeat.Verify(m => m.InformRelease(), Times.Never());
        }
        
        [Fact]
        public void SimulateClaimingHold()
        {
            var input = new KeyboardSingularInput("keyCode");
            
            // you cannot claim an unpressed button
            var beat = new Mock<IBeat>();
            Assert.False(input.ClaimOnStart(beat.Object));
            Assert.Null(input.GetClaimingChannel());

            // you can claim a button only on start, for now
            input.Activate();
            Assert.Equal(SingularInputState.Started, input.GetState());
            
            // you cannot claim a button outside on start
            input.Activate();
            Assert.Equal(SingularInputState.Held, input.GetState());
            Assert.False(input.ClaimOnStart(beat.Object));
            Assert.Null(input.GetClaimingChannel());
        }
        
        [Fact]
        public void SimulateClaimingWithHoldLifecycle()
        {
            var input = new KeyboardSingularInput("keyCode");
            
            // you cannot claim an unpressed button
            var beat = new Mock<IBeat>();
            Assert.False(input.ClaimOnStart(beat.Object));
            Assert.Null(input.GetClaimingChannel());

            // you can claim a button only on start, for now
            input.Activate();
            Assert.Equal(SingularInputState.Started, input.GetState());
            Assert.True(input.ClaimOnStart(beat.Object));
            Assert.Equal(beat.Object, input.GetClaimingChannel());

            // hold status
            input.Activate();
            Assert.Equal(SingularInputState.Held, input.GetState());
            var differentBeat = new Mock<IBeat>();
            Assert.False(input.ClaimOnStart(differentBeat.Object));
            Assert.Equal(beat.Object, input.GetClaimingChannel());

            // on release, all claims are released also, and the claimer will be informed
            // note: that we have to be very careful with this
            input.Release();
            beat.Verify(m => m.InformRelease(), Times.Once());
            differentBeat.Verify(m => m.InformRelease(), Times.Never());
        }
    }
}