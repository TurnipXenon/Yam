using System;
using Godot;
using Yam.Core.Rhythm.Chart;
using Yam.Game.Scripts.Rhythm.Game.SingleBeat;

namespace Yam.Game.Scripts.Rhythm.Game.HoldBeat;

public partial class HoldPiece : Node2D
{
    public SingleBeat.SingleBeat VisualStartBeat;
    private Beat _startBeat;
    private Beat _endBeat;
    private float _divisions;
    private float _increment;
    private const float DivisionMultiplier = 0.1f;
    private Vector2 _ogP1;
    private Vector2 _ogP1Out;
    private Vector2 _ogP2;
    private Vector2 _ogP2In;
    private bool _active = true;
    private RhythmSimulator _simulator;
    private Vector2 _p1;
    private Vector2 _p1Out;
    private Vector2 _p2;
    private Vector2 _p2In;

    public void Initialize(RhythmSimulator rhythmSimulator,
        Beat startBeat,
        Beat endBeat,
        SingleBeatPooler pooler,
        Beat parentBeat)
    {
        _simulator = rhythmSimulator;
        _startBeat = startBeat;
        _endBeat = endBeat;
        
        VisualStartBeat = pooler.Request(new PooledSingleBeatArgs()
        {
            Beat = startBeat,
            RhythmSimulator = rhythmSimulator
        });
        VisualStartBeat.ReleaseEvent += OnVisualStartBeatRelease;

        if (VisualStartBeat == null)
        {
            GD.Print($"Failed creating start beat: {startBeat.Time}");
            return;
        }

        // todo: find a better place
        _ogP1 = VisualStartBeat.Beat.GetVector();
        _ogP1.X = SingleBeat.SingleBeat.TimeToX(rhythmSimulator, startBeat.Time);
        if (startBeat.POut != null)
        {
            _ogP1Out = startBeat.POut.ToVector();
            _ogP1Out.X = SingleBeat.SingleBeat.TimeToX(rhythmSimulator, _ogP1Out.X);
        }

        _ogP2 = _endBeat.GetVector();
        _ogP2.X = SingleBeat.SingleBeat.TimeToX(rhythmSimulator, _endBeat.Time);
        if (endBeat.PIn != null)
        {
            _ogP2In = endBeat.PIn.ToVector();
            _ogP2In.X = SingleBeat.SingleBeat.TimeToX(rhythmSimulator, _ogP2In.X);
        }

        var p1DiffVector = _ogP1; // todo: reduce with outward p1 p1_out
        var p2DiffVector = _endBeat.GetVector(); // todo: reduce with inward p2 p2_out
        var pointDiffVector = VisualStartBeat.Beat.GetVector() - _endBeat.GetVector();
        _divisions = (Mathf.Abs(p1DiffVector.X) + Mathf.Abs(p1DiffVector.Y) + Mathf.Abs(p2DiffVector.X) +
                      Mathf.Abs(p2DiffVector.Y) + Mathf.Abs(pointDiffVector.X) + Mathf.Abs(pointDiffVector.Y))
                     * DivisionMultiplier;
        _increment = 1f / _divisions;

        Position = new Vector2(_ogP1.X - SingleBeat.SingleBeat.TimeToX(rhythmSimulator, parentBeat.Time),
            startBeat.UCoord - parentBeat.UCoord);
    }

    private void OnVisualStartBeatRelease(object sender, EventArgs e)
    {
        _active = false;
    }

    private static Vector2 CubicBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        // todo: cache
        var tDiff = 1 - t;
        return (Mathf.Pow(tDiff, 3) * p0)
               + (3f * t * Mathf.Pow(tDiff, 2) * p1)
               + (3f * Mathf.Pow(t, 2) * tDiff * p2)
               + Mathf.Pow(t, 3) * p3;
    }

    private Vector2 CubicBezierCached(float t)
    {
        return CubicBezier(_p1, _p1Out, _p2In, _p2, t);
    }

    public override void _Ready()
    {
        _p1 = Vector2.Zero;
        _p1Out = VisualStartBeat.Beat.POut == null ? Vector2.Zero : _ogP1Out - _ogP1;
        _p2 = _ogP2 - _ogP1;
        _p2In = _endBeat.PIn == null ? _p2 : _ogP2In - _ogP1;
        _lastTime = 0;
    }


    public override void _Draw()
    {
        var prevPoint = CubicBezierCached(_lastTime);
        for (var t = _lastTime + _increment; t < 1f + _increment; t += _increment)
        {
            var nextPoint = CubicBezierCached(t);
            DrawLine(prevPoint, nextPoint, Colors.Green, 8f);
            prevPoint = nextPoint;
        }
    }

    public void SubmitResult(BeatInputResult result)
    {
        if (_active)
        {
            _active = false;
            VisualStartBeat.Beat.SubmitResult(result);
        }
    }

    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
        {
            VisualStartBeat.ReleaseEvent -= OnVisualStartBeatRelease;
        }
    }

    Vector2 _lastPosition = Vector2.Zero;
    private Vector2 _nextPosition;
    private float _totalDistance;
    
    // from 0 to 1
    private float _lastTime;
    
    public bool CalculateScore(bool isLast)
    {
        // convert global position to location position; global time to local time
        var localTime = (_simulator.GetCurrentSongTime() - _startBeat.Time) / (_endBeat.Time - _startBeat.Time);
        var localPosition = ToLocal(_simulator.GetCursorPosition());
        _nextPosition = CubicBezierCached(localTime);
        var positionDifference = Mathf.Abs(localPosition.Y - _nextPosition.Y);
        _endBeat.RecordPositionDifference(positionDifference, Mathf.Abs(_lastTime - localTime));
        _lastTime = localTime;

        QueueRedraw();
        var done = !isLast && localTime > 1f;
        if (done)
        {
            _endBeat.SubmitHoldResult();
        }
        return done;
    }
}