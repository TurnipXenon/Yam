using System.Collections.Generic;

namespace Yam.Core.Rhythm.Chart;

public interface IRhythmSimulator
{
    public float GetCurrentSongTime();
    public float GetPreEmptTime();
    public float GetPreEmptDuration();
    public List<ReactionWindow> GetReactionWindowList();
}