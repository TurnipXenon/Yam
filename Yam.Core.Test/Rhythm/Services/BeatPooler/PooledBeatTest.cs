using System.Numerics;
using JetBrains.Annotations;
using Moq;
using Yam.Core.Rhythm.Services.BeatPooler;
using Yam.Core.Test.Utils;

namespace Yam.Core.Test.Rhythm.Services.BeatPooler;

[TestSubject(typeof(PooledBeat))]
public class PooledBeatTest
{
	public class Tick
	{
		[Fact]
		public void SimulateBeatLifecycle()
		{
			var host = new Mock<IPooledBeatHost>();
			Vector2 currentPosition = Vector2.Zero;
			host.Setup(h => h.SetPosition(It.IsAny<Vector2>()))
				.Callback((Vector2 v) => currentPosition = v);
			host.Setup(h => h.Deactivate()).Verifiable();
			var pooledBeat = new PooledBeat(host.Object);

			pooledBeat.Tick();
			host.Verify(h => h.SetPosition(It.IsAny<Vector2>()), Times.Never);
			host.Verify(h => h.Deactivate(), Times.Never);

			var resource = new Mock<IPooledBeatResource>();
			resource.Setup(r => r.GetSpawningPoint())
				.Returns(Vector2.One);
			resource.Setup(r => r.GetTriggerPoint())
				.Returns(Vector2.One * 10f);
			resource.Setup(r => r.GetDestructionPoint())
				.Returns(Vector2.One * 15f);

			var pooler = new Mock<Core.Rhythm.Servers.BeatPooler>(resource.Object);
			pooledBeat.Initialize(pooler.Object, resource.Object);

			pooledBeat.Tick();
			host.Verify(h => h.Deactivate(), Times.Once);
			host.Verify(h => h.SetPosition(It.IsAny<Vector2>()), Times.Never);

			// 1.2f is preempt time so at time 0, it should be around the spawning point
			resource.Setup(r => r.GetPlaybackPosition())
				.Returns(0);
			var b = TestingUtils.NewBeatState(timing: 1.2f);
			pooledBeat.SetActive(b);

			pooledBeat.Tick();
			host.Verify(h => h.SetPosition(It.IsAny<Vector2>()), Times.Once);
			Assert.Equal(Vector2.One, currentPosition);

			// mid point between spawning point and trigger point
			resource.Setup(r => r.GetPlaybackPosition())
				.Returns(0.6f);
			pooledBeat.Tick();
			host.Verify(h => h.SetPosition(It.IsAny<Vector2>()), Times.Exactly(2));
			Assert.Equal(Vector2.One * (11f / 2f), currentPosition);

			resource.Setup(r => r.GetPlaybackPosition())
				.Returns(1.2f);
			pooledBeat.Tick();
			host.Verify(h => h.SetPosition(It.IsAny<Vector2>()), Times.Exactly(3));
			host.Verify(h => h.Deactivate(), Times.Once);
			Assert.Equal(Vector2.One * 10f, currentPosition);

			resource.Setup(r => r.GetPlaybackPosition())
				.Returns(2.4f);
			pooledBeat.Tick();
			host.Verify(h => h.SetPosition(It.IsAny<Vector2>()), Times.Exactly(4));
			host.Verify(h => h.Deactivate(), Times.Exactly(2));
			Assert.True(currentPosition.X > 15f);
		}
	}
}