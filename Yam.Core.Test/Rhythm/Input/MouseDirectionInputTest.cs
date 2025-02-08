using Godot;
using JetBrains.Annotations;
using Moq;
using Xunit.Abstractions;
using Yam.Core.Common;
using Yam.Core.Rhythm.Chart;
using Yam.Core.Rhythm.Input;
using Yam.Core.Test.Utility;

namespace Yam.Core.Test.Rhythm.Input;

[TestSubject(typeof(MouseDirectionInput))]
public abstract class MouseDirectionInputTest
{
    public class Lifecycle
    {
        [Fact]
        public void SimulateClaimingLifecycle()
        {
            var input = new MouseDirectionInput();

            // you cannot claim an unpressed button
            var beat = new Mock<IBeat>();
            Assert.False(input.ClaimOnStart(beat.Object));
            Assert.Null(input.GetClaimingChannel(beat.Object));
            Assert.False(input.IsValidDirection());

            // no need to claim mouse
            input.SetRelativeMotion(new Vector2(0, -1));
            Assert.Equal(-Mathf.Pi / 2, input.GetDirection());
            Assert.True(input.IsValidDirection());
            input.SetRelativeMotion(new Vector2(0, 1));
            Assert.Equal(Mathf.Pi / 2, input.GetDirection());
            input.SetRelativeMotion(new Vector2(-1, 0));
            Assert.Equal(Mathf.Pi, input.GetDirection());
            Assert.True(input.IsValidDirection());
            
            input.Poll(Globals.FrameEpsilon);
            Assert.False(input.IsValidDirection());
        }
    }
}