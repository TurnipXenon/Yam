using System.Linq;
using Godot;
using Yam.Core.Rhythm.Chart;

namespace Yam.Game.Scripts.Rhythm;

public partial class TriggerVisualizer : Node2D
{
    private const float Speed = 1_000f;
    private const float ReactTime = 2f;

    private bool _isReady;
    private BeatChannel _channel;
    private RhythmSimulator _simulator;
    private TriggerInitializer _parent;
    private Beat _beat;

    public override void _Process(double delta)
    {
        if (!_isReady)
        {
            return;
        }

        Vector2 targetPosition;
        if (_beat?.GetBeatType() == BeatType.Hold
            && !_beat.IsFullyStraight()
            && _beat.HoldReleaseResult is BeatInputResult.Holding
                or BeatInputResult.Anticipating
                or BeatInputResult.Idle)
        {
            // follow the _direction visualizer
            targetPosition = _simulator.InputVisualizer.Position;
        }
        else
        {
            _beat = _channel.TryToGetBeatForInput();
            if (_beat?.Visualizer == null || _beat.Time - _simulator.GetCurrentSongTime() > ReactTime)
            {
                // follow the busy visualizer
                IsFollowMode = false;
                var preferredVisualizer = _parent.Visualizers.Last();
                foreach (var otherVisualizer in _parent.Visualizers.Where(otherVisualizer =>
                             otherVisualizer.IsFollowMode))
                {
                    preferredVisualizer = otherVisualizer;
                    break;
                }

                targetPosition = preferredVisualizer.Position;
            }
            else
            {
                // follow the designated beat
                IsFollowMode = true;
                targetPosition = Position with { Y = _beat.Visualizer.GetPosition().Y };
            }
        }

        Position = Position.MoveToward(targetPosition, (float)delta * Speed);
    }

    private bool IsFollowMode { get; set; }

    public void Initialize(BeatChannel channel, RhythmSimulator simulator, Node root, TriggerInitializer parent)
    {
        _channel = channel;
        _simulator = simulator;
        _parent = parent;
        _isReady = true;
        root.AddChild(this);
        Position = parent.Position;
    }
}