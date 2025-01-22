using System;
using Godot;
using Yam.Core.Rhythm.Chart;

namespace Yam.Core.Rhythm.Input;

public class MultiHoldInput : IRhythmInput
{
    // Inspired by the Null Object Pattern
    public static readonly MultiHoldInput GameInput = new(InputSource.Game);
    public static readonly MultiHoldInput UnknownInput = new(InputSource.Unknown);

    private readonly InputSource _source;

    private MultiHoldInput(InputSource source)
    {
        _source = source;
    }

    public bool IsValidDirection()
    {
        throw new NotImplementedException();
    }

    public bool IsDirectionSensitive()
    {
        throw new NotImplementedException();
    }

    public Vector2 GetDirection()
    {
        throw new NotImplementedException();
    }

    public string GetInputCode()
    {
        throw new NotImplementedException();
    }

    public IBeat? GetClaimingChannel()
    {
        throw new NotImplementedException();
    }

    public bool ClaimOnStart(IBeat beat)
    {
        throw new NotImplementedException();
    }

    public void ReleaseInput()
    {
        throw new NotImplementedException();
    }

    public InputSource GetSource()
    {
        return _source;
    }

    public RhythmActionType GetRhythmActionType()
    {
        return RhythmActionType.Invalid;
    }

    public void Activate()
    {
        throw new NotImplementedException();
    }

    public void Release()
    {
        throw new NotImplementedException();
    }
}