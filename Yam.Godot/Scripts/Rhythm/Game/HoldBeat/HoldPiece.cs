using Godot;
using Yam.Core.Rhythm.Chart;
using Yam.Godot.Scripts.Rhythm.Game.SingleBeat;

namespace Yam.Godot.Scripts.Rhythm.Game.HoldBeat;

public partial class HoldPiece : Node2D
{
    public SingleBeat.SingleBeat StartBeat;
    private Beat _endBeat;
    private float _divisions;
    private float _increment;
    private const float DivisionMultiplier = 0.1f;
    private Vector2 _ogP1;
    private Vector2 _ogP1Out;
    private Vector2 _ogP2;
    private Vector2 _ogP2In;

    public void Initialize(RhythmPlayer rhythmPlayer,
        Beat startBeat,
        Beat endBeat,
        SingleBeatPooler pooler,
        Beat parentBeat)
    {
        StartBeat = pooler.Request(new PooledSingleBeatArgs()
        {
            Beat = startBeat,
            RhythmPlayer = rhythmPlayer
        });

        if (StartBeat == null)
        {
            GD.PrintErr($"Failed creating start beat: {startBeat.Time}");
            return;
        }

        _endBeat = endBeat;

        // todo: find a better place
        _ogP1 = StartBeat.Beat.GetVector();
        _ogP1.X = SingleBeat.SingleBeat.TimeToX(rhythmPlayer, startBeat.Time);
        if (startBeat.POut != null)
        {
            _ogP1Out = startBeat.POut.ToVector();
            _ogP1Out.X = SingleBeat.SingleBeat.TimeToX(rhythmPlayer, _ogP1Out.X);
        }

        _ogP2 = _endBeat.GetVector();
        _ogP2.X = SingleBeat.SingleBeat.TimeToX(rhythmPlayer, _endBeat.Time);
        if (endBeat.PIn != null)
        {
            _ogP2In = endBeat.PIn.ToVector();
            _ogP2In.X = SingleBeat.SingleBeat.TimeToX(rhythmPlayer, _ogP2In.X);
        }
        
        var p1DiffVector = _ogP1; // todo: reduce with outward p1 p1_out
        var p2DiffVector = _endBeat.GetVector(); // todo: reduce with inward p2 p2_out
        var pointDiffVector = StartBeat.Beat.GetVector() - _endBeat.GetVector();
        _divisions = (Mathf.Abs(p1DiffVector.X) + Mathf.Abs(p1DiffVector.Y) + Mathf.Abs(p2DiffVector.X) +
                      Mathf.Abs(p2DiffVector.Y) + Mathf.Abs(pointDiffVector.X) + Mathf.Abs(pointDiffVector.Y))
                     * DivisionMultiplier;
        _increment = 1f / _divisions;

        Position = new Vector2(_ogP1.X - SingleBeat.SingleBeat.TimeToX(rhythmPlayer, parentBeat.Time),
            startBeat.UCoord);
    }


    private Vector2 CubicBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        // todo: cache
        var tDiff = 1 - t;
        return (Mathf.Pow(tDiff, 3) * p0)
               + (3f * t * Mathf.Pow(tDiff, 2) * p1)
               + (3f * Mathf.Pow(t, 2) * tDiff * p2)
               + Mathf.Pow(t, 3) * p3;
    }


    public override void _Draw()
    {
        var p1 = Vector2.Zero;
        var p1Out = StartBeat.Beat.POut == null ? Vector2.Zero : _ogP1Out - _ogP1;
        var p2 = _ogP2 - _ogP1;
        var p2In = _endBeat.PIn == null ? p2 : _ogP2In - _ogP1;
        var prevPoint = Vector2.Zero;
        for (var t = _increment; t < 1f + _increment; t += _increment)
        {
            var nextPoint = CubicBezier(p1, p1Out, p2In, p2, t);
            DrawLine(prevPoint, nextPoint, Colors.Green, 8f);
            prevPoint = nextPoint;
        }
    }
}