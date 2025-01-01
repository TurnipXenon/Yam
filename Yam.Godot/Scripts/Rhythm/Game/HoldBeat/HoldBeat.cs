using System.Linq;
using Godot;
using Yam.Core.Common;
using Yam.Core.Rhythm.Chart;
using Yam.Godot.Scripts.Rhythm.Game.SingleBeat;

namespace Yam.Godot.Scripts.Rhythm.Game.HoldBeat;

public partial class HoldBeat : Node2D, IBasicListener
{
    private Beat _mainBeat;
    private RhythmPlayer _rhythmPlayer;
    /** HoldBeat handles instantiating _endBeat */
    private SingleBeat.SingleBeat _endSingleBeat;

    private bool _isActive = false;

    public void Initialize(RhythmPlayer rhythmPlayer, Beat mainBeat)
    {
        _rhythmPlayer = rhythmPlayer;
        _mainBeat = mainBeat;
        Name = $"HoldBeat({mainBeat.Time})";
        var endBeat = mainBeat.BeatList.Last();
        _endSingleBeat = _rhythmPlayer.SingleBeatPooler.Request(new PooledSingleBeatArgs()
        {
            Beat = endBeat,
            RhythmPlayer = _rhythmPlayer
        });
        _endSingleBeat.SubscribeToEnd(this);

        for (var i = 0; i < _mainBeat.BeatList.Count - 1; i++)
        {
            var pooler = i == 0 ? _rhythmPlayer.SingleBeatPooler : _rhythmPlayer.TickPooler;
            var holdPiece = new HoldPiece();
            holdPiece.Initialize(_rhythmPlayer, _mainBeat.BeatList[i], _mainBeat.BeatList[i + 1], pooler, _mainBeat);
            AddChild(holdPiece);

            if (i == 0)
            {
                Position = Position with { Y = rhythmPlayer.TriggerPoint.Position.Y + _mainBeat.BeatList[i].UCoord };
            }
        }

        _isActive = true;

    }

    public override void _Process(double delta)
    {
        if (!_isActive)
        {
            return;
        }

        // todo(turnip): cache position
        var x = _rhythmPlayer.SpawnPoint.Position.X
                + ((_rhythmPlayer.GetCurrentSongTime() - (_mainBeat.Time - _rhythmPlayer.PreEmptDuration)) *
                   (_rhythmPlayer.TriggerPoint.Position.X - _rhythmPlayer.SpawnPoint.Position.X))
                / (_rhythmPlayer.PreEmptDuration);

        Position = Position with { X = x };

        // todo: kill when endBeat reaches beyond
    }

    public void Trigger()
    {
        QueueFree();
    }
}