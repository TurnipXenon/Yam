using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.NativeInterop;
using Yam.Core.Common;
using Yam.Core.Rhythm.Chart;

namespace Yam.Core.Rhythm.Input;

public class MultiHoldInput : IRhythmInput
{
    public const float VisualEpsilon = 0.25f;

    private readonly IRhythmSimulator _simulator;
    private readonly List<IRhythmInput> _incativeList = [];
    private readonly List<IRhythmInput> _activeList = [];
    private readonly float _startingTime;

    public MultiHoldInput(IRhythmSimulator rhythmSimulator, IRhythmInput firstInput)
    {
        _simulator = rhythmSimulator;
        _incativeList.Add(firstInput);

        _startingTime = _simulator.GetCurrentSongTime();
    }

    public bool IsValidDirection()
    {
        return false;
    }

    public bool IsDirectionSensitive()
    {
        return false;
    }

    public Vector2 GetDirection()
    {
        return Vector2.Zero;
    }

    public string GetInputCode()
    {
        // todo(turnip): do we need to know?
        return "multi";
    }

    public IBeat? GetClaimingChannel(IBeat contextualBeat)
    {
        // todo: we need to make unit test here
        if (_activeList.Exists(i => i.GetClaimingChannel(contextualBeat) == contextualBeat))
        {
            return contextualBeat;
        }

        return _activeList.Count > 0
            ? _activeList[0].GetClaimingChannel(contextualBeat)
            : null;
    }

    public bool ClaimOnStart(IBeat beat)
    {
        for (var index = 0; index < _incativeList.Count; index++)
        {
            var rhythmInput = _incativeList[index];
            if (rhythmInput.ClaimOnStart(beat))
            {
                _incativeList.Remove(rhythmInput);
                _activeList.Add(rhythmInput);
                return true;
            }
        }

        return false;
    }

    public void ReleaseInput()
    {
        // todo: we might delete this because it's not even used???
        throw new NotImplementedException();
    }

    public InputSource GetSource()
    {
        return InputSource.Game;
    }

    public RhythmActionType GetRhythmActionType()
    {
        return RhythmActionType.Singular;
    }

    public void Activate()
    {
        // exclusive on single input
        throw new NotImplementedException();
    }

    public void Release()
    {
        // exclusive on single input
        throw new NotImplementedException();
    }

    public void AddInput(IRhythmInput realInput)
    {
        _incativeList.Add(realInput);
    }

    /// <summary>
    /// Returns true when it's still going to accept inputs to handle
    /// </summary>
    /// <returns>bool</returns>
    public bool IsStillAcceptingInputs()
    {
        return _simulator.GetCurrentSongTime() < _startingTime + VisualEpsilon;
    }
}