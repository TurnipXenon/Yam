using Yam.Core.Rhythm.Clients;
using Yam.Core.Rhythm.Models.Base;
using Yam.Core.Rhythm.Services;

namespace Yam.Core.Rhythm.Servers;

internal class ChartVisualizer : IGameListeners
{
	private readonly IRhythmGameHost _host;
	private readonly ChartState _chartState;

	// todo: add pooler
	public ChartVisualizer(IRhythmGameHost host, ChartState chartState)
	{
		this._host = host;
		this._host.RegisterListener(this);
		this._chartState = chartState;
	}

	public void Tick(double delta)
	{
		// todo: create new Beats
	}
}