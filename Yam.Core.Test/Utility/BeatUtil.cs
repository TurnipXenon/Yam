using System.Diagnostics;
using Yam.Core.Rhythm.Chart;

namespace Yam.Core.Test.Utility;

public static class BeatUtil
{
    public static Beat NewHoldBeat(float startTime, float endTime)
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

    public static Beat NewSingleBeat(BeatEntity beatEntity)
    {
        return Beat.FromEntity(beatEntity, Beat.DefaultRelativeReactionWindow);
    }
}