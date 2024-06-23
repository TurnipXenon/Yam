using JetBrains.Annotations;
using Moq;
using Yam.Core.Rhythm.Clients;
using Yam.Core.Rhythm.Models.Base;
using Yam.Core.Rhythm.Models.Wrappers;
using Yam.Core.Rhythm.Servers;
using Yam.Core.Rhythm.Services.BeatPooler;

namespace Yam.Core.Test.Rhythm.Servers;

// structure inspired by https://haacked.com/archive/2012/01/02/structuring-unit-tests.aspx/

[TestSubject(typeof(ChartVisualizer))]
public class ChartVisualizerTest
{
	public class Tick
	{
		[Fact]
		public void SimulateBeatVisualizationCorrectly()
		{
			// initialize data
			// reference: https://osu.ppy.sh/wiki/en/Beatmap/Approach_rate
			var preemptDuration = 1.2f;
			var beatTiming1 = 1.818f;
			var beatTiming2 = 3f;
			var beatTiming3 = 3.3f;
			var allBeatTimings = new List<float> { beatTiming1, beatTiming2, beatTiming3 };

			// setup pooler stub to make sure the correct order of beats are called
			var host = new Mock<IRhythmGameHost>();
			var resource = new Mock<IPooledBeatResource>();
			resource.Setup(r => r.RequestResource());
			var pooler = new Mock<BeatPooler>(resource.Object);
			var calledBeats = new List<BeatState>();
			pooler.Setup(p => p.RequestBeat(It.IsAny<BeatState>()))
				.Callback((BeatState b) => { calledBeats.Add(b); });

			var model = new ChartModel
			{
				ApproachRate = 5, // preempt time is 1.2f for approach rate of 5
				Beats = allBeatTimings.Select(timing => new BeatModel
				{
					Timing = timing,
					Type = BeatType.Tap
				}).ToList()
			};
			model.TimingSections.Add(new TimingSection());
			var chartState = new Mock<ChartState>(model, host.Object);
			var visualizer = new ChartVisualizer(
				host: host.Object,
				chartState: chartState.Object,
				pooler: pooler.Object
			);

			// test at 0, and it should not call RequestBeat
			host.Setup(a => a.GetPlaybackPosition()).Returns(0);
			visualizer.Tick(0);
			pooler.Verify(p => p.RequestBeat(It.IsAny<BeatState>()), Times.Never());

			// test at 1.818f - 1.2f preempt time
			host.Setup(a => a.GetPlaybackPosition()).Returns(beatTiming1 - preemptDuration + 0.2f);
			visualizer.Tick(0);
			pooler.Verify(p => p.RequestBeat(It.IsAny<BeatState>()), Times.Once);
			Assert.Equal(new[] { beatTiming1 }, calledBeats.Select(s => s.Beat.Timing).ToArray());

			// test at same time, it should not call RequestBeat
			visualizer.Tick(0);
			pooler.Verify(p => p.RequestBeat(It.IsAny<BeatState>()), Times.Once);

			// test at next beat, it should call the last two beats
			host.Setup(a => a.GetPlaybackPosition()).Returns(beatTiming3 - preemptDuration + 0.2f);
			visualizer.Tick(0);
			pooler.Verify(p => p.RequestBeat(It.IsAny<BeatState>()), Times.Exactly(3));
			Assert.Equal(allBeatTimings, calledBeats.Select(s => s.Beat.Timing).ToArray());

			// test at the same time period but no beats should be called anymore
			visualizer.Tick(0);
			pooler.Verify(p => p.RequestBeat(It.IsAny<BeatState>()), Times.Exactly(3));
		}
	}
}