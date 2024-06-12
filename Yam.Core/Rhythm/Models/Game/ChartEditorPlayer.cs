using System.Collections.Generic;
using System.Numerics;

namespace Yam.Core.Rhythm.Models.game;

public record ChartEditorPlayer
{
	public Vector2 MousePosition { get; set; }
	public List<IInputListener> InputListeners { get; set; } = new();
	public List<IRewindListener> RewindListeners { get; set; } = new();
}