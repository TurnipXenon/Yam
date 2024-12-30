using System.Collections.Generic;

namespace Yam.Core.Rhythm.Chart;

public class Channel : List<Beat>
{
    public int CurrentIndex;

    // todo: give current beats to visualize given current time and time frame to visualize


    // todo: how to figure out current time and time frame to visualize??
    // todo: note that currentIndex increases!!!
    /**
     * Note that CurrentIndex increases
     */
    public Beat? TryToGetBeatToVisualize(IRhythmPlayer rhythmPlayer)
    {
        if (CurrentIndex >= Count)
        {
            return null;
        }

        var currentBeat = this[CurrentIndex];
        
        if (rhythmPlayer.GetCurrentSongTime() > currentBeat.Time - rhythmPlayer.GetPreEmptTime()
            && rhythmPlayer.GetCurrentSongTime() < currentBeat.Time + rhythmPlayer.GetPreEmptTime())
        {
            CurrentIndex++;
            return currentBeat;
        }

        return null;
    }
}