using System.Collections.Generic;
using Yam.Core.Rhythm.Models.States;
using Yam.Core.Rhythm.Services.BeatPooler;

namespace Yam.Core.Rhythm.Servers;

// the functions are virtual to allow Moq to create a proxy class for it
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
internal class BeatPooler
{
	private readonly IPooledBeatResource _beatResource;
	private readonly List<PooledBeat> _inUse = new();
	private readonly Stack<PooledBeat> _available = new();
	private bool isUsable = true;

	public BeatPooler(IPooledBeatResource beatResource)
	{
		_beatResource = beatResource;
	}

	public virtual PooledBeat? RequestBeat(BeatState beat)
	{
		if (!isUsable || beat.VisualizationState != VisualizationState.Unowned)
		{
			return null;
		}

		beat.VisualizationState = VisualizationState.Visualized;

		PooledBeat newPooledBeatTick;
		if (_available.Count > 0)
		{
			newPooledBeatTick = _available.Pop();
		}
		else
		{
			newPooledBeatTick = _beatResource.RequestResource();
			newPooledBeatTick.Initialize(this, _beatResource);
		}

		_inUse.Add(newPooledBeatTick);
		newPooledBeatTick.SetActive(beat);
		return newPooledBeatTick;
	}

	public void Release(PooledBeat pooledBeatTick)
	{
		_inUse.Remove(pooledBeatTick);
		_available.Push(pooledBeatTick);
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