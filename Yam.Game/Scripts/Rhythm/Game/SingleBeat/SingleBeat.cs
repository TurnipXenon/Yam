using System.Collections.Generic;
using Godot;
using Yam.Core.Common;
using Yam.Core.Rhythm.Chart;

namespace Yam.Game.Scripts.Rhythm.Game.SingleBeat;

public partial class SingleBeat : Node2D, IBeatVisualizer
{
    public bool IsActive { get; set; }

    private Beat _beat;

    public Beat Beat
    {
        get => _beat;
        set { _beat = value; }
    }

    public RhythmPlayer RhythmPlayer { get; set; }
    public SingleBeatPooler Pooler { get; set; }

    private Stack<IBasicListener> _releaseListeners = new();

    public override void _Process(double delta)
    {
        if (!IsActive)
        {
            return;
        }

        // todo(turnip): cache position
        var x = RhythmPlayer.SpawnPoint.Position.X
                + ((RhythmPlayer.GetCurrentSongTime() - (Beat.Time - RhythmPlayer.PreEmptDuration)) *
                   (RhythmPlayer.TriggerPoint.Position.X - RhythmPlayer.SpawnPoint.Position.X))
                / (RhythmPlayer.PreEmptDuration);

        Position = Position with { X = x };

        if (Position.X > RhythmPlayer.DestructionPoint.Position.X)
        {
            ReleaseSelf();
        }
    }

    private void ReleaseSelf()
    {
        IsActive = false;
        Visible = false;
        Pooler.Release(this);

        while (_releaseListeners.Count > 0)
        {
            _releaseListeners.Pop().Trigger();
        }
    }

    public void SubscribeToEnd(IBasicListener listener)
    {
        _releaseListeners.Push(listener);
    }

    public void Initialize(PooledSingleBeatArgs args)
    {
        args.Beat.IsVisualized = true;
        RhythmPlayer = args.RhythmPlayer;
        Beat = args.Beat;
        args.Beat.SetVisualizer(this);
        Position = Position with { Y = RhythmPlayer.TriggerPoint.Position.Y + _beat.UCoord };
        IsActive = true;
        Name = $"SingleBeat: {Beat.GetVector()}";
        Visible = true;
    }

    public static float TimeToX(RhythmPlayer rhythmPlayer, float time)
    {
        return rhythmPlayer.SpawnPoint.Position.X
               + ((rhythmPlayer.GetCurrentSongTime() - (time - rhythmPlayer.PreEmptDuration)) *
                  (rhythmPlayer.TriggerPoint.Position.X - rhythmPlayer.SpawnPoint.Position.X))
               / (rhythmPlayer.PreEmptDuration);
    }

    public void InformEndResult(BeatInputResult result, IBeat beat)
    {
        // todo(turnip): add effects
        GD.Print("Result received in Godot: ", result.ToString());
        ReleaseSelf();
    }
}