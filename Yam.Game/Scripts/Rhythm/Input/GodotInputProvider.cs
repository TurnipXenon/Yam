#nullable enable
using System.Collections.Generic;
using Godot;
using Yam.Core.Rhythm.Input;
using In = Godot.Input;

namespace Yam.Game.Scripts.Rhythm.Input;

public class GodotInputProvider : IRhythmInputProvider
{
    public const string Keyboard1Up = "keyboard1_up";

    // 2 slides + input combo then 6 more solo input combos
    private List<IRhythmInput> _activeInputList = new();

    private KeyboardDirectionInput _keyboardDirectionInput = new();
    private readonly KeyboardSingularInput _keyboardUp = new(Keyboard1Up);

    private readonly List<ISingularInput> _keyboardSingularInputList;

    public GodotInputProvider()
    {
        _keyboardSingularInputList = new List<ISingularInput>
        {
            _keyboardUp,
            new KeyboardSingularInput("keyboard1_down"),
            new KeyboardSingularInput("keyboard1_left"),
            new KeyboardSingularInput("keyboard1_right")
        };
    }

    public List<ISingularInput> GetDirectionInputList()
    {
        // todo(turnip): do something
        return _keyboardSingularInputList;
    }

    public List<ISingularInput> GetSingularInputList()
    {
        // todo: fix
        return _keyboardSingularInputList;
    }

    public void PollInput()
    {
        // because it's complex
        // _keyboardDirectionInput.Direction = Vector2.Zero;
        //
        // if (In.IsActionPressed("keyboard1_up"))
        // {
        //     _keyboardDirectionInput.Direction = Vector2.Up;
        // }
        // else if (In.IsActionPressed("keyboard1_down"))
        // {
        //     _keyboardDirectionInput.Direction = Vector2.Down;
        // }
        //
        // if (In.IsActionPressed("keyboard1_left"))
        // {
        //     _keyboardDirectionInput.Direction += Vector2.Left;
        // } else if (In.IsActionPressed("keyboard1_right"))
        // {
        //     _keyboardDirectionInput.Direction += Vector2.Right;
        // }

        // if (_keyboardDirectionInput.GetClaimingChannel() != null)
        // {
        //     foreach (var code in _keyboardSingularInputList)
        //     {
        //         // todo(turnip): 
        //     }
        // }
    }

    public IRhythmInput ProcessEvent(InputEvent @event)
    {
        if (@event.IsAction(Keyboard1Up, true))
        {
            // todo(turnip): consider difference with just pressed and held state buffer?
            if (@event.IsActionReleased(Keyboard1Up))
            {
                _keyboardUp.Release();
            }
            else
            {
                _keyboardUp.Press();
            }

            return _keyboardUp;
        }

        return SpecialInput.UnknownInput;
    }
}