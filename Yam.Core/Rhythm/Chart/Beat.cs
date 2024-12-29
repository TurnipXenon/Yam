using System.Collections.Generic;
using System.Linq;
using Yam.Core.Common;

namespace Yam.Core.Rhythm.Chart;

public class Beat
{
    /** Based on 1/75th of an input frame */
    private const float InputEpsilon = (1f / 60f) * 0.75f;

    public float Time { get; set; }
    public float UCoord { get; set; }
    public BitwiseDirection Direction { get; set; }
    public List<Beat> BeatList { get; set; } = new();

    public float EndTime => BeatList.Count == 0 ? Time : BeatList.Last().Time;
    public bool Active { get; set; }

    public static Beat FromEntity(BeatEntity beatEntity)
    {
        var beat = new Beat
        {
            Time = beatEntity.Time,
            UCoord = beatEntity.UCoord
        };
        beatEntity.BeatList.ForEach(childEntity => { beat.BeatList.Add(FromEntity(childEntity)); });
        return beat;
    }

    public bool Overlaps(Beat other)
    {
        var otherStart = other.Time - InputEpsilon;
        var otherEnd = other.EndTime + InputEpsilon;
        return this.Time <= otherEnd && otherStart <= this.EndTime;
    }

    public BeatType GetBeatType()
    {
        if (BeatList.Count > 1)
        {
            return BeatType.Hold;
        }

        return Direction != BitwiseDirection.None
            ? BeatType.Slide
            : BeatType.Single;
    }
}