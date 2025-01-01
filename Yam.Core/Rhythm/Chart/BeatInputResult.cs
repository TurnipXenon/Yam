using System;

namespace Yam.Core.Rhythm.Chart;

[Flags]
public enum BeatInputResult
{
    Waiting,
    TooEarly,
    Miss,
    Bad,
    Anticipating,
    Ok,
    Good,
    Excellent,
    Holding,
    Ignore
}