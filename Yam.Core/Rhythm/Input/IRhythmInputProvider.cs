#nullable enable
using System.Collections.Generic;

namespace Yam.Core.Rhythm.Input;

/**
 * Requirements:
 * 1. We have slides, taps, and hold
 * 2. We want to detect our mouse or slide pads directions and identify which one is active
 * 3. Channels can claim inputs
 */
public interface IRhythmInputProvider
{
    public List<IRhythmInput> GetDirectionInputList();
    public List<IRhythmInput> GetSingularInputList();
    public void PollInput();
}