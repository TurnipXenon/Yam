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

    public const float DefaultTooEarlyRadius = 0.4f;
    public const float DefaultOkRadius = 0.3f;
    public const float DefaultGoodRadius = 0.2f;
    public const float DefaultExcellentRadius = 0.1f;

    public const float ExcellentHoldDistanceLimit = 64f;
    public const float GoodHoldDistanceLimit = 160f;
    public const float OkHoldDistanceLimit = 256f;

    public const float DirectionEpsilon = Mathf.Pi / 32;
    public const float LeastWrongDirectionDifference = Mathf.Pi / 4;
    public const float DefaultDirectionTolerance = LeastWrongDirectionDifference - DirectionEpsilon;

    // todo(turnip): NOW
    public static readonly List<ReactionWindow> DefaultRelativeReactionWindow = new()
    {
        new ReactionWindow(DefaultExcellentRadius, BeatInputResult.Excellent),
        new ReactionWindow(DefaultGoodRadius, BeatInputResult.Good),
        new ReactionWindow(DefaultOkRadius, BeatInputResult.Ok),
        new ReactionWindow(DefaultTooEarlyRadius, BeatInputResult.TooEarly),
    };

    public enum State
    {
        Waiting,
        Holding,
        Done
    }

    /** Based on 1/75th of an input frame */
    private const float InputEpsilon = (1f / 60f) * 0.75f;

    public TimeUCoordVector? PIn { get; set; }
    public TimeUCoordVector? POut { get; set; }

    /// <summary>
    /// radian
    /// </summary>
    /// todo(turnip): better documentation
    public float? Direction { get; set; }

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
    public State GetState() => _state;

    #endregion Beat State

    public IBeatVisualizer? Visualizer;

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

        if (beatEntity.Direction != null)
        {
            beat.Direction = Mathf.DegToRad((float)beatEntity.Direction);

            while (beat.Direction > Mathf.Pi)
            {
                beat.Direction -= 2 * Mathf.Pi;
            }
        }

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
            _beatType = Direction != null
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

        BeatInputResult result = GetBeatType() switch
        {
            BeatType.Single => _simulateSingleBeat(rhythmSimulator, playerInput),
            BeatType.Slide => _simulateSlideBeat(rhythmSimulator, playerInput),
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
                SubmitResult(result);
                break;
            case BeatInputResult.Holding:
                _state = State.Holding;
                _state = State.Holding;
                HoldReleaseResult = BeatInputResult.Holding;
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

    // private int _holdIndex = 1;
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

        return SimulateHoldingIdleBeat();
    }

    private bool _isFullyStraightChecked;
    private bool _isFullyStraight = true;

    public bool IsFullyStraight()
    {
        if (_isFullyStraightChecked)
        {
            return _isFullyStraight;
        }

        _isFullyStraightChecked = true;

        if (GetBeatType() != BeatType.Hold)
        {
            return _isFullyStraight;
        }

        // _isFullyStraight = !BeatList.Exists(beat => beat.PIn != null || beat.POut != null);
        for (var index = 1; index < BeatList.Count; index++)
        {
            var previousBeat = BeatList[index - 1];
            var nextBeat = BeatList[index];

            if (Math.Abs(previousBeat.UCoord - nextBeat.UCoord) > 0.001)
            {
                // they're found not equal
                _isFullyStraight = false;
                break;
            }

            if (previousBeat.PIn != null
                || previousBeat.POut != null
                || nextBeat.PIn != null
                || nextBeat.POut != null)
            {
                _isFullyStraight = false;
                break;
            }
        }

        // todo(turnip): add test
        // todo: only applicable for hold beats, throw if not hold beat
        return _isFullyStraight;
    }

    public BeatInputResult SimulateHoldingIdleBeat()
    {
        if (_simulator == null)
        {
            // todo: inform signal about state change and result?
            _state = State.Done;
            Logger.Print("Simulator is null when simulating idle");
            HoldReleaseResult = BeatInputResult.Done;
            return BeatInputResult.Ignore;
        }

        // detecting release is handled at the bottom, we only handle possible late releases here
        var lastBeat = BeatList.Last();
        var okReaction = lastBeat._reactionWindowList[^2];
        var currentTime = _simulator.GetCurrentSongTime();
        if (currentTime >= okReaction.Range.Y)
        {
            // todo: inform signal about state change and result?
            // todo: make multiholdinput listen!
            _state = State.Done;
            Logger.Print($"Missed Hold release ({Time}, {UCoord})");
            HoldReleaseResult = BeatInputResult.Miss;
            return BeatInputResult.Miss;
        }

        // todo(turnip): add test?
        // todo(turnip): for holding with movement, make sure we are on track
        // todo(turnip): which beat are we assessing?
        // var beatToAssess = BeatList[_holdIndex];
        // var initialBeat = BeatList[_holdIndex - 1];
        // beatToAssess.SimulateMovingHoldBeat(initialBeat, beatToAssess == lastBeat, IsFullyStraight());
        // todo(turnip): only increment if we are not at the last beat

        // todo: inform signal about state change and result?
        return BeatInputResult.Holding;
    }


    // private float _averageDistance = 0;
    // public void SimulateMovingHoldBeat(Beat initialBeat, bool isLast, bool isFullyStraight)
    // {
    //     // todo(turnip): add test
    //     
    //     
    //     // todo(turnip): determine if straight, if straight you are free to ignore?
    //     // todo(turnip): use hold piece
    //     // var positionDifference = (_simulator.GetCursorPosition() - UCoord);
    //     
    //
    //     // we move the hold index if we are above beattoassess unless it's the end note
    // }
    //
    // private void _refresh()
    // {
    //     // todo: make sure that the beat will not be invoked
    //     // todo: if reaction was not granted then
    //     
    // }


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

        if (playerInput.GetClaimingChannel(this) == null && playerInput.ClaimOnStart(this))
        {
            foreach (var reactionWindow in _reactionWindowList.Where(reactionWindow =>
                         reactionWindow.Range.X < currentTime && currentTime < reactionWindow.Range.Y))
            {
                // we need reference to this for the release time
                _simulator = rhythmSimulator;
                BeatList[0].SubmitResult(reactionWindow.BeatInputResult);
                return BeatInputResult.Holding;
            }
        }

        // the detected input does not apply for this beat since it's claimed by another one already
        return BeatInputResult.Anticipating;
    }


    // todo: delete this variable when we find a better way to communicate a release to the hold beat visualizer
    // then we can have a mock listening for the result of how this beat ends
    public BeatInputResult HoldReleaseResult = BeatInputResult.Idle;

    private BeatInputResult _onInputRelease(bool shouldApply)
    {
        var currentTime = _simulator?.GetCurrentSongTime();
        if (currentTime == null || _state != State.Holding)
        {
            return BeatInputResult.Ignore;
        }

        var lastBeat = BeatList.Last();
        var lastReactionWindow = lastBeat._reactionWindowList;
        foreach (var reactionWindow in lastReactionWindow.Where(reactionWindow =>
                     reactionWindow.Range.X < currentTime && currentTime < reactionWindow.Range.Y))
        {
            var releaseResult = reactionWindow.BeatInputResult;
            var holdResult = lastBeat._calculateHoldResult();
            var finalResult = BeatInputResultUtil.AverageResult(releaseResult, holdResult);
            if (shouldApply)
            {
                // Logger.Print($"({Time}): Release: {releaseResult}; Hold: {holdResult}; Final: {finalResult}");
                _state = State.Done;
                HoldReleaseResult = finalResult;
            }

            // todo: inform beat channel next???
            return finalResult;
        }

        if (shouldApply)
        {
            HoldReleaseResult = BeatInputResult.Miss;
            Logger.Print("Release too late!");
            _state = State.Done;
        }

        return BeatInputResult.Miss;
    }

    public void OnInputRelease()
    {
        var result = _onInputRelease(true);

        if (result is BeatInputResult.TooEarly
            or BeatInputResult.Miss or BeatInputResult.Bad or BeatInputResult.Ok
            or BeatInputResult.Good or BeatInputResult.Excellent or BeatInputResult.Done)
        {
            // Logger.Print($"OnInput: {Visualizer != null}");
            SubmitResult(result, BeatList.Last());
        }
    }


    public BeatInputResult OnSimulateInputRelease()
    {
        return _onInputRelease(false);
    }

    #endregion Hold

    private BeatInputResult _simulateSlideBeat(IRhythmSimulator rhythmSimulator, IRhythmInput playerInput)
    {
        // unlike a normal beat, we wait for the excelled judgment
        // 
        // we might want to accomodate players that want more sensitive judgments. those sensitive
        // judgments are reserved for direction button players and joystick or gamepad players
        var tooEarlyReaction = _reactionWindowList[0];
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
            return BeatInputResult.Miss;
        }


        if (playerInput.GetSource() != InputSource.Player
            || playerInput.GetRhythmActionType() != RhythmActionType.Directional)
        {
            return BeatInputResult.Anticipating;
        }


        var timingResult = BeatInputResult.Anticipating;
        if (playerInput.IsValidDirection())
        {
            foreach (var reactionWindow in _reactionWindowList.Where(reactionWindow =>
                         reactionWindow.Range.X < currentTime && currentTime < reactionWindow.Range.Y))
            {
                timingResult = reactionWindow.BeatInputResult;
                break;
            }
        }

        if (BeatInputResultUtil.GetScore(timingResult) <= 0)
        {
            return timingResult;
        }

        if (playerInput.GetRhythmActionType() != RhythmActionType.Directional)
        {
            return BeatInputResult.Anticipating;
        }

        // we might want to do that once we implement a smarter middleware?
        var radDiff = Mathf.Abs((float)Direction! - playerInput.GetDirection());
        // todo(turnip): decide if we need to add a bad judgment?
        return radDiff < rhythmSimulator.GetDirectionTolerance() ? timingResult : BeatInputResult.Anticipating;
    }

    public void SetVisualizer(IBeatVisualizer visualizer)
    {
        Visualizer = visualizer;
    }

    public IBeatVisualizer? GetVisualizer()
    {
        return Visualizer;
    }

    public void SubmitResult(BeatInputResult result)
    {
        SubmitResult(result, this);
    }

    public void SubmitResult(BeatInputResult result, Beat referenceBeat)
    {
        Visualizer?.OnBeatResult(result, referenceBeat);
        Visualizer = null;
    }

    private float _positionTotal;
    private float _timeTotal;

    public void RecordPositionDifference(float positionDifference, float timeDifference)
    {
        _positionTotal += positionDifference * timeDifference;
        _timeTotal += timeDifference;
        // Logger.Print($"Record called ({Time}) {_positionTotal}");
    }

    private BeatInputResult _calculateHoldResult()
    {
        var weightedTotal = _positionTotal / float.Max(_timeTotal, 1f);
        // Logger.Print($"Weighted total: {_positionTotal} / float.Max({_timeTotal}, 1f) = {weightedTotal}");
        return weightedTotal switch
        {
            < ExcellentHoldDistanceLimit => BeatInputResult.Excellent,
            < GoodHoldDistanceLimit => BeatInputResult.Good,
            < OkHoldDistanceLimit => BeatInputResult.Ok,
            _ => BeatInputResult.Miss
        };
    }

    public void SubmitHoldResult()
    {
        if (Visualizer == null)
        {
            return;
        }

        SubmitResult(_calculateHoldResult());
    }
}