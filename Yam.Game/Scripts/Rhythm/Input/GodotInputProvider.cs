#nullable enable
using System;
using System.Collections.Generic;
using Godot;
using Yam.Core.Common;
using Yam.Core.Rhythm.Input;

namespace Yam.Game.Scripts.Rhythm.Input;

public class GodotInputProvider : IRhythmInputProvider
{
    private const string Keyboard1Up = "keyboard1_up";

    // 2 slides + input combo then 6 more solo input combos
    private List<IRhythmInput> _activeInputList = new();

    private readonly KeyboardSingularInput _keyboardUp = new(Keyboard1Up);
    private readonly List<ISingularInput> _keyboardSingularInputList;
    private readonly MouseDirectionInput _mouse = new();

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

    public void Poll(double delta)
    {
        _mouse.Poll(delta);
    }

    public IRhythmInput ProcessEvent(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            _mouse.SetRelativeMotion(eventMouseMotion.Relative);
            return _mouse;
        }
        
        foreach (var singularInput in _keyboardSingularInputList)
        {
            var keyCode = singularInput.GetInputCode();
            if (@event.IsActionReleased(keyCode))
            {
                singularInput.Release();
                return singularInput;
            } else if ((@event.IsActionPressed(keyCode)))
            {
                singularInput.Activate();
                return singularInput;
            }
        }
        
        return SpecialInput.UnknownInput;
    }
}