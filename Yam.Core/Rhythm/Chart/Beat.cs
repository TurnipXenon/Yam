using System.Collections.Generic;
using System.Linq;

namespace Yam.Core.Rhythm.Chart;

public class Beat
{
    /** Based on 1/75th of an input frame */
    private const float InputEpsilon = (1f / 60f) * 0.75f;

    public float Time { get; set; }
    public List<Beat> BeatList { get; set; } = new();

    public float EndTime => BeatList.Count == 0 ? Time : BeatList.Last().Time;

    public static Beat FromEntity(BeatEntity beatEntity)
    {
        var beat = new Beat
        {
            Time = beatEntity.Time
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
}