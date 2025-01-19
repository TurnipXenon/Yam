using System;
using System.Collections.Generic;
using Godot;
using Yam.Core.Rhythm.Input;

namespace Yam.Core.Rhythm.Chart;

public class BeatChannel : List<Beat>
{
    private int _currentVisualizationIndex;
    private int _currentInputIndex;

    // todo: give current beats to visualize given current time and time frame to visualize


    // todo: how to figure out current time and time frame to visualize??
    // todo: note that currentIndex increases!!!
    /**
     * Note that CurrentIndex increases
     */
    public Beat? TryToGetBeatToVisualize(IRhythmPlayer rhythmPlayer)
    {
        if (_currentVisualizationIndex >= Count)
        {
            return null;
        }

        var currentBeat = this[_currentVisualizationIndex];
        
        if (rhythmPlayer.GetCurrentSongTime() > currentBeat.Time - rhythmPlayer.GetPreEmptTime()
            && rhythmPlayer.GetCurrentSongTime() < currentBeat.Time + rhythmPlayer.GetPreEmptTime())
        {
            _currentVisualizationIndex++;
            return currentBeat;
        }

        return null;
    }

    private Beat? TryToGetBeatForInput()
    {
        return _currentInputIndex >= Count ? null : this[_currentInputIndex];
    }

    public void SimulateBeatInput(IRhythmPlayer rhythmPlayer, IRhythmInput inputProvider)
    {
        var currentBeat = TryToGetBeatForInput();
        var result = currentBeat?.SimulateInput(rhythmPlayer, inputProvider);

        switch (result)
        {
            case BeatInputResult.Idle:
                break;
            case BeatInputResult.Anticipating:
                break;
            case BeatInputResult.TooEarly:
            case BeatInputResult.Miss:
            case BeatInputResult.Bad:
            case BeatInputResult.Ok:
            case BeatInputResult.Good:
            case BeatInputResult.Excellent:
                GD.Print($"Finished with ({currentBeat!.Time}, {currentBeat.UCoord})");
                _currentInputIndex++;
                break;
            case BeatInputResult.Holding:
                break;
            case BeatInputResult.Ignore:
                GD.Print($"IGNORE: Finished with ({currentBeat!.Time}, {currentBeat.UCoord})");
                _currentInputIndex++;
                break;
            case null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // todo(turnip): send signal if there is a reaction
    }
}