using Godot;

namespace Yam.Game.Scripts.Rhythm;

public partial class InputVisualizer : Sprite2D
{
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            SetRotation(eventMouseMotion.Relative.Angle());
        }
    }
}