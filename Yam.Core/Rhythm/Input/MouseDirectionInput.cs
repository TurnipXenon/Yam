using System;
using Godot;
using Yam.Core.Common;
using Yam.Core.Rhythm.Chart;

namespace Yam.Core.Rhythm.Input;

// todo(turnip): refactor code to remove duplicates
public class MouseDirectionInput : IDirectionInput
{
    private const string Keycode = "MOUSE_KEYCODE";
    private IBeat? _claimingBeat;
    private float _direction;

    /// <summary>
    /// Direction's validity time to live which is measured in <b>frames</b>
    /// </summary>
    private double _directionTtl = 0;

    public bool IsValidDirection()
    {
        return _directionTtl > 0;
    }

    public bool IsDirectionSensitive() => true;

    public float GetDirection()
    {
        return _direction;
    }

    public string GetInputCode()
    {
        return Keycode;
    }

    // todo(turnip): we release claiming beat only if we release the singular beats
    // this is auto released when we dont have singular inputs
    public IBeat? GetClaimingChannel(IBeat contextualBeat) => _claimingBeat;

    public bool ClaimOnStart(IBeat claimingBeat)
    {
        if (!IsValidDirection() || _claimingBeat != null || _singularInputState != SingularInputState.Started)
        {
            return false;
        }

        _claimingBeat = claimingBeat;
        return true;
    }


    public InputSource GetSource()
    {
        return InputSource.Player;
    }

    public RhythmActionType GetRhythmActionType()
    {
        return RhythmActionType.Directional;
    }

    private SingularInputState _singularInputState = SingularInputState.Free;
    private Vector2 _position;

    // todo(turnip): might be released when listening to a singular input that was claimed
    public void ReleaseInput()
    {
        throw new NotImplementedException();
    }

    // todo: we might want to extract this for single input?
    public void Activate()
    {
        throw new NotImplementedException();
    }

    public SingularInputState GetState()
    {
        return _singularInputState;
    }

    public void Release()
    {
        throw new NotImplementedException();
    }

    public IRhythmInput ActSingle()
    {
        // todo: uncomment
        return this;
    }

    public void Poll(double delta)
    {
        if (_directionTtl > 0)
        {
            _directionTtl -= delta;
        }
        else
        {
            _claimingBeat = null;
        }
    }

    public void SetRelativeMotion(Vector2 direction)
    {
        // todo: dead spot?
        _direction = direction.Angle();
        _directionTtl = Globals.FrameEpsilon;
    }

    public void SetCursorPosition(Vector2 position)
    {
        _position = position;
    }

    public Vector2 GetCursorPosition()
    {
        return _position;
    }
}