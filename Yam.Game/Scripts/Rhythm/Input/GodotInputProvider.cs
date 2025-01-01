using System.Collections.Generic;
using Godot;
using Yam.Core.Rhythm.Input;
using In = Godot.Input;

namespace Yam.Game.Scripts.Rhythm.Input;

public class GodotInputProvider : IRhythmInputProvider
{
    // 2 slides + input combo then 6 more solo input combos
    private List<IRhythmInput> _activeInputList = new();

    private KeyboardDirectionInput _keyboardDirectionInput = new();

    private List<KeyboardSingularInput> _keyboardSingularInputList = new()
    {
        new KeyboardSingularInput("keyboard1_up"),
        new KeyboardSingularInput("keyboard1_down"),
        new KeyboardSingularInput("keyboard1_left"),
        new KeyboardSingularInput("keyboard1_right")
    };

    public List<IRhythmInput> GetDirectionInputList()
    {
        return _activeInputList;
    }

    public List<IRhythmInput> GetSingularInputList()
    {
        // todo: fix
        return _activeInputList;
    }

    public void PollInput()
    {
        _keyboardDirectionInput.Direction = Vector2.Zero;
        
        if (In.IsActionPressed("keyboard1_up"))
        {
            _keyboardDirectionInput.Direction = Vector2.Up;
        }
        else if (In.IsActionPressed("keyboard1_down"))
        {
            _keyboardDirectionInput.Direction = Vector2.Down;
        }

        if (In.IsActionPressed("keyboard1_left"))
        {
            _keyboardDirectionInput.Direction += Vector2.Left;
        } else if (In.IsActionPressed("keyboard1_right"))
        {
            _keyboardDirectionInput.Direction += Vector2.Right;
        }

        if (_keyboardDirectionInput.GetClaimingChannel() != null)
        {
            foreach (var code in _keyboardSingularInputList)
            {
                
            }
        }
    }
}