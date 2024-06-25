using System;
using System.Numerics;
using Yam.Core.Rhythm.Models.States;

namespace Yam.Core.Rhythm.Services.NotePooler;

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
public class PooledNote
{
	private readonly IPooledNoteHost _host;
	private NoteState? _note;
	private Servers.NotePooler _pooler;
	private IPooledNoteResource _hostResource;
	private Vector2 _destructionPoint;
	private Vector2 _triggerPoint;
	private Vector2 _spawningPoint;
	private bool _isLtr;
	private Vector2 _precalculatedLerp;

	public PooledNote(IPooledNoteHost host)
	{
		_host = host;
	}

	// todo: change to tick
	internal void SetActive(NoteState note)
	{
		_note = note;
		_spawningPoint = _hostResource.GetSpawningPoint();
		_triggerPoint = _hostResource.GetTriggerPoint();
		_destructionPoint = _hostResource.GetDestructionPoint();

		// todo: possibly support up and down points
		_isLtr = _triggerPoint.X < _destructionPoint.X;

		// precalculating linear interpolation
		// v = v_spawning + [(v_trigger - v_spawning)/(timing - preempt_time)]*(current_time - preeempt_time)
		// we can precalculate everything inside []
		_precalculatedLerp = (_triggerPoint - _spawningPoint) / _note.PreemptDuration;

		_host.Activate(_note.Type);
	}

	internal void Initialize(Servers.NotePooler beatPooler, IPooledNoteResource beatResource)
	{
		_host.Deactivate();
		_hostResource = beatResource;
		_pooler = beatPooler;
	}


	// linear interpolation
	// see SetActive to learn the full equation
	private Vector2 GetLerpedPosition => _spawningPoint + _precalculatedLerp
		* (_hostResource.GetPlaybackPosition() - _note.PreemptTime);

	public void Tick()
	{
		if (_note == null)
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
		if (_note == null)
		{
			// todo: migrate to logging system
			Console.WriteLine("Error: _beat is null when deactivating");
		}
		else
		{
			_note.VisualizationState = VisualizationState.Unowned;
			_note = null;
		}

		_host.Deactivate();
		_pooler.Release(this);
	}

	public void DestroyResource()
	{
		_host.DestroyResource();
	}
}