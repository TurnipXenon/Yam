using System.Collections.Generic;
using Yam.Core.Rhythm.Models.States;
using Yam.Core.Rhythm.Services.NotePooler;

namespace Yam.Core.Rhythm.Servers;

// the functions are virtual to allow Moq to create a proxy class for it
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
internal class BeatTickPooler
{
	private readonly IPooledNoteResource _resource;
	private readonly List<PooledNote> _inUse = new();
	private readonly Stack<PooledNote> _available = new();
	private bool isUsable = true;

	public BeatTickPooler(IPooledNoteResource resource)
	{
		_resource = resource;
	}

	// todo: change away from beat state
	public virtual PooledNote? RequestBeatTick(BeatState beat)
	{
		if (!isUsable || beat.VisualizationState != VisualizationState.Unowned)
		{
			return null;
		}

		beat.VisualizationState = VisualizationState.Visualized;

		PooledNote newPooledNote;
		if (_available.Count > 0)
		{
			newPooledNote = _available.Pop();
		}
		else
		{
			newPooledNote = _resource.RequestResource();
			newPooledNote.Initialize(this, _resource);
		}

		_inUse.Add(newPooledNote);
		newPooledNote.SetActive(beat);
		return newPooledNote;
	}

	public void Release(PooledNote pooledNote)
	{
		_inUse.Remove(pooledNote);
		_available.Push(pooledNote);
	}

	public void DestroyAllResources()
	{
		isUsable = false; // do not make any new resource
		_inUse.AddRange(_available);
		_available.Clear();
		_inUse.ForEach(b => b.DestroyResource());
		_inUse.Clear();
	}
}