using Yam.scenes.rhythm.game;
using Yam.scenes.rhythm.models.@base;

namespace Yam.scenes.rhythm.logic;

public class ChartEditor
{
	public IRhythmGameHost Host;
	private ChartModel _chartModel;

	public void Play(ChartModel chartModel)
	{
		_chartModel = chartModel;
		Host.PlaySong($"{_chartModel.SelfPath}/{_chartModel.AudioRelativePath}");
	}
}