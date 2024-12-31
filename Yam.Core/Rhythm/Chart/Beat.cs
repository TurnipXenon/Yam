using System.Collections.Generic;
using System.Linq;
using Godot;
using Yam.Core.Common;

namespace Yam.Core.Rhythm.Chart;

public class Beat : TimeUCoordVector
{
    /** Based on 1/75th of an input frame */
    private const float InputEpsilon = (1f / 60f) * 0.75f;

    public TimeUCoordVector? PIn { get; set; }
    public TimeUCoordVector? POut { get; set; }
    public BitwiseDirection Direction { get; set; }
    public List<Beat> BeatList { get; set; } = new();

    public float EndTime => BeatList.Count == 0 ? Time : BeatList.Last().Time;
    public bool Active { get; set; }

    public static Beat FromEntity(BeatEntity beatEntity)
    {
        var beat = new Beat
        {
            Time = beatEntity.Time,
            UCoord = beatEntity.UCoord,
            PIn = beatEntity.PIn,
            POut = beatEntity.POut
        };
        beatEntity.BeatList.ForEach(childEntity => { beat.BeatList.Add(FromEntity(childEntity)); });

        // double checking
        if (beat.BeatList.Count > 0)
        {
            var firstBeat = beat.BeatList[0];
            beat.Time = firstBeat.Time;
            beat.UCoord = firstBeat.UCoord;
        }

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

    public Vector2 GetVector()
    {
        return new Vector2(Time, UCoord);
    }
}