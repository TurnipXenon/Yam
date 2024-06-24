using System.Diagnostics;
using Yam.Core.Rhythm.Clients;
using Yam.Core.Rhythm.Models.Base;
using Yam.Core.Rhythm.Models.States;
using Yam.Core.Rhythm.Services;
using Yam.Core.Rhythm.Services.BeatPooler;
using Yam.Core.Rhythm.Services.NotePooler;

namespace Yam.Core.Rhythm.Servers;

internal class ChartEditor : IChartEditor
{
	public IRhythmGameHost? Host;
	public IPooledBeatResource ChartVisualizerResource;
	public IPooledNoteResource EditorVisualizerResource;

	private ChartModel _chartModel;
	private ChartVisualizer _visualizer;
	private IChartState chartState;
	private ChartEditorVisualizer _editorVisualizer;

	public void Play(ChartModel chartModel)
	{
		_chartModel = chartModel;

		Debug.Assert(Host != null);
		chartState = new ChartState(_chartModel, Host);
		_visualizer = new ChartVisualizer(
			host: Host,
			chartState: chartState,
			pooler: new BeatPooler(ChartVisualizerResource)
		);
		_editorVisualizer = new ChartEditorVisualizer(
			host: Host,
			chartState: chartState,
			resource: EditorVisualizerResource
		);
		Host?.PlaySong($"{_chartModel.SelfPath}/{_chartModel?.AudioRelativePath}");
	}

	public void OnRewind()
	{
		_visualizer.OnRewind();
	}
}