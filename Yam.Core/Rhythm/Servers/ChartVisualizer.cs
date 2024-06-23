using Yam.Core.Rhythm.Clients;
using Yam.Core.Rhythm.Models.Base;
using Yam.Core.Rhythm.Models.Wrappers;
using Yam.Core.Rhythm.Services;

namespace Yam.Core.Rhythm.Servers;

internal class ChartVisualizer : IGameListeners
{
	internal class Props
	{
		internal IRhythmGameHost Host;
		internal ChartState ChartState;
		internal BeatPooler Pooler;
	}

	private readonly IRhythmGameHost _host;
	private readonly ChartState _chartState;
	private int currentLowerBound;
	private BeatPooler _pooler;

	// todo: add pooler
	public ChartVisualizer(Props props)
	{
		_host = props.Host;
		_host.RegisterListener(this);
		_chartState = props.ChartState;
		_pooler = props.Pooler;
	}


	public void Tick(double delta)
	{
		for (var i = 0; i < 5; i++)
		{
			var beat = _chartState.GetBeatOrDefault(currentLowerBound + i);
			if (beat.ShouldBePreEmpted(_host.GetPlaybackPosition()))
			{
				_pooler.RequestBeat(beat);
				if (i == 0)
				{
					// only increase lower bound if the requested beat was the lower bound
					// a beat is lower bound if i = 0
					currentLowerBound++;
					i--;
				}
			}
		}
	}
}