namespace Yam.Core.Rhythm.Chart;

public interface IRhythmPlayer
{
    public float GetCurrentSongTime();
    public float GetPreEmptTime();
    public float GetPreEmptDuration();
}