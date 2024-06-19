using System.Diagnostics;
using Yam.Core.Rhythm.Clients;
using Yam.Core.Rhythm.Models.Base;
using Yam.Core.Rhythm.Services;

namespace Yam.Core.Rhythm.Servers;

internal class ChartEditor : IChartEditor
{
	public IRhythmGameHost? Host;
	private ChartModel _chartModel;
	private ChartVisualizer _visualizer;
	private ChartState chartState;

	public void Play(ChartModel chartModel)
	{
		_chartModel = chartModel;

		Debug.Assert(Host != null);
		this.chartState = new ChartState(_chartModel, Host);
		this._visualizer = new ChartVisualizer(Host, this.chartState);
		Host?.PlaySong($"{_chartModel.SelfPath}/{_chartModel?.AudioRelativePath}");
	}
}