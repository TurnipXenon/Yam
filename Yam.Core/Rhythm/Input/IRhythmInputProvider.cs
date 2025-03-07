using System.Collections.Generic;
using Godot;

namespace Yam.Core.Rhythm.Input;

/**
 * Requirements:
 * 1. We have slides, taps, and hold
 * 2. We want to detect our mouse or slide pads directions and identify which one is active
 * 3. Channels can claim inputs
 */
public interface IRhythmInputProvider
{
    public List<ISingularInput> GetDirectionInputList();
    public List<ISingularInput> GetSingularInputList();

    /// <summary>
    /// Simulate a frame
    /// </summary>
    /// <param name="delta">time since last frame in <b>seconds</b></param>
    public void Poll(double delta);

    IRhythmInput ProcessEvent(InputEvent @event);

    /// <summary>
    /// Get abstracted direction input
    /// </summary>
    /// <returns></returns>
    /// <remarks>Required for visualization</remarks>
    IDirectionInput GetDirectionInput();
}