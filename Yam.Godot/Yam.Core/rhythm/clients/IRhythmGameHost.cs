namespace Yam.scenes.rhythm.game;

public interface IRhythmGameHost
{
    float GetAudioPosition();
    void PlaySong(string songPath);
}