using System;
using Yam.Core.Rhythm.Clients;
using Yam.Core.Rhythm.Services;

namespace Yam.Core.Rhythm.Servers;

internal class ChartVisualizer : IGameListeners
{
	private readonly IRhythmGameHost _host;

	public ChartVisualizer(IRhythmGameHost host)
	{
		this._host = host;
		this._host.RegisterListener(this);
	}

	public void Tick(double delta)
	{
		// todo: create new Beats
	}
}