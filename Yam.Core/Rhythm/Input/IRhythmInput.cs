#nullable enable
using Godot;
using Yam.Core.Rhythm.Chart;

namespace Yam.Core.Rhythm.Input;

public interface IRhythmInput
{
    public bool IsValidDirection();
    /**
     * True if we want more sensitivity or player is using a mouse or they feel turning it on with a slide pad
     * False if slide pad (default behavior) or button
     */
    public bool IsDirectionSensitive();
    public Vector2 GetDirection();
    public string GetInputCode();
    public Beat? GetClaimingChannel();

    /// <summary>
    /// Returns true if successfully claimed channel. False if another channel claimed this input
    /// </summary>
    /// <returns></returns>
    public bool ClaimInput(Beat beat);

    public void ReleaseInput();
}