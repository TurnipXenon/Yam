using System.Collections.Generic;

namespace Yam.Core.Rhythm.Chart;

public class Channel: List<Beat>
{
    public int CurrentIndex;
    
    // todo: give current beats to visualize given current time and time frame to visualize
    // todo: how to figure out current time and time frame to visualize??
}