using System.Diagnostics;
using Godot;
using Yam.Core.Rhythm.Input;

namespace Yam.Game.Scripts.Rhythm;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// The max y coordinate is the coordinate of this Node during _Ready
/// </remarks>
public partial class InputVisualizer : Sprite2D
{
    [Export] public Texture2D ActiveImage;
    [Export] public Texture2D InactiveImage;
    [Export] public RhythmSimulator RhythmSimulator;
    [Export] public Node2D TriggerPoint;
    private IRhythmInputProvider _inputProvider;
    private IDirectionInput _directionInput;
    private float _maxY;
    private float _minY;

    public override void _Ready()
    {
        Debug.Assert(ActiveImage != null);
        Debug.Assert(InactiveImage != null);
        Debug.Assert(RhythmSimulator != null);
        Debug.Assert(TriggerPoint != null);

        _inputProvider = RhythmSimulator.InputProvider;
        RhythmSimulator.InputVisualizer = this;
        _directionInput = _inputProvider.GetDirectionInput();
        _minY = TriggerPoint.Position.Y;
        _maxY = Position.Y;
    }

    public override void _Process(double delta)
    {
        SetRotation(_directionInput.GetDirection());
        Position = Position with { Y = Mathf.Clamp(_directionInput.GetCursorPosition().Y, _minY, _maxY) };
    }
}