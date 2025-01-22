using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using Xunit.Abstractions;
using Yam.Core.Common;
using Yam.Core.Rhythm.Input;

namespace Yam.Core.Rhythm.Chart;

public class Beat : TimeUCoordVector, IBeat
{
    public GameLogger Logger = new();
    
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

    private float _endTime = -10;

    public float EndTime
    {
        get
        {
            if (_endTime < -9)
            {
                _endTime = BeatList.Count == 0 ? Time : BeatList.Last().Time;
            }

            return _endTime;
        }
    }

    #region Beat State

    public bool IsVisualized;
    private State _state = State.Waiting;

    #endregion Beat State

    private IBeatVisualizer? _visualizer;

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
            beat.PIn = firstBeat.PIn;
            beat.POut = firstBeat.POut;
            beat._reactionWindowList = ReactionWindowsFromRelative(reactionWindow, beat.Time);
        }

        return beat;
    }

    public static Beat FromEntity(BeatEntity beatEntity,
        List<ReactionWindow> defaultRelativeReactionWindow,
        ITestOutputHelper? xUnitLogger)
    {
        var beat = FromEntity(beatEntity, defaultRelativeReactionWindow);

        if (xUnitLogger != null)
        {
            beat.Logger.XUnitLogger = xUnitLogger;
            foreach (var child in beat.BeatList)
            {
                child.Logger.XUnitLogger = xUnitLogger;
            }
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

    public BeatInputResult SimulateInput(IRhythmSimulator rhythmSimulator, IRhythmInput playerInput)
    {
        if (_state == State.Done)
        {
            return BeatInputResult.Done;
        }

        var result = GetBeatType() switch
        {
            BeatType.Single => _simulateSingleBeat(rhythmSimulator, playerInput),
            BeatType.Slide => BeatInputResult.Ignore,
            BeatType.Hold => _simulateHoldBeat(rhythmSimulator, playerInput),
            _ => throw new ArgumentOutOfRangeException()
        };

        switch (result)
        {
            case BeatInputResult.Idle or BeatInputResult.Anticipating:
                _state = State.Waiting;
                break;
            case BeatInputResult.Ignore
                or BeatInputResult.Done
                or BeatInputResult.TooEarly
                or BeatInputResult.Miss
                or BeatInputResult.Bad
                or BeatInputResult.Ok
                or BeatInputResult.Good
                or BeatInputResult.Excellent:
                _state = State.Done;
                _visualizer?.InformEndResult(result, this);
                _visualizer = null;
                break;
            case BeatInputResult.Holding:
                _state = State.Holding;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return result;
    }

    private BeatInputResult _simulateSingleBeat(IRhythmSimulator rhythmSimulator, IRhythmInput playerInput)
    {
        // todo: fix window from being based on current time to current beat

        var tooEarlyReaction = _reactionWindowList.Last();
        var okReaction = _reactionWindowList[^2];
        var currentTime = rhythmSimulator.GetCurrentSongTime();

        if (_state != State.Waiting)
        {
            // todo(turnip): add test for this case
            Logger.PrintErr($"Not expected result: ({Time}, {UCoord})");
            return BeatInputResult.Ignore;
        }


        if (currentTime < tooEarlyReaction.Range.X)
        {
            return BeatInputResult.Idle;
        }

        if (currentTime >= okReaction.Range.Y)
        {
            _state = State.Done;
            Logger.Print($"Missed ({Time}, {UCoord})");
            return BeatInputResult.Miss;
        }

        if (playerInput.GetSource() != InputSource.Player
            || playerInput.GetRhythmActionType() != RhythmActionType.Singular)
        {
            return BeatInputResult.Anticipating;
        }

        if (playerInput.ClaimOnStart(this))
        {
            foreach (var reactionWindow in _reactionWindowList.Where(reactionWindow =>
                         reactionWindow.Range.X < currentTime && currentTime < reactionWindow.Range.Y))
            {
                return reactionWindow.BeatInputResult;
            }
        }

        // the detected input does not apply for this beat since it's claimed by another one already
        return BeatInputResult.Anticipating;
    }

    #region Hold

    private int _holdIndex;
    private IRhythmSimulator? _simulator;

    // For hold, we need to store the following information:
    // Hold Start (start)
    // Hold consistency (between each tick)
    // Hold release (final beat or final tick)
    private BeatInputResult _simulateHoldBeat(IRhythmSimulator rhythmSimulator, IRhythmInput playerInput)
    {
        if (_state == State.Waiting)
        {
            return _simulateStartHold(rhythmSimulator, playerInput);
        }

        // todo(turnip): for holding with movement, make sure we are on track

        // detecting release is handled at the bottom, we only handle possible late releases here
        var lastBeat = BeatList.Last();
        var okReaction = lastBeat._reactionWindowList[^2];
        var currentTime = rhythmSimulator.GetCurrentSongTime();
        if (currentTime >= okReaction.Range.Y)
        {
            _state = State.Done;
            Logger.Print($"Missed Hold ({Time}, {UCoord})");
            return BeatInputResult.Miss;
        }

        return BeatInputResult.Holding;
    }


    private BeatInputResult _simulateStartHold(IRhythmSimulator rhythmSimulator, IRhythmInput playerInput)
    {
        var tooEarlyReaction = _reactionWindowList.Last();
        var okReaction = _reactionWindowList[^2];
        var currentTime = rhythmSimulator.GetCurrentSongTime();

        if (_state != State.Waiting)
        {
            // todo(turnip): add test for this case
            Logger.PrintErr($"Not expected result: ({Time}, {UCoord})");
            return BeatInputResult.Ignore;
        }


        if (currentTime < tooEarlyReaction.Range.X)
        {
            return BeatInputResult.Idle;
        }

        if (currentTime >= okReaction.Range.Y)
        {
            _state = State.Done;
            Logger.Print(
                $"Missed hold start ({Time}, {UCoord}). OkRange ends at {okReaction.Range.Y}. Current time is {currentTime}");
            return BeatInputResult.Miss;
        }

        if (playerInput.GetSource() != InputSource.Player
            && playerInput.GetRhythmActionType() != RhythmActionType.Singular)
        {
            return BeatInputResult.Anticipating;
        }

        if (playerInput.GetClaimingChannel() == null && playerInput.ClaimOnStart(this))
        {
            foreach (var reactionWindow in _reactionWindowList.Where(reactionWindow =>
                         reactionWindow.Range.X < currentTime && currentTime < reactionWindow.Range.Y))
            {
                // we need reference to this for the release time
                _simulator = rhythmSimulator;

                // todo(turnip): inform initial beat of the result and animate
                var result = reactionWindow.BeatInputResult;
                // todo: think of how visualizing works later
                _visualizer?.InformEndResult(result, this);
                _visualizer = null;
                Logger.Print("Start hold");
                return BeatInputResult.Holding;
            }
        }

        // the detected input does not apply for this beat since it's claimed by another one already
        return BeatInputResult.Anticipating;
    }

    #endregion Hold

    public void SetVisualizer(IBeatVisualizer visualizer)
    {
        _visualizer = visualizer;
    }

    // todo: delete this variable when we find a better way to communicate a release to the hold beat visualizer
    // then we can have a mock listening for the result of how this beat ends
    public BeatInputResult HoldReleaseResult;

    public void OnInputRelease()
    {
        var currentTime = _simulator?.GetCurrentSongTime();
        if (currentTime == null)
        {
            return;
        }

        var lastReactionWindow = BeatList.Last()._reactionWindowList;
        foreach (var reactionWindow in lastReactionWindow.Where(reactionWindow =>
                     reactionWindow.Range.X < currentTime && currentTime < reactionWindow.Range.Y))
        {
            // todo(turnip): inform initial beat of the result and animate
            var result = reactionWindow.BeatInputResult;
            Logger.Print($"Release: {result}");
            // todo: figure out which visualizer we should call??? the hold beat???
            // _visualizer?.InformEndResult(result, this);
            // _visualizer = null;
            _state = State.Done;
            HoldReleaseResult = result;
            // todo: inform beat channel next???
            return;
        }

        HoldReleaseResult = BeatInputResult.Miss;
        Logger.Print("Release too late!");
        _state = State.Done;
    }
}