#nullable enable
using System;
using Godot;
using Yam.Core.Common;

namespace Yam.Game.Scripts.Rhythm.Game.SingleBeat;

public class SingleBeatPooler : GenericPooler<SingleBeat, PooledSingleBeatArgs>
{
    private readonly RhythmSimulator _rhythmSimulator;
    private readonly PackedScene _prefab;

    protected override SingleBeat? InstantiatePooledObject(PooledSingleBeatArgs args)
    {
        if (args.Beat.IsVisualized)
        {
            return null;
        }

        var singleBeat = _prefab.Instantiate<SingleBeat>();
        singleBeat.Initialize(args);
        
        singleBeat.RhythmSimulator.Parent.AddChild(singleBeat);
        singleBeat.Pooler = this;
        return singleBeat;
    }

    public SingleBeatPooler(RhythmSimulator rhythmSimulator, PackedScene prefab)
    {
        _rhythmSimulator = rhythmSimulator;
        _prefab = prefab;
    }

    protected override SingleBeat? RevivePooledObject(PooledSingleBeatArgs args)
    {
        if (args.Beat.IsVisualized)
        {
            return null;
        }

        var singleBeat = Available.Pop();
        singleBeat.Initialize(args);
        return singleBeat;
    }

    protected override void DestroyPooledObject(SingleBeat singleObject)
    {
        throw new NotImplementedException();
    }

    protected override void Free(SingleBeat pooledObject)
    {
        InUse.Remove(pooledObject);
        pooledObject.IsActive = false;
        Available.Push(pooledObject);
    }
}
