using System.Diagnostics;
using Yam.Core.Rhythm.Clients;
using Yam.Core.Rhythm.Models.Base;

namespace Yam.Core.Rhythm.Logic;

public class ChartEditor
{
	public IRhythmGameHost? Host;
	private ChartModel? _chartModel;

	public void Play(ChartModel? chartModel)
	{
		_chartModel = chartModel;
		Debug.Assert(Host != null);
		Host?.PlaySong($"{_chartModel?.SelfPath}/{_chartModel?.AudioRelativePath}");
	}
}