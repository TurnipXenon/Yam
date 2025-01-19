#nullable enable
using System;
using Godot;
using Yam.Core.Rhythm.Chart;

namespace Yam.Core.Rhythm.Input;

// todo(turnip): refactor code to remove duplicates
// note to self: knows the implementation?
// todo(turnip): create ISingularInput. this is a keyboard implementation
// todo(turnip): OnStarted, OnHold, OnRelease (document differences)
public class KeyboardSingularInput : ISingularInput
{
    // todo(turnip): see if we can combine them into the details class below
    private IBeat? _claimingBeat;
    private string _keyCode;

    public KeyboardSingularInput(string keyCode)
    {
        _keyCode = keyCode;
    }

    public bool IsValidDirection()
    {
        return false;
    }

    public bool IsDirectionSensitive() => false;

    public Vector2 GetDirection()
    {
        return Vector2.Zero;
    }

    public string GetInputCode()
    {
        return _keyCode;
    }

    public IBeat? GetClaimingChannel() => _claimingBeat;

    public bool ClaimOnStart(IBeat claimingBeat)
    {
        if (_claimingBeat != null || _singularInputState != SingularInputState.Started)
        {
            return false;
        }

        _claimingBeat = claimingBeat;
        return true;
    }

    public void ReleaseInput()
    {
        _claimingBeat = null;
    }

    public InputSource GetSource()
    {
        return InputSource.Game;
    }

    public RhythmActionType GetRhythmActionType()
    {
        return RhythmActionType.Singular;
    }

    private SingularInputState _singularInputState = SingularInputState.Free;

    public void Press()
    {
        _singularInputState = _singularInputState switch
        {
            SingularInputState.Free => SingularInputState.Started,
            SingularInputState.Started or SingularInputState.Held => SingularInputState.Held,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public SingularInputState GetState()
    {
        return _singularInputState;
    }

    public void Release()
    {
        _claimingBeat?.InformRelease();
    }
}