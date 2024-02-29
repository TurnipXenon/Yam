using Godot;
using Yam.scenes.rhythm.models;

public partial class HitObject : Sprite2D
{
    private HitObjectData _data;
    private RhythmTestMain _rhythmTestMain;
    private bool _isReady;
    private float _premultiplier;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    public void SetData(HitObjectData data, RhythmTestMain rhythmTestMain, ulong preemptTime)
    {
        _rhythmTestMain = rhythmTestMain;
        _data = data;
        _premultiplier = (_rhythmTestMain.TriggerPoint.Position.X - _rhythmTestMain.SpawnPoint.Position.X) /
                         preemptTime;
        _isReady = true;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!_isReady)
        {
            return;
        }

        // todo: maybe put all the time in one place so they dont go insane?
        var x = _rhythmTestMain.SpawnPoint.Position.X + _premultiplier * (Time.GetTicksMsec() - _data.Timing);
        Position = new Vector2(x, _rhythmTestMain.SpawnPoint.Position.Y);
    }
}