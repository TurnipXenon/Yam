using System.Collections.Generic;
using System.Diagnostics;

namespace Yam.Core.Rhythm.Chart;

/**
 * todo(turnip): better documentation
 * Reflects the JSON structure
 */
public class BeatEntity : TimeUCoordVector
{
    public BeatEntity()
    {
    }

    public BeatEntity(float time)
    {
        Time = time;
    }
    
    public BeatEntity(float time, float uCoord)
    {
        Time = time;
        UCoord = uCoord;
    }
    
    public BeatEntity(List<BeatEntity> tickList)
    {
        Debug.Assert(tickList.Count > 0);
        var initialTick = tickList[0];
        Time = initialTick.Time;
        UCoord = initialTick.UCoord;
        PIn = initialTick.PIn?.Clone();
        POut = initialTick.POut?.Clone();
        tickList.ForEach(t => BeatList.Add(t));
    }

    public TimeUCoordVector? PIn { get; set; }
    public TimeUCoordVector? POut { get; set; }
    public List<BeatEntity> BeatList { get; set; } = new();

    /// <summary>
    /// Clones the fields in the beat but ignores BeatList
    /// </summary>
    /// <returns>BeatEntity</returns>
    public BeatEntity ShallowClone()
    {
        var newBeat = new BeatEntity
        {
            Time = Time,
            UCoord = UCoord,
            PIn = PIn?.Clone(),
            POut = POut?.Clone()
        };
        return newBeat;
    }
}