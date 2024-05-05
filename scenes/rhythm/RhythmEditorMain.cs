using System.Diagnostics;
using System.Text.Json;
using Godot;
using Yam.scenes.rhythm.game;
using Yam.scenes.rhythm.logic;
using Yam.scenes.rhythm.models.@base;

public partial class RhythmEditorMain : Node2D, IRhythmGameHost
{
	[Export] public Resource ChartResource { get; set; }
	[Export] public AudioStreamPlayer AudioStreamPlayer { get; set; }

	private ChartEditor _editor;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ParseAndPlayChart();
	}

	private void ParseAndPlayChart()
	{
		Debug.Assert(AudioStreamPlayer != null);
		Debug.Assert(ChartResource != null);
		using var f = FileAccess.Open(ChartResource.ResourcePath, FileAccess.ModeFlags.Read);
		if (f == null)
		{
			GD.PrintErr("TODO: handle error in ReadMap");
			return;
		}

		var chartModel = JsonSerializer.Deserialize<ChartModel>(f.GetAsText());
		chartModel.SetSelfPath(ChartResource.ResourcePath);
		_editor = new ChartEditor
		{
			Host = this
		};
		_editor.Play(chartModel);
	}

	public float GetAudioPosition()
	{
		return AudioStreamPlayer?.GetPlaybackPosition() ?? 0;
	}

	public void PlaySong(string songPath)
	{
		using var file = FileAccess.Open(songPath, FileAccess.ModeFlags.Read);
		var sound = new AudioStreamMP3();
		sound.Data = file.GetBuffer((long)file.GetLength());
		AudioStreamPlayer.Stream = sound;
		AudioStreamPlayer.Play();
	}
}