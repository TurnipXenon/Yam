namespace Yam.Core.Rhythm.Models.game;

public interface IRewindListener
{
    void OnAudioRewind(float newAudioPosition);
}