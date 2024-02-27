using System;

namespace Yam.scenes.rhythm.models;

/// <summary>
/// <c>HitObject</c> (or we could generically as <c>Beat</c>) is the representation
/// of a musical beat that players should react to.
/// </summary>
public class HitObject
{
    public int X;
    public int Y;

    /// <summary>
    /// <c>Timing</c> is the time in milliseconds when the <c>HitObject</c> should be interacted with.
    /// </summary>
    /// <remarks>
    /// <c>Timing</c> is the duration (in milliseconds) from the beginning of the song up to the time that the
    /// <c>HitObject</c> should exactly be interacted with. Or visually, should be displayed (with best effort)
    /// as the exact point in time when the object should be interacted with.
    /// </remarks>
    public int Timing;
    // todo: other properties

    public static HitObject FromOsuHitObjectString(string line)
    {
        // todo: handle error / exceptions
        var components = line.Split(",");
        var hitObject = new HitObject
        {
            Timing = Convert.ToInt32(components[2])
        };
        return hitObject;
    }

    public override string ToString()
    {
        return Timing.ToString();
    }
}