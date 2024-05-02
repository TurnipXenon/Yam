using System.Collections.Generic;
using Godot;

namespace Yam.scenes.rhythm.models.game;

public record ChartEditorPlayer
{
    public Vector2 MousePosition { get; set; }
    public List<IInputListener> InputListeners { get; set; } = new();
    public List<IRewindListener> RewindListeners { get; set; } = new();
}