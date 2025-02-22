using Godot;

namespace Yam.Core.Rhythm.Chart;

public interface IBeatVisualizer
{
    void OnBeatResult(BeatInputResult result, IBeat beat);
    Vector2 GetPosition();
    float GetWeightedDistance();
}