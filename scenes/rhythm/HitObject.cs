using Godot;
using Yam.scenes.rhythm.models;

public partial class HitObject : Sprite2D
{
    private HitObjectData _data;
    private RhythmTestMain _rhythmTestMain;
    private bool _isReady;
    private float _premultiplier;


    public void SetData(HitObjectData data, RhythmTestMain rhythmTestMain, ulong preemptTime)
    {
        _rhythmTestMain = rhythmTestMain;
        _data = data;
        _premultiplier = (_rhythmTestMain.TriggerPoint.Position.X - _rhythmTestMain.SpawnPoint.Position.X) /
                         preemptTime;
        var x = _rhythmTestMain.SpawnPoint.Position.X + _premultiplier * (Time.GetTicksMsec() - _data.Timing);
        Position = new Vector2(x, _rhythmTestMain.SpawnPoint.Position.Y);
        _isReady = true;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!_isReady)
        {
            return;
        }

        // todo: maybe put all the time in one place so the HitObjects dont go insane?
        var currentTime = Time.GetTicksMsec();
        var timeDiff = currentTime > _data.Timing ? currentTime - _data.Timing : 0; // prevent underflow
        var x = _rhythmTestMain.SpawnPoint.Position.X + _premultiplier * timeDiff;
        Position = new Vector2(x, _rhythmTestMain.SpawnPoint.Position.Y);

        // todo: put the object in a pool object residing in RhythmInterpreter instead of fully destroying
        if (x > _rhythmTestMain.DestructionPoint.Position.X)
        {
            QueueFree();
        }
    }
}