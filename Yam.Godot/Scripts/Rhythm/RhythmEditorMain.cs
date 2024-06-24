using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using Godot;
using Yam.Core.Rhythm.Clients;
using Yam.Core.Rhythm.Models.Base;
using Yam.Core.Rhythm.Services;
using Yam.Core.Rhythm.Services.BeatPooler;
using Vector2 = System.Numerics.Vector2;

namespace Yam.Godot.Scripts.Rhythm;

public partial class RhythmEditorMain : Node2D, IRhythmGameHost, IPooledBeatResource
{
	[Export] public Resource ChartResource { get; set; }
	[Export] public AudioStreamPlayer AudioStreamPlayer { get; set; }
	[Export] public PackedScene GodotPooledBeat { get; set; }
	[Export] public Node2D SpawningPoint { get; set; }
	[Export] public Node2D TriggerPoint { get; set; }
	[Export] public Node2D DestructionPoint { get; set; }

	public bool IsReady { get; private set; }
	private IChartEditor _editor;
	private List<IGameListeners> _listeners = new();
	private float _currentAudioTime;
	private Vector2 _spawningPosition;
	private Vector2 _triggerPosition;
	private Vector2 _destructionPosition;
	private float _streamLength;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var spawningGVector = SpawningPoint.Position;
		_spawningPosition = new Vector2(spawningGVector.X, spawningGVector.Y);
		var triggerGVector = TriggerPoint.Position;
		_triggerPosition = new Vector2(triggerGVector.X, triggerGVector.Y);
		var destructionGVector = DestructionPoint.Position;
		_destructionPosition = new Vector2(destructionGVector.X, destructionGVector.Y);

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
		_editor = ServiceInitializers.CreateEditor(this, this);
		_editor.Play(chartModel);
	}

	public void PlaySong(string songPath)
	{
		GD.Print(songPath);
		using var file = FileAccess.Open(songPath, FileAccess.ModeFlags.Read);
		var sound = new AudioStreamMP3();
		sound.Data = file.GetBuffer((long)file.GetLength());
		AudioStreamPlayer.Stream = sound;
		_streamLength = (float)sound.GetLength();
		AudioStreamPlayer.Play();
		IsReady = true;
	}


	public void RegisterListener(IGameListeners listener)
	{
		this._listeners.Add(listener);
	}

	public override void _Process(double delta)
	{
		if (AudioStreamPlayer.Playing)
		{
			// from https://docs.godotengine.org/en/stable/tutorials/audio/sync_with_audio.html
			_currentAudioTime = (float)(AudioStreamPlayer.GetPlaybackPosition() + AudioServer.GetTimeSinceLastMix() -
			                            AudioServer.GetOutputLatency());
		}

		// this should be after currentAudioTime update
		_listeners.ForEach(l => l.Tick(delta));
	}

	// this is called multiple times during listeners are ticking inside _process so cache the same result for all of
	// them for consistency
	public float GetPlaybackPosition()
	{
		return _currentAudioTime;
	}

	public float GetStreamLength()
	{
		return _streamLength;
	}

	public PooledBeat RequestResource()
	{
		var pooledBeat = GodotPooledBeat.Instantiate<GodotPooledBeat>();
		AddChild(pooledBeat);
		return pooledBeat.PooledBeat;
	}

	public Vector2 GetSpawningPoint()
	{
		return _spawningPosition;
	}

	public Vector2 GetTriggerPoint()
	{
		return _triggerPosition;
	}

	public Vector2 GetDestructionPoint()
	{
		return _destructionPosition;
	}
}