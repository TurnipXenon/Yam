namespace Yam.Core.Rhythm.Services;

public interface IAudioPosition
{
    float GetPlaybackPosition();
    float GetStreamLength();
    void OnAudioRewind();
}