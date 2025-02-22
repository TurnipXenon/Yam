using System;
using Godot;
using Yam.Core.Rhythm.Chart;
using Yam.Game.Scripts.Rhythm;

namespace Yam.Game.Scenes.Rhythm.RhythmPlayground.Result;

public partial class ResultLabel : Label
{
    [Export] public float Height = 0.5f;

    /// <summary>
    /// Duration time in milliseconds
    /// </summary>
    [Export] public float Duration = 750f;

    private bool _isReady;
    private float _yMax;
    private float _baseX;
    private ulong _timeStart;
    private Vector2 _basePosition;

    public override void _Process(double delta)
    {
        if (!_isReady)
        {
            return;
        }

        // todo: lerp opacity at 3/4 from 100 to 0 then QueueFree our object

        var timeDiff = Time.GetTicksMsec() - _timeStart;
        var y = Height * CurveCalc(timeDiff, Duration) / _yMax;
        Position = _basePosition + Vector2.Up * y;

        if (timeDiff > Duration)
        {
            QueueFree();
            _isReady = false;
        }
    }

    public void Initialize(RhythmSimulator simulator, BeatInputResult result, IBeatVisualizer beatVisualizer)
    {
        _timeStart = Time.GetTicksMsec();

        // todo: find a way to propagate the beat position from the beat godot object to here
        _basePosition = beatVisualizer.GetPosition();
        switch (result)
        {
            case BeatInputResult.Bad:
                // todo(turnip): consider?????
                GD.PrintErr("ResultLabel.cs: Bad recorded: unexpected result behavior");
                QueueFree();
                return;
            case BeatInputResult.Idle:
            case BeatInputResult.Anticipating:
            case BeatInputResult.Holding:
            case BeatInputResult.Ignore:
            case BeatInputResult.Done:
                // todo(turnip): maybe add additional logs
                QueueFree();
                return;
            case BeatInputResult.TooEarly:
            case BeatInputResult.Miss:
                _basePosition.X = simulator.TriggerPoint.Position.X;
                Text = "Miss";
                break;
            case BeatInputResult.Ok:
                Text = "Ok";
                break;
            case BeatInputResult.Good:
                Text = "Good";
                break;
            case BeatInputResult.Excellent:
                Text = "Perfect";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(result), result, null);
        }

        if (beatVisualizer.GetPosition().X < simulator.TriggerPoint.Position.X)
        {
            _basePosition.X = beatVisualizer.GetPosition().X;
        }

        // todo: remove

        var maxY_x = 3 * Duration / 4;
        _yMax = CurveCalc(maxY_x, Duration);
        simulator.Parent.AddChild(this);
        
        _isReady = true;
    }

    private static float CurveCalc(float time, float duration) =>
        ((time * time) - (3 * duration * time / 2));
}