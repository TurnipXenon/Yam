namespace Yam.Core.Rhythm.Chart;

public interface IBeat
{
    void InformRelease();
    void SetVisualizer(IBeatVisualizer visualizer);
}