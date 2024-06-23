using System.Diagnostics;
using Yam.Core.Rhythm.Clients;
using Yam.Core.Rhythm.Models.Base;
using Yam.Core.Rhythm.Models.Wrappers;
using Yam.Core.Rhythm.Services;
using Yam.Core.Rhythm.Services.BeatPooler;

namespace Yam.Core.Rhythm.Servers;

internal class ChartEditor : IChartEditor
{
	public IRhythmGameHost? Host;
	public IPooledBeatResource BeatResource;

	private ChartModel _chartModel;
	private ChartVisualizer _visualizer;
	private IChartState chartState;

	public void Play(ChartModel chartModel)
	{
		_chartModel = chartModel;

		Debug.Assert(Host != null);
		chartState = new ChartState(_chartModel, Host);
		_visualizer = new ChartVisualizer(
			host: Host,
			chartState: chartState,
			pooler: new BeatPooler(BeatResource)
		);
		Host?.PlaySong($"{_chartModel.SelfPath}/{_chartModel?.AudioRelativePath}");
	}
}