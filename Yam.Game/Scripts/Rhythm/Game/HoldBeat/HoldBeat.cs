using System.Linq;
using Godot;
using Yam.Core.Common;
using Yam.Core.Rhythm.Chart;
using Yam.Game.Scripts.Rhythm.Game.SingleBeat;

namespace Yam.Game.Scripts.Rhythm.Game.HoldBeat;

public partial class HoldBeat : Node2D, IBasicListener, IBeatVisualizer
{
    private Beat _mainBeat;
    private RhythmSimulator _rhythmSimulator;

    /** HoldBeat handles instantiating _endBeat */
    private SingleBeat.SingleBeat _endSingleBeat;

    private bool _isActive = false;

    public void Initialize(RhythmSimulator rhythmSimulator, Beat mainBeat)
    {
        _rhythmSimulator = rhythmSimulator;
        _mainBeat = mainBeat;
        Name = $"HoldBeat({mainBeat.Time})";
        var endBeat = mainBeat.BeatList.Last();
        _endSingleBeat = _rhythmSimulator.SingleBeatPooler.Request(new PooledSingleBeatArgs()
        {
            Beat = endBeat,
            RhythmSimulator = _rhythmSimulator
        });
        _endSingleBeat.SubscribeToEnd(this);

        for (var i = 0; i < _mainBeat.BeatList.Count - 1; i++)
        {
            var pooler = i == 0 ? _rhythmSimulator.SingleBeatPooler : _rhythmSimulator.TickPooler;
            var holdPiece = new HoldPiece();
            holdPiece.Initialize(_rhythmSimulator, _mainBeat.BeatList[i], _mainBeat.BeatList[i + 1], pooler, _mainBeat);
            // todo(turnip): set visualizer here?
            AddChild(holdPiece);

            if (i == 0)
            {
                Position = Position with { Y = rhythmSimulator.TriggerPoint.Position.Y + _mainBeat.BeatList[i].UCoord };
            }
        }

        _mainBeat.Visualizer = this;
        _isActive = true;
    }

    public override void _Process(double delta)
    {
        if (!_isActive)
        {
            return;
        }

        // todo(turnip): cache position
        var x = _rhythmSimulator.SpawnPoint.Position.X
                + ((_rhythmSimulator.GetCurrentSongTime() - (_mainBeat.Time - _rhythmSimulator.PreEmptDuration)) *
                   (_rhythmSimulator.TriggerPoint.Position.X - _rhythmSimulator.SpawnPoint.Position.X))
                / (_rhythmSimulator.PreEmptDuration);

        Position = Position with { X = x };

        if (_mainBeat.GetState() == Beat.State.Holding)
        {
            _mainBeat.SimulateHoldingIdleBeat();
        }

        // todo: kill when endBeat reaches beyond
    }

    public void Trigger()
    {
        QueueFree();
    }

    public void OnBeatResult(BeatInputResult result, IBeat beat)
    {
        _rhythmSimulator.InvokeBeatResultEvent(this, beat, result);
    }
}