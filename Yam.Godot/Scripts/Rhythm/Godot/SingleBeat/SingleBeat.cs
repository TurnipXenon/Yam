using Godot;
using Yam.Core.Rhythm.Chart;

namespace Yam.Godot.Scripts.Rhythm.Godot.SingleBeat;

public partial class SingleBeat : Node2D
{
    public bool IsActive { get; set; }
    public Beat Beat { get; set; }
    public RhythmPlayer RhythmPlayer { get; set; }
    public SingleBeatPooler Pooler { get; set; }

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
            IsActive = false;
            Pooler.Release(this);
        }
    }
}