using System;
using System.Numerics;
using Yam.Core.Rhythm.Models.States;

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
	private IPooledBeatResource _hostResource;
	private Vector2 _destructionPoint;
	private Vector2 _triggerPoint;
	private Vector2 _spawningPoint;
	private bool _isLtr;
	private Vector2 _precalculatedLerp;

	public PooledBeat(IPooledBeatHost host)
	{
		_host = host;
	}

	internal void SetActive(BeatState beat)
	{
		_beat = beat;
		_spawningPoint = _hostResource.GetSpawningPoint();
		_triggerPoint = _hostResource.GetTriggerPoint();
		_destructionPoint = _hostResource.GetDestructionPoint();

		// todo: possibly support up and down points
		_isLtr = _triggerPoint.X < _destructionPoint.X;

		// precalculating linear interpolation
		// v = v_spawning + [(v_trigger - v_spawning)/(timing - preempt_time)]*(current_time - preeempt_time)
		// we can precalculate everything inside []
		_precalculatedLerp = (_triggerPoint - _spawningPoint) / _beat.PreemptDuration;

		_host.Activate();
	}

	internal void Initialize(Servers.BeatPooler beatPooler, IPooledBeatResource beatResource)
	{
		_host.Deactivate();
		_hostResource = beatResource;
		_beatPooler = beatPooler;
	}

	// linear interpolation
	// see SetActive to learn the full equation
	private Vector2 GetLerpedPosition => _spawningPoint + _precalculatedLerp
		* (_hostResource.GetPlaybackPosition() - _beat.PreemptTime);

	public void Tick()
	{
		if (_beat == null)
		{
			return;
		}

		var v = GetLerpedPosition;
		_host.SetPosition(v);

		if (_IsDestroyable(v))
		{
			Deactivate();
		}
	}

	private bool _IsDestroyable(Vector2 v)
		=> (_isLtr && v.X > _destructionPoint.X)
		   || (!_isLtr && v.X < _destructionPoint.X);

	// used for rewinding
	public bool IsDestroyable()
	{
		var v = GetLerpedPosition;
		return _IsDestroyable(v);
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