using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Yam.Godot.Scripts.Rhythm.Dev;

public class ParsedBeatObject : IParsedTick
{
    public List<IParsedTick> parsedTickList = new();

    public ParsedBeatObject(string startTime, string endTime)
    {
        parsedTickList.Add(new ParsedTick(startTime));
        parsedTickList.Add(new ParsedTick(endTime.Split(':')[0]));
    }

    public float GetTime()
    {
        if (parsedTickList.Count == 0)
        {
            return -1;
        }

        return parsedTickList[0].GetTime();
    }

    public bool IsComplex()
    {
        return true;
    }

    public bool TryContainTick(IParsedTick parsedTick)
    {
        var isContained = parsedTickList[0].GetTime() < parsedTick.GetTime()
                          && parsedTick.GetTime() < parsedTickList.Last().GetTime();

        if (isContained)
        {
            for (var i = 0; i < parsedTickList.Count; i++)
            {
                if (parsedTickList[i].GetTime() > parsedTick.GetTime())
                {
                    parsedTickList.Insert(i, parsedTick);
                    break;
                }
                
            }
        }

        return isContained;
    }

    public List<IParsedTick> GetTickList()
    {
        return parsedTickList;
    }
}

// todo: extract class to a new file but put here for now
public class ParsedTick : IParsedTick
{
    public float Time;

    public ParsedTick(string time)
    {
        Time = float.Parse(time, CultureInfo.InvariantCulture);
    }

    public float GetTime()
    {
        return Time;
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

public interface IParsedTick
{
    float GetTime();
    bool IsComplex();
    bool TryContainTick(IParsedTick tick);
    List<IParsedTick> GetTickList();
}