namespace Yam.Core.Rhythm.Chart;

public interface IBeatVisualizer
{
    void InformEndResult(BeatInputResult result, IBeat beat);
}