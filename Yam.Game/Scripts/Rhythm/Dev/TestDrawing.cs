using Godot;

namespace Yam.Game.Scripts.Rhythm.Dev;

public partial class TestDrawing : Node2D
{
    [Export] public Vector2 P1 = Vector2.Zero;
    [Export] public Vector2 P1In = new(0, 40);
    [Export] public Vector2 P2 = Vector2.Right * 100;
    [Export] public Vector2 P2Out = new(100, 35);
    [Export] public float DivisionMultiplier = 5;
    [Export] public bool IsOn = true;

    private float _divisions = 100f;
    private float _increment = 1f / 100f;

    public override void _Ready()
    {
        if (!IsOn)
        {
            QueueFree();
        }
        
        if (P1In == Vector2.Zero)
        {
            P1In = P1;
        }

        if (P2Out == Vector2.Zero)
        {
            P2Out = P2;
        }

        var p1DiffVector = P1 - P1In;
        var p2DiffVector = P2 - P2Out;
        var pointDiffVector = P1 - P2;
        _divisions = (Mathf.Abs(p1DiffVector.X) + Mathf.Abs(p1DiffVector.Y) + Mathf.Abs(p2DiffVector.X) +
                      Mathf.Abs(p2DiffVector.Y) + Mathf.Abs(pointDiffVector.X) + Mathf.Abs(pointDiffVector.Y))
                     * DivisionMultiplier;
        _increment = 1f / _divisions;
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
        var prevPoint = P1;
        for (var t = _increment; t < 1f + _increment; t += _increment)
        {
            var nextPoint = CubicBezier(P1, P1In, P2Out, P2, t);
            DrawLine(prevPoint, nextPoint, Colors.Green, 8f);
            prevPoint = nextPoint;
        }
    }

    public override void _Process(double delta)
    {
        Position = Position with { X = ((float)(Position.X + delta) % 200) };
    }
}