using System.Collections.Generic;

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

    public TimeUCoordVector? PIn { get; set; }
    public TimeUCoordVector? POut { get; set; }
    public List<BeatEntity> BeatList { get; set; } = new();
}