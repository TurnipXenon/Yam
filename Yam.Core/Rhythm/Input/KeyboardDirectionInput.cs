#nullable enable
using Godot;
using Yam.Core.Common;
using Yam.Core.Rhythm.Chart;

namespace Yam.Core.Rhythm.Input;

// todo(turnip): refactor code to remove duplicates
public class KeyboardDirectionInput
{
    public GameLogger Logger = new();
    
    // todo(turnip): see if we can combine them into the details class below
    private Beat? _claimingBeat;
    public Vector2 Direction { get; set; }

    public bool IsValidDirection() => true;

    public bool IsDirectionSensitive() => false;

    public Vector2 GetDirection() => Direction;

    public string GetInputCode() => "N/A";

    public Beat? GetClaimingChannel() => _claimingBeat;

    public bool ClaimInput(Beat claimingBeat)
    {
        if (_claimingBeat == null)
        {
            return false;
        }

        Logger.Print("Input Claimed");
        _claimingBeat = claimingBeat;
        return true;
    }

    public void ReleaseInput()
    {
        _claimingBeat = null;
    }
}