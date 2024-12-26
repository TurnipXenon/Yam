using System.Collections.Generic;

namespace Yam.Core.Rhythm.Chart;

/**
 * todo(turnip): better documentation
 * Reflects the JSON structure
 */
public class BeatEntity
{
    public float Time { get; set; }
    public List<BeatEntity> BeatList { get; set; } = new();
}