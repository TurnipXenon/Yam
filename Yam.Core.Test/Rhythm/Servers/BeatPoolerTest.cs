using JetBrains.Annotations;
using Moq;
using Yam.Core.Rhythm.Servers;
using Yam.Core.Rhythm.Services.BeatPooler;
using Yam.Core.Test.Utils;

namespace Yam.Core.Test.Rhythm.Servers;

[TestSubject(typeof(BeatPooler))]
public class BeatPoolerTest
{
	public class RequestBeat
	{
		[Fact]
		public void SimulateBasicPooler()
		{
			// simulates requesting pooled beats which maybe pooled or are new resources
			// then destroying the pooler at the end, releasing all references to Godot objects (IPooledBeatHost)

			// in real use case, you should not have the same host for every pooled beat!
			var host = new Mock<IPooledBeatHost>();
			host.Setup(h => h.DestroyResource()).Verifiable();

			var resource = new Mock<IPooledBeatResource>();
			resource.Setup(r => r.RequestResource()).Returns(new PooledBeat(host.Object));
			var pooler = new BeatPooler(resource.Object);

			var pooledBeat1 = pooler.RequestBeat(TestingUtils.NewBeatState());
			resource.Verify(r => r.RequestResource(), Times.Once);
			Assert.NotNull(pooledBeat1);

			var pooledBeat2 = pooler.RequestBeat(TestingUtils.NewBeatState());
			resource.Verify(r => r.RequestResource(), Times.Exactly(2));
			Assert.NotNull(pooledBeat2);

			pooledBeat1!.Deactivate();
			pooledBeat2!.Deactivate();
			pooler.RequestBeat(TestingUtils.NewBeatState());
			pooler.RequestBeat(TestingUtils.NewBeatState());
			resource.Verify(r => r.RequestResource(), Times.Exactly(2));

			pooler.RequestBeat(TestingUtils.NewBeatState());
			resource.Verify(r => r.RequestResource(), Times.Exactly(3));

			pooler.DestroyAllResources();
			host.Verify(h => h.DestroyResource(), Times.Exactly(3));
			Assert.Null(pooler.RequestBeat(TestingUtils.NewBeatState()));
		}

		// todo: reusing the same beat state
	}
}