using System.Diagnostics;
using Xunit.Abstractions;
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

    public static Beat NewSingleBeat(BeatEntity beatEntity, ITestOutputHelper xUnitLogger)
    {
        return Beat.FromEntity(beatEntity, Beat.DefaultRelativeReactionWindow, xUnitLogger);
    }

    public static Beat NewHoldBeat(List<BeatEntity> beatEntityList, ITestOutputHelper xUnitLogger)
    {
        Debug.Assert(beatEntityList.Count > 0);
        var baseBeat = beatEntityList[0].ShallowClone();
        baseBeat.BeatList = beatEntityList;
        return Beat.FromEntity(baseBeat, Beat.DefaultRelativeReactionWindow, xUnitLogger);
    }


    public static Chart NewChart(List<BeatEntity> beatEntityList, ITestOutputHelper xUnitLogger)
    {
        Debug.Assert(beatEntityList.Count > 0);
        return Chart.FromEntity(
            new ChartEntity() { BeatList = beatEntityList },
            Beat.DefaultRelativeReactionWindow,
            xUnitLogger);
    }
}