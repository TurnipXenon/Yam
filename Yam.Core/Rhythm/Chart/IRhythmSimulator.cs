using System;
using System.Collections.Generic;

namespace Yam.Core.Rhythm.Chart;

public interface IRhythmSimulator
{
    public float GetCurrentSongTime();
    public float GetPreEmptTime();
    public float GetPreEmptDuration();
    public List<ReactionWindow> GetReactionWindowList();
    /// <summary>
    /// radian
    /// </summary>
    /// <returns></returns>
    float GetDirectionTolerance();
    
    // for subscription
    public event EventHandler<BeatResultEvent> BeatSimulationResultEvent;
    
    // for invocation
    void InvokeBeatResultEvent(BeatResultEvent @event);

    void GenerateResultEffect(IBeatVisualizer beatVisualizer, IBeat beat, BeatInputResult result);
}