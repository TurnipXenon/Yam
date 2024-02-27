using System;
using System.Collections.Generic;
using System.Linq;

namespace Yam.scenes.rhythm.models;

public class RhythmData
{
    // todo Timing points

    // todo: HitObjects
    public readonly List<HitObject> HitObjectList = new();

    public override string ToString()
    {
        return string.Join("\n", HitObjectList);
    }
}