using System;
using Yam.Core.Rhythm.Models.Wrappers;

namespace Yam.Core.Rhythm.Services.BeatPooler;

/**
 * <code>
 * public class GodotPooledBeat: Node2D, IPooledBeatHost {
 *  PooledBeat pooledBeat;
 *
 *  public override void _Ready() {
 *   pooledBeat = new PooledBeat(this);
 *  }
 * }
 * </code>
 */
public class PooledBeat
{
	private readonly IPooledBeatHost _host;
	private BeatState? _beat;
	private Servers.BeatPooler _beatPooler;

	public PooledBeat(IPooledBeatHost host)
	{
		_host = host;
	}

	internal void SetActive(BeatState beat)
	{
		_beat = beat;
		_host.Activate();
	}

	internal void Initialize(Servers.BeatPooler beatPooler)
	{
		_beatPooler = beatPooler;
	}

	public void Tick()
	{
		if (_beat == null)
		{
			return;
		}

		// todo
	}

	public void Deactivate()
	{
		if (_beat == null)
		{
			// todo: migrate to logging system
			Console.WriteLine("Error: _beat is null when deactivating");
		}
		else
		{
			_beat.VisualizationState = VisualizationState.Unowned;
			_beat = null;
		}

		_host.Deactivate();
		_beatPooler.Release(this);
	}

	public void DestroyResource()
	{
		_host.DestroyResource();
	}
}