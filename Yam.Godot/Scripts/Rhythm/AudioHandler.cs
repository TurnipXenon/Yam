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
		base.Play(position < 0f ? _currentAudioTime : position);
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
}