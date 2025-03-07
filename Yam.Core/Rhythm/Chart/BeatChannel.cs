using System;
using System.Collections.Generic;
using Xunit.Abstractions;
using Yam.Core.Common;
using Yam.Core.Rhythm.Input;

namespace Yam.Core.Rhythm.Chart;

public class BeatChannel : List<Beat>
{
    public GameLogger Logger = new();

    private int _currentVisualizationIndex;
    private int _currentInputIndex;

    public BeatChannel(ITestOutputHelper? xUnitLogger)
    {
        if (xUnitLogger != null)
        {
            Logger.XUnitLogger = xUnitLogger;
        }
    }

    // todo: give current beats to visualize given current time and time frame to visualize


    // todo: how to figure out current time and time frame to visualize??
    // todo: note that currentIndex increases!!!
    /**
     * Note that CurrentIndex increases
     */
    public Beat? TryToGetBeatToVisualize(IRhythmSimulator rhythmSimulator)
    {
        if (_currentVisualizationIndex >= Count)
        {
            return null;
        }

        var currentBeat = this[_currentVisualizationIndex];

        if (rhythmSimulator.GetCurrentSongTime() > currentBeat.Time - rhythmSimulator.GetPreEmptTime()
            && rhythmSimulator.GetCurrentSongTime() < currentBeat.Time + rhythmSimulator.GetPreEmptTime())
        {
            _currentVisualizationIndex++;
            return currentBeat;
        }

        return null;
    }

    public Beat? TryToGetBeatForInput()
    {
        return _currentInputIndex >= Count ? null : this[_currentInputIndex];
    }


    public void SimulateBeatInput(IRhythmSimulator rhythmSimulator, IRhythmInput playerInput)
    {
        var currentBeat = TryToGetBeatForInput();
        var result = currentBeat?.SimulateInput(rhythmSimulator, playerInput);

        switch (result)
        {
            case BeatInputResult.Idle:
            case BeatInputResult.Anticipating:
                break;
            case BeatInputResult.Holding:
            case BeatInputResult.Done:
            case BeatInputResult.TooEarly:
            case BeatInputResult.Miss:
            case BeatInputResult.Bad:
            case BeatInputResult.Ok:
            case BeatInputResult.Good:
            case BeatInputResult.Excellent:
                Logger.Print($"Finished with ({currentBeat!.Time}, {currentBeat.UCoord}): {result.ToString()}");
                _currentInputIndex++;
                break;
            case BeatInputResult.Ignore:
                Logger.Print($"IGNORE: Finished with ({currentBeat!.Time}, {currentBeat.UCoord})");
                _currentInputIndex++;
                break;
            case null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public float GetLatestInputTime()
    {
        return TryToGetBeatForInput()?.Time ?? 0f;
    }
}