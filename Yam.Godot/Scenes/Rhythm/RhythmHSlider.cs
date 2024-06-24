using Godot;
using Yam.Godot.Scripts.Rhythm;

namespace Yam.Godot.Scenes.Rhythm;

public partial class RhythmHSlider : HSlider
{
	[Export] public RhythmEditorMain SceneManager;

	private bool _isDragging;

	public override void _Process(double delta)
	{
		if (!SceneManager.IsReady || _isDragging)
		{
			return;
		}

		Value = (SceneManager.GetPlaybackPosition() / SceneManager.GetStreamLength()) * 100;
	}

	public void OnDragEnded(bool valueChanged)
	{
		SceneManager.AudioStreamPlayer.Play(SceneManager.GetStreamLength() * (float)Value / 100f);
		_isDragging = false;
	}

	public void OnDragStarted()
	{
		SceneManager.AudioStreamPlayer.Stop();
		_isDragging = true;
	}
}