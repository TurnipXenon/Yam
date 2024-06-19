using Moq;
using Yam.Core.Rhythm.Clients;
using Yam.Core.Rhythm.Models.Base;
using Yam.Core.Rhythm.Servers;

namespace Yam.Core.Test.Rhythm;

public class ChartVisualizerTest
{
	[Fact]
	public void TestPreemptTimeBeatInitialization()
	{
		// reference: https://osu.ppy.sh/wiki/en/Beatmap/Approach_rate
		var host = new Mock<IRhythmGameHost>();
		var model = new ChartModel();
		model.TimingSections.Add(new TimingSection());
		var audioPositionMock = new Mock<IAudioPosition>();
		var chartState = new Mock<ChartState>(model, audioPositionMock.Object);
		var visualizer = new ChartVisualizer(host.Object, chartState.Object);

		audioPositionMock.Setup(a => a.GetPlaybackPosition()).Returns(0);
		visualizer.Tick(0);
		
		// todo: check if beat was not created
		// visualizer calls the pooler
		// todo: mock pooler, pass it to the constructor for ChartVisualizer
		// check if beat was requested by visualizer
	}
}