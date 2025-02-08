#nullable enable
using System;
using Godot;
using Yam.Core.Common;

namespace Yam.Game.Scripts.Rhythm.Game.SlideBeat;

public class SlideBeatPooler(RhythmSimulator rhythmSimulator, PackedScene prefab)
    : GenericPooler<SlideBeat, PooledSlideBeatArgs>
{
    protected override SlideBeat? InstantiatePooledObject(PooledSlideBeatArgs args)
    {
        if (args.Beat.IsVisualized)
        {
            return null;
        }

        var slideBeat = prefab.Instantiate<SlideBeat>();
        slideBeat.Initialize(args);

        slideBeat.RhythmSimulator.Parent.AddChild(slideBeat);
        slideBeat.Pooler = this;
        return slideBeat;
    }

    protected override SlideBeat? RevivePooledObject(PooledSlideBeatArgs args)
    {
        if (args.Beat.IsVisualized)
        {
            return null;
        }

        var singleBeat = Available.Pop();
        singleBeat.Initialize(args);
        return singleBeat;
    }

    protected override void DestroyPooledObject(SlideBeat singleObject)
    {
        throw new NotImplementedException();
    }

    protected override void Free(SlideBeat pooledObject)
    {
        // todo(turnip): remove when needed
        // InUse.Remove(pooledObject);
        // pooledObject.IsActive = false;
        // Available.Push(pooledObject);
    }
}