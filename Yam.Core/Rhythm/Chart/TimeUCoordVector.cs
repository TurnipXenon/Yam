using System;
using Godot;
using Yam.Core.Common;

namespace Yam.Core.Rhythm.Chart;

/**
 * todo(turnip): better documentation
 * Reflects the JSON structure
 */
public class TimeUCoordVector
{
    public float Time { get; set; }
    public float UCoord { get; set; }

    public Vector2 ToVector()
    {
        return new Vector2(Time, UCoord);
    }

    public TimeUCoordVector Clone()
    {
        return new TimeUCoordVector
        {
            Time = Time,
            UCoord = UCoord
        };
    }

    public bool CompareTimeUCoord(TimeUCoordVector other)
    {
        return Math.Abs(Time - other.Time) < Globals.FrameEpsilon &&
               Math.Abs(UCoord - other.UCoord) < Globals.FrameEpsilon;
    }
}