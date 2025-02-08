using Godot;
using Yam.Core.Rhythm.Chart;

namespace Yam.Core.Rhythm.Input;

public interface IDirectionInput : IRhythmInput
{
    /// <summary>
    /// Poll input to simulate passing time
    /// </summary>
    /// <param name="delta">Time passed since last frame in seconds</param>
    /// <remarks>A single poll is one frame</remarks>
    void Poll(double delta);

    /// <summary>
    /// Set the direction the input is moved to
    /// </summary>
    /// <param name="direction">Non-normalized direction of movement</param>
    void SetRelativeMotion(Vector2 direction);

    void SetCursorPosition(Vector2 position);
    Vector2 GetCursorPosition();
}