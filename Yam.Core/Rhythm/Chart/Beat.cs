using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Yam.Core.Common;
using Yam.Core.Rhythm.Input;

namespace Yam.Core.Rhythm.Chart;

public class Beat : TimeUCoordVector
{
    private enum State
    {
        Waiting,
        Holding,
        Finished
    }

    /** Based on 1/75th of an input frame */
    private const float InputEpsilon = (1f / 60f) * 0.75f;

    public TimeUCoordVector? PIn { get; set; }
    public TimeUCoordVector? POut { get; set; }
    public BitwiseDirection Direction { get; set; }
    public List<Beat> BeatList { get; set; } = new();

    public float EndTime => BeatList.Count == 0 ? Time : BeatList.Last().Time;

    #region Beat State

    public bool IsVisualized;
    private State _state = State.Waiting;

    #endregion Beat State

    public static Beat FromEntity(BeatEntity beatEntity)
    {
        var beat = new Beat
        {
            Time = beatEntity.Time,
            UCoord = beatEntity.UCoord,
            PIn = beatEntity.PIn,
            POut = beatEntity.POut
        };
        beatEntity.BeatList.ForEach(childEntity => { beat.BeatList.Add(FromEntity(childEntity)); });

        // double checking
        if (beat.BeatList.Count > 0)
        {
            var firstBeat = beat.BeatList[0];
            beat.Time = firstBeat.Time;
            beat.UCoord = firstBeat.UCoord;
        }

        return beat;
    }

    public bool Overlaps(Beat other)
    {
        var otherStart = other.Time - InputEpsilon;
        var otherEnd = other.EndTime + InputEpsilon;
        return this.Time <= otherEnd && otherStart <= this.EndTime;
    }

    private bool _typeDecided;
    private BeatType _beatType;

    public BeatType GetBeatType()
    {
        if (_typeDecided)
        {
            return _beatType;
        }

        if (BeatList.Count > 1)
        {
            _beatType = BeatType.Hold;
        }

        _beatType = Direction != BitwiseDirection.None
            ? BeatType.Slide
            : BeatType.Single;

        _typeDecided = true;
        return _beatType;
    }

    public Vector2 GetVector()
    {
        return new Vector2(Time, UCoord);
    }

    public BeatInputResult SimulateInput(IRhythmPlayer rhythmPlayer, IRhythmInputProvider inputProvider)
    {
        switch (GetBeatType())
        {
            case BeatType.Single:
                return SimulateSingleBeat(rhythmPlayer, inputProvider);
            case BeatType.Slide:
                break;
            case BeatType.Hold:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        GD.PrintErr("Not implemented beat type");
        return BeatInputResult.Ignore;
    }

    private BeatInputResult SimulateSingleBeat(IRhythmPlayer rhythmPlayer, IRhythmInputProvider inputProvider)
    {
        var reactionWindowList = rhythmPlayer.GetReactionWindowList();
        if (!reactionWindowList.Any())
        {
            return BeatInputResult.Waiting;
        }

        if (reactionWindowList.Count < 2)
        {
            GD.PrintErr("Reaction window is less than expected");
            return BeatInputResult.Ignore;
        }

        var missReaction = reactionWindowList.Last();
        var okReaction = reactionWindowList[^2];
        var currentTime = rhythmPlayer.GetCurrentSongTime();

        if (_state != State.Waiting)
        {
            GD.PrintErr($"Not expected result: ({Time},{UCoord})");
            return BeatInputResult.Ignore;
        }
        
        if (currentTime < missReaction.Range.X)
        {
            return BeatInputResult.Waiting;
        }

        if (currentTime >= okReaction.Range.Y)
        {
            _state = State.Finished;
            return BeatInputResult.Miss;
        }
        
        // find matching input, if there is execute
        // todo: release condition for input is if the beat gives up or the player releases the input
        var gameInputList = inputProvider.GetSingularInputList();
        
        // find free gameInput and claim it
        var wasInputDetected = false;
        foreach (var gameInput in gameInputList.Where(gameInput => gameInput.GetClaimingChannel() == null))
        {
            gameInput.ClaimInput(this);
            wasInputDetected = true;
            break;
        }
        
        if (wasInputDetected)
        {
            foreach (var reactionWindow in reactionWindowList.Where(reactionWindow => reactionWindow.Range.X < currentTime && currentTime < reactionWindow.Range.Y))
            {
                GD.PrintErr(reactionWindow.BeatInputResult);
                return reactionWindow.BeatInputResult;
            }
        }

        return BeatInputResult.Anticipating;
    }
}