#nullable enable
using System;
using Yam.Core.Common;

namespace Yam.Godot.Scripts.Rhythm.Godot.SingleBeat;

public class SingleBeatPooler : GenericPooler<SingleBeat, SinglePooledBeatArgs>
{
    private readonly RhythmPlayer _rhythmPlayer;

    protected override SingleBeat? InstantiatePooledObject(SinglePooledBeatArgs args)
    {
        if (args.Beat.Active)
        {
            return null;
        }

        var singleBeat = _rhythmPlayer.SingleBeatPrefab.Instantiate<SingleBeat>();
        args.Beat.Active = true;
        singleBeat.RhythmPlayer = args.RhythmPlayer;
        singleBeat.Beat = args.Beat;
        singleBeat.IsActive = true;
        singleBeat.RhythmPlayer.Parent.AddChild(singleBeat);
        return singleBeat;
    }

    public SingleBeatPooler(RhythmPlayer rhythmPlayer)
    {
        _rhythmPlayer = rhythmPlayer;
    }

    protected override SingleBeat? RevivePooledObject(SinglePooledBeatArgs args)
    {
        if (args.Beat.Active)
        {
            return null;
        }

        var singleBeat = Available.Pop();
        args.Beat.Active = true;
        singleBeat.RhythmPlayer = args.RhythmPlayer;
        singleBeat.Beat = args.Beat;
        singleBeat.IsActive = true;
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
