using System.Collections.Generic;
using System.Linq;

namespace Yam.Game.Scripts.Rhythm.Dev;

public class ParsedBeatObject : IParsedTick
{
    private readonly List<IParsedTick> _parsedTickList = new();

    public ParsedBeatObject(string startTime, string endTime)
    {
        _parsedTickList.Add(new ParsedTick(startTime, "0"));
        _parsedTickList.Add(new ParsedTick(endTime.Split(':')[0], "0"));
    }

    public float GetTime()
    {
        if (_parsedTickList.Count == 0)
        {
            return -1;
        }

        return _parsedTickList[0].GetTime();
    }

    public float GetUCoord()
    {
        if (_parsedTickList.Count == 0)
        {
            return -1;
        }

        return _parsedTickList[0].GetUCoord();
    }

    public bool IsComplex()
    {
        return true;
    }

    public bool TryContainTick(IParsedTick parsedTick)
    {
        var isContained = _parsedTickList[0].GetTime() < parsedTick.GetTime()
                          && parsedTick.GetTime() < _parsedTickList.Last().GetTime();

        if (isContained)
        {
            for (var i = 0; i < _parsedTickList.Count; i++)
            {
                if (_parsedTickList[i].GetTime() > parsedTick.GetTime())
                {
                    _parsedTickList.Insert(i, parsedTick);
                    break;
                }
            }
        }

        return isContained;
    }

    public List<IParsedTick> GetTickList()
    {
        return _parsedTickList;
    }
}