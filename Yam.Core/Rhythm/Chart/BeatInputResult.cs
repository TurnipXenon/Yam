using System;

namespace Yam.Core.Rhythm.Chart;

[Flags]
public enum BeatInputResult
{
    Idle,
    TooEarly,
    Miss,
    Bad,
    // todo(turnip): investigate whether anticipate is redundant
    Anticipating,
    Ok,
    Good,
    Excellent,
    Holding,
    Ignore,
    Done
}