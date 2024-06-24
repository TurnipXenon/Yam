using Godot;

namespace Yam.Godot.Scripts.Rhythm;

public partial class AudioHandler : AudioStreamPlayer
{
	private float _currentAudioTime;

	public void SetStream(AudioStream stream)
	{
		Stream = stream;
	}

	public new void Play(float position = -1f)
	{
		var wasRewind = position < _currentAudioTime && position >= 0f;
		base.Play(position < 0f ? _currentAudioTime : position);
		if (wasRewind)
		{
			EmitSignal(SignalName.OnRewind);
		}
	}

	public override void _Process(double delta)
	{
		if (Playing)
		{
			// from https://docs.godotengine.org/en/stable/tutorials/audio/sync_with_audio.html
			_currentAudioTime = (float)(base.GetPlaybackPosition() + AudioServer.GetTimeSinceLastMix() -
			                            AudioServer.GetOutputLatency());
		}
	}

	public new float GetPlaybackPosition()
	{
		return _currentAudioTime;
	}

	public void Pause()
	{
		Stop();
	}

	[Signal]
	public delegate void OnRewindEventHandler();
}