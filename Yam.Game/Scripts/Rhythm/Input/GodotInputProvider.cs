#nullable enable
using System.Collections.Generic;
using Godot;
using Yam.Core.Rhythm.Input;

namespace Yam.Game.Scripts.Rhythm.Input;

public class GodotInputProvider : IRhythmInputProvider
{
    private const string Keyboard1Up = "keyboard1_up";

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
        return _keyboardSingularInputList;
    }

    public IRhythmInput ProcessEvent(InputEvent @event)
    {
        foreach (var singularInput in _keyboardSingularInputList)
        {
            var keyCode = singularInput.GetInputCode();
            if (@event.IsActionReleased(keyCode))
            {
                singularInput.Release();
                return singularInput;
            } else if ((@event.IsActionReleased(keyCode)))
            {
                singularInput.Start();
                return singularInput;
            }
        }
        
        return SpecialInput.UnknownInput;
    }
}