using System;
using System.Collections.Generic;
using System.Linq;

namespace Yam.scenes.rhythm.models;

public class RhythmData
{
    // todo: other metadata and difficulty properties
    /// <summary>
    /// <c>ApproachRate</c> is the speed at which the HitObjects move on screen.
    /// </summary>
    /// <remarks>
    /// This property affects, not only the speed, but also the judgment duration.
    /// </remarks>
    public float ApproachRate;
    
    // todo Timing points

    // todo: HitObjects
    public readonly List<HitObject> HitObjectList = new();

    // public static RhythmData FromOsuMapFile(string path)
    // {
    //     
    // }

    public override string ToString()
    {
        return string.Join("\n", HitObjectList);
    }
}