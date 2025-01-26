using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Yam.Core.Common;
using Yam.Core.Rhythm.Chart;

namespace Yam.Core.Rhythm.Input;

public class MultiHoldInput : IRhythmInput, IBeat
{
    public GameLogger Logger = new();

    public const float VisualEpsilon = 0.25f;

    private readonly IRhythmSimulator _simulator;
    private readonly List<IRhythmInput> _inactiveList = [];
    private readonly List<IRhythmInput> _activeList = [];
    private readonly List<IBeat> _beatList = [];
    private readonly float _startingTime;

    public MultiHoldInput(IRhythmSimulator rhythmSimulator, IRhythmInput firstInput)
    {
        _simulator = rhythmSimulator;
        _inactiveList.Add(firstInput);

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
        if (_beatList.Contains(contextualBeat))
        {
            return contextualBeat;
        }

        return null;
    }

    public bool ClaimOnStart(IBeat beat)
    {
        for (var index = 0; index < _inactiveList.Count; index++)
        {
            var rhythmInput = _inactiveList[index];
            if (rhythmInput.ClaimOnStart(this))
            {
                _inactiveList.Remove(rhythmInput);
                _activeList.Add(rhythmInput);
                _beatList.Add(beat);
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
        return InputSource.Player;
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

    public BeatInputResult? SimulateRelease()
    {
        throw new NotImplementedException();
    }

    public bool AddInput(IRhythmInput realInput)
    {
        if (IsStillAcceptingInputs())
        {
            _inactiveList.Add(realInput);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns true when it's still going to accept inputs to handle
    /// </summary>
    /// <returns>bool</returns>
    public bool IsStillAcceptingInputs()
    {
        return _simulator.GetCurrentSongTime() < _startingTime + VisualEpsilon;
    }

    public void OnInputRelease()
    {
        if (!_beatList.Any())
        {
            Logger.PrintErr("Received onInputRelease event despite having no active inputs");
            return;
        }

        var bestScore = BeatInputResultUtil.WorstScore;
        var bestBeat = _beatList[0];

        foreach (var beat in _beatList)
        {
            var currentScore = BeatInputResultUtil.GetScore(beat.OnSimulateInputRelease());
            if (currentScore > bestScore)
            {
                bestScore = currentScore;
                bestBeat = beat;
            }
        }

        bestBeat.OnInputRelease();
        _beatList.Remove(bestBeat);
    }

    public void SetVisualizer(IBeatVisualizer visualizer)
    {
        throw new NotImplementedException();
    }

    public BeatInputResult OnSimulateInputRelease()
    {
        throw new NotImplementedException();
    }
}