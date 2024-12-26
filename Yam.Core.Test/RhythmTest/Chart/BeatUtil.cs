using System.Diagnostics;
using Yam.Core.Rhythm.Chart;

namespace Yam.Core.Test.RhythmTest.Chart;

public static class BeatUtil
{
    public static Beat NewBeat(float startTime, float endTime)
    {
        Debug.Assert(startTime < endTime);
        var beat = new Beat
        {
            Time = startTime
        };
        beat.BeatList.Add(new Beat()
        {
            Time = endTime
        });
        return beat;
    }
}