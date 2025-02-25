using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Yam.Core.Rhythm.Chart;
using Yam.Game.Scripts.Rhythm.Game.SingleBeat;

namespace Yam.Game.Scripts.Rhythm.Game.HoldBeat;

public partial class HoldBeat : Node2D, IBeatVisualizer
{
    private Beat _mainBeat;
    private RhythmSimulator _rhythmSimulator;

    /** HoldBeat handles instantiating _endBeat */
    private SingleBeat.SingleBeat _endSingleBeat;

    private bool _isActive = false;
    private readonly List<HoldPiece> _holdPieceList = [];

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
        _endSingleBeat.ReleaseEvent += OnTriggerEndBeat;

        for (var i = 0; i < _mainBeat.BeatList.Count - 1; i++)
        {
            var pooler = i == 0 ? _rhythmSimulator.SingleBeatPooler : _rhythmSimulator.TickPooler;
            var holdPiece = new HoldPiece();
            holdPiece.Initialize(_rhythmSimulator, _mainBeat.BeatList[i], _mainBeat.BeatList[i + 1], pooler, _mainBeat);
            // todo(turnip): set visualizer here?
            AddChild(holdPiece);
            _holdPieceList.Add(holdPiece);

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
            var result = _mainBeat.SimulateHoldingIdleBeat();

            if (result == BeatInputResult.Holding)
            {
                _calculateHoldScore();
            }
        }

        // todo: kill when endBeat reaches beyond
    }

    private int _holdIndex = 0;
    private void _calculateHoldScore()
    {
        var holdPiece = _holdPieceList[_holdIndex];
        if (holdPiece.CalculateScore(_holdIndex == _holdPieceList.Count - 1))
        {
            _holdIndex++;
        }
    }

    // final beat was triggered so everybody got cleared anyway
    private void OnTriggerEndBeat(object sender, EventArgs eventArgs)
    {
        if (_isActive)
        {
            _isActive = false;
            QueueFree();
        }
    }

    public void OnBeatResult(BeatInputResult result, IBeat beat)
    {
        if (_isActive)
        {
            _rhythmSimulator.InvokeBeatResultEvent(this, beat, result);
            _isActive = false;
            _releaseSelf(result);
        }
    }

    private void _releaseSelf(BeatInputResult result)
    {
        // todo: get scoring per piece first before uncommenting?
        // we have a bug where it affects future hold beats and i dont know why it does lol
        _holdPieceList.ForEach(hp => hp.SubmitResult(result));
        _endSingleBeat.Beat.SubmitResult(result);
        QueueFree();
    }

    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
        {
            _endSingleBeat.ReleaseEvent -= OnTriggerEndBeat;
        }
    }
}