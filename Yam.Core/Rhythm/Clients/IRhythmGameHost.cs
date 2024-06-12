namespace Yam.Core.Rhythm.Clients;

public interface IRhythmGameHost
{
    float GetAudioPosition();
    void PlaySong(string songPath);
}