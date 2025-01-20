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
    public IBeat? GetClaimingChannel();

    /// <summary>
    /// Returns true if successfully claimed channel. False if another channel claimed this input
    /// </summary>
    /// <returns></returns>
    public bool ClaimOnStart(IBeat beat);

    public void ReleaseInput();
    public InputSource GetSource();
    RhythmActionType GetRhythmActionType();
    
    /// <summary>
    /// Activate is a generic term to indicate that the input's state has become active
    /// For buttons, this means they've been pressed
    /// For mouse, this means they've been moved
    /// For direction pads, this means they've been moved out of Vector2.Zero
    /// </summary>
    public void Activate();
    public void Release();
}

public interface IRhythmInputListener
{
    public void OnInputRelease();
}