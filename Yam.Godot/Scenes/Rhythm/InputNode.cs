using Godot;
using Yam.Godot.Scripts.Rhythm;

namespace Yam.Godot.Scenes.Rhythm;

public partial class InputNode : Node
{
	[Export] public RhythmEditorMain SceneManager;

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionReleased("toggle_pause"))
		{
			if (SceneManager.AudioHandler.Playing)
			{
				SceneManager.AudioHandler.Pause();
			}
			else
			{
				SceneManager.AudioHandler.Play();
			}
		}
	}
}