#nullable enable
using System;
using Godot;
using Yam.Core.Common;

namespace Yam.Godot.Scripts.Rhythm.Godot.SingleBeat;

public class SingleBeatPooler : GenericPooler<SingleBeat, PooledSingleBeatArgs>
{
    private readonly RhythmPlayer _rhythmPlayer;
    private readonly PackedScene _prefab;

    protected override SingleBeat? InstantiatePooledObject(PooledSingleBeatArgs args)
    {
        if (args.Beat.Active)
        {
            return null;
        }

        var singleBeat = _prefab.Instantiate<SingleBeat>();
        singleBeat.Initialize(args);
        
        singleBeat.RhythmPlayer.Parent.AddChild(singleBeat);
        singleBeat.Pooler = this;
        return singleBeat;
    }

    public SingleBeatPooler(RhythmPlayer rhythmPlayer, PackedScene prefab)
    {
        _rhythmPlayer = rhythmPlayer;
        _prefab = prefab;
    }

    protected override SingleBeat? RevivePooledObject(PooledSingleBeatArgs args)
    {
        if (args.Beat.Active)
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
