using System.Collections.Generic;
using System.Globalization;
using Godot;

namespace Yam.Game.Scripts.Rhythm.Dev;

public class ParsedTick : IParsedTick
{
    public float Time;
    public float UCoord;

    public ParsedTick(string time, string x)
    {
        Time = float.Parse(time, CultureInfo.InvariantCulture) / 1000f;
        UCoord = Mathf.Round(float.Parse(x, CultureInfo.InvariantCulture) / 103f) * 128f;
    }

    public float GetTime()
    {
        return Time;
    }

    public float GetUCoord()
    {
        return UCoord;
    }

    public bool IsComplex()
    {
        return false;
    }

    public bool TryContainTick(IParsedTick tick)
    {
        return false;
    }

    public List<IParsedTick> GetTickList()
    {
        return new List<IParsedTick>();
    }
}