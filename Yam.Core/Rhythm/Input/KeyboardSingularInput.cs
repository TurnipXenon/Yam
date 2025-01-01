#nullable enable
using Godot;
using Yam.Core.Rhythm.Chart;

namespace Yam.Core.Rhythm.Input;

// todo(turnip): refactor code to remove duplicates
public class KeyboardSingularInput : IRhythmInput
{
    // todo(turnip): see if we can combine them into the details class below
    private Beat? _claimingBeat;
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

    public Beat? GetClaimingChannel() => _claimingBeat;

    public bool ClaimInput(Beat claimingBeat)
    {
        if (_claimingBeat == null)
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
}