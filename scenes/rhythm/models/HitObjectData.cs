using System;
using System.Diagnostics;
using Godot;

namespace Yam.scenes.rhythm.models;

/// <summary>
/// <c>HitObject</c> (or we could generically as <c>Beat</c>) is the representation
/// of a musical beat that players should react to.
/// </summary>
public class HitObjectData
{
    /// <summary>
    /// Type of the current <c>HitObject</c> based on osu file format
    /// </summary>
    /// <remarks>
    /// Source: https://osu.ppy.sh/wiki/en/Client/File_formats/osu_%28file_format%29#type
    /// </remarks>
    public enum Type
    {
        Unknown = -2,
        Unset = -1,
        HitCircle = 1,
        Slider = 1 << 1,
        Spinner = 3 << 1,
    }

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
    public ulong Timing;

    public int TypeBit;
    // todo: other properties

    private Type _type = Type.Unset;

    public static HitObjectData FromOsuHitObjectString(string line)
    {
        // todo: handle error / exceptions
        var components = line.Split(",");
        var hitObject = new HitObjectData
        {
            Timing = Convert.ToUInt64(components[2]),
            TypeBit = int.Parse(components[3])
            // 5th one end time for mania
        };
        return hitObject;
    }

    public Type GetHitObjectType()
    {
        if (_type == Type.Unset)
        {
            if ((TypeBit & (int)Type.HitCircle) != 0)
            {
                _type = Type.HitCircle;
            }
            else if ((TypeBit & (int)Type.Slider) != 0)
            {
                _type = Type.Slider;
            }
            else if ((TypeBit & (int)Type.Spinner) != 0)
            {
                _type = Type.Spinner;
            }
            else
            {
                GD.PrintErr($"Unknown beat type at time {Timing} with value {TypeBit}");
                _type = Type.Unknown;
            }
        }

        return _type;
    }

    public override string ToString()
    {
        return Timing.ToString();
    }
}