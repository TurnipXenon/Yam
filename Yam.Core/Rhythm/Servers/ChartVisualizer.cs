using Yam.Core.Rhythm.Clients;
using Yam.Core.Rhythm.Models.States;
using Yam.Core.Rhythm.Services;
using Yam.Core.Rhythm.Services.BeatPooler;

namespace Yam.Core.Rhythm.Servers;

internal class ChartVisualizer : IGameListeners
{
	private readonly IRhythmGameHost _host;
	private readonly IChartState _chartState;
	private int currentLowerBound;
	private BeatPooler _pooler;

	// todo: add pooler
	public ChartVisualizer(IRhythmGameHost host, IChartState chartState, BeatPooler pooler)
	{
		_host = host;
		_host.RegisterListener(this);
		_chartState = chartState;
		_pooler = pooler;
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

	public void OnRewind()
	{
		// todo: add test
		// we want to find either the first beat visualized or the first beat that should be destroyed
		currentLowerBound = 0;
		PooledBeat? lastBeat = null;
		while (lastBeat == null || !lastBeat.IsDestroyable())
		{
			var beat = _chartState.GetBeatOrDefault(currentLowerBound);
			if (beat.VisualizationState == VisualizationState.Visualized)
			{
				// it already exists so, let's just skip
				break;
			}

			if (beat.ShouldBePreEmpted(_host.GetPlaybackPosition()))
			{
				lastBeat = _pooler.RequestBeat(beat);
			}
			else if (beat == BeatState.DefaultBeatState)
			{
				// we ran out of beats to show so just break
				break;
			}
		}
	}
}