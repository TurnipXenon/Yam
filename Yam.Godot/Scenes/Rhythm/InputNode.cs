using Godot;
using Yam.Godot.Scripts.Rhythm;

namespace Yam.Godot.Scenes.Rhythm;

public partial class InputNode : Node
{
	[Export] public RhythmEditorMain SceneManager;

	private float lastTime;

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionReleased("toggle_pause"))
		{
			if (SceneManager.AudioStreamPlayer.Playing)
			{
				lastTime = SceneManager.GetPlaybackPosition();
				SceneManager.AudioStreamPlayer.Stop();
			}
			else
			{
				SceneManager.AudioStreamPlayer.Play(lastTime);
			}
		}
	}
}