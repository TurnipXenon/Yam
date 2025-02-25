using System;

namespace Yam.Core.Rhythm.Chart;

public static class BeatInputResultUtil
{
    public static int WorstScore = -100;

    public static int GetScore(BeatInputResult? result)
    {
        return result switch
        {
            BeatInputResult.Idle
                or BeatInputResult.Ignore
                or BeatInputResult.Done
                or BeatInputResult.Anticipating
                or BeatInputResult.Holding
                or null => 0,
            BeatInputResult.TooEarly or BeatInputResult.Miss => -1,
            BeatInputResult.Bad => -2,
            BeatInputResult.Ok => 1,
            BeatInputResult.Good => 2,
            BeatInputResult.Excellent => 3,
            _ => throw new ArgumentOutOfRangeException(nameof(result), result, null)
        };
    }
    
    
    public static BeatInputResult AverageResult(BeatInputResult? result1, BeatInputResult? result2)
    {
        var averageScore = (int)float.Round((GetScore(result1) + GetScore(result2)) / 2f);
        return averageScore switch
        {
            1 => BeatInputResult.Ok,
            2 => BeatInputResult.Good,
            3 => BeatInputResult.Excellent,
            _ => BeatInputResult.Miss
        };
    }
}