using System.Diagnostics;
using Yam.Core.Rhythm.Chart;

namespace Yam.Core.Test.Utility;

public static class BeatUtil
{
    public static Beat NewHoldBeat_OverlapTest(float startTime, float endTime)
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
    
    public static Beat NewHoldBeat(List<BeatEntity> beatEntityList)
    {
        Debug.Assert(beatEntityList.Count > 0);
        var baseBeat = beatEntityList[0].ShallowClone();
        baseBeat.BeatList = beatEntityList;
        return Beat.FromEntity(baseBeat, Beat.DefaultRelativeReactionWindow);
    }
}