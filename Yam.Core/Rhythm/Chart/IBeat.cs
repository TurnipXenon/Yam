using Yam.Core.Rhythm.Input;

namespace Yam.Core.Rhythm.Chart;

public interface IBeat: IRhythmInputListener
{
    void SetVisualizer(IBeatVisualizer visualizer);
    BeatInputResult OnSimulateInputRelease();
}