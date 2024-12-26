using System.Collections.Generic;

namespace Yam.Core.Rhythm.Chart;

/**
 * todo(turnip): improve documentation
 * Reflects the JSON structure
 */
public class ChartEntity
{
    public List<BeatEntity> BeatList { get; set; } = new();
}