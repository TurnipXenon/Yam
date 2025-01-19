using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using Yam.Core.Common;
using Yam.Core.Rhythm.Input;

namespace Yam.Core.Rhythm.Chart;

public class Beat : TimeUCoordVector, IBeat
{
    public static readonly float DefaultTooEarlyRadius = 1f;
    public static readonly float DefaultOkRadius = 0.75f;
    public static readonly float DefaultGoodRadius = 0.5f;
    public static readonly float DefaultExcellentRadius = 0.2f;
    public static readonly float FrameEpsilon = 1f / 60f;

    // todo(turnip): NOW
    public static readonly List<ReactionWindow> DefaultRelativeReactionWindow = new()
    {
        new ReactionWindow(DefaultExcellentRadius, BeatInputResult.Excellent),
        new ReactionWindow(DefaultGoodRadius, BeatInputResult.Good),
        new ReactionWindow(DefaultOkRadius, BeatInputResult.Ok),
        new ReactionWindow(DefaultTooEarlyRadius, BeatInputResult.TooEarly),
    };

    private enum State
    {
        Waiting,
        Holding,
        Done
    }

    /** Based on 1/75th of an input frame */
    private const float InputEpsilon = (1f / 60f) * 0.75f;

    public TimeUCoordVector? PIn { get; set; }
    public TimeUCoordVector? POut { get; set; }
    public BitwiseDirection Direction { get; set; }
    public List<Beat> BeatList { get; set; } = new();

    private List<ReactionWindow> _reactionWindowList = new();

    public float EndTime => BeatList.Count == 0 ? Time : BeatList.Last().Time;

    #region Beat State

    public bool IsVisualized;
    private State _state = State.Waiting;

    #endregion Beat State

    public static List<ReactionWindow> ReactionWindowsFromRelative(List<ReactionWindow> reactionWindow, float time)
    {
        return reactionWindow
            .Select(rw => new ReactionWindow(rw.Threshold, rw.BeatInputResult, time))
            .ToList();
    }

    public static Beat FromEntity(BeatEntity beatEntity, List<ReactionWindow> reactionWindow)
    {
        var beat = new Beat
        {
            Time = beatEntity.Time,
            UCoord = beatEntity.UCoord,
            PIn = beatEntity.PIn,
            POut = beatEntity.POut,
            _reactionWindowList = ReactionWindowsFromRelative(reactionWindow, beatEntity.Time)
        };

        Debug.Assert(beat._reactionWindowList.Count > 2);

        beatEntity.BeatList.ForEach(childEntity => { beat.BeatList.Add(FromEntity(childEntity, reactionWindow)); });

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
        else
        {
            _beatType = Direction != BitwiseDirection.None
                ? BeatType.Slide
                : BeatType.Single;
        }


        _typeDecided = true;
        return _beatType;
    }

    public Vector2 GetVector()
    {
        return new Vector2(Time, UCoord);
    }

    public BeatInputResult SimulateInput(IRhythmPlayer rhythmPlayer, IRhythmInputProvider inputProvider)
    {
        if (_state == State.Done)
        {
            return BeatInputResult.Done;
        }

        var result = GetBeatType() switch
        {
            BeatType.Single => _simulateSingleBeat(rhythmPlayer, inputProvider),
            BeatType.Slide => BeatInputResult.Ignore,
            BeatType.Hold => BeatInputResult.Ignore,
            _ => throw new ArgumentOutOfRangeException()
        };

        _state = result switch
        {
            BeatInputResult.Idle
                or BeatInputResult.Anticipating => State.Waiting,
            BeatInputResult.Ignore
                or BeatInputResult.Done
                or BeatInputResult.TooEarly
                or BeatInputResult.Miss
                or BeatInputResult.Bad
                or BeatInputResult.Ok
                or BeatInputResult.Good
                or BeatInputResult.Excellent => State.Done,
            BeatInputResult.Holding => State.Holding,
            _ => throw new ArgumentOutOfRangeException()
        };

        return result;
    }

    private BeatInputResult _simulateSingleBeat(IRhythmPlayer rhythmPlayer, IRhythmInputProvider inputProvider)
    {
        // todo: fix window from being based on current time to current beat

        var tooEarlyReaction = _reactionWindowList.Last();
        var okReaction = _reactionWindowList[^2];
        var currentTime = rhythmPlayer.GetCurrentSongTime();

        if (_state != State.Waiting)
        {
            // todo(turnip): add test for this case
            GD.PrintErr($"Not expected result: ({Time}, {UCoord})");
            return BeatInputResult.Ignore;
        }


        if (currentTime < tooEarlyReaction.Range.X)
        {
            return BeatInputResult.Idle;
        }

        if (currentTime >= okReaction.Range.Y)
        {
            _state = State.Done;
            GD.Print($"Missed ({Time}, {UCoord})");
            return BeatInputResult.Miss;
        }

        // find matching input, if there is execute
        // todo: release condition for input is if the beat gives up or the player releases the input
        var gameInputList = inputProvider.GetSingularInputList();

        // find free gameInput and claim it
        var wasInputDetected = false;
        foreach (var gameInput in gameInputList.Where(gameInput => gameInput.GetClaimingChannel() == null))
        {
            gameInput.ClaimOnStart(this);
            wasInputDetected = true;
            break;
        }

        if (wasInputDetected)
        {
            Console.WriteLine("Input detected");
            foreach (var reactionWindow in _reactionWindowList.Where(reactionWindow =>
                         reactionWindow.Range.X < currentTime && currentTime < reactionWindow.Range.Y))
            {
                return reactionWindow.BeatInputResult;
            }
        }

        return BeatInputResult.Anticipating;
    }

    public void InformRelease()
    {
        // todo(turnip)
    }
}