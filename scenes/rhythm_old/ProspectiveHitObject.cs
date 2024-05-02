using Godot;
using Yam.scenes.rhythm.services;

public partial class ProspectiveHitObject : Sprite2D
{
    [Export]
    public RhythmTestMain RhythmTestMain;

    private RhythmInterpreter _interpreter;
    private bool _isReady;
    private float _premultiplier;

    public override void _Ready()
    {
        _interpreter = RhythmTestMain.Interpreter;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!_interpreter.IsReady)
        {
            return;
        }

        if (!_isReady)
        {
            _premultiplier = (RhythmTestMain.TriggerPoint.Position.X - RhythmTestMain.SpawnPoint.Position.X) /
                             _interpreter.PreemptTime;
            _isReady = true;
        }

        var mousePosition = GetGlobalMousePosition();

        // convert mouseX to time
        var timeDiff = _interpreter.AudioPosition
                       - (mousePosition.X - RhythmTestMain.TriggerPoint.Position.X) /
                       _premultiplier;

        // floor(currentTime/beatLength) * beatLength
        var flooredTime = Mathf.Floor(timeDiff / _interpreter.CurrentTiming.BeatLength) *
                          _interpreter.CurrentTiming.BeatLength;
        if (Mathf.IsNaN(flooredTime))
        {
            GD.Print("isNan");
            flooredTime = 0f;
        }

        // newTime to mouseX
        var x = RhythmTestMain.TriggerPoint.Position.X + _premultiplier * (_interpreter.AudioPosition - flooredTime);
        Position = new Vector2(x, mousePosition.Y);
    }
}