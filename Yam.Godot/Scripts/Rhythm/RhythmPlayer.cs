using System;
using System.Diagnostics;
using System.Text.Json;
using Godot;
using Yam.Core.Rhythm.Chart;
using Yam.Godot.Scripts.Rhythm.Godot.SingleBeat;
using ChartModel = Yam.Core.Rhythm.Chart.Chart;

namespace Yam.Godot.Scripts.Rhythm;

public partial class RhythmPlayer : Node, IRhythmPlayer
{
    [Export] public Resource Chart { get; set; }
    [Export] public AudioStreamPlayer AudioStreamPlayer { get; set; }
    [Export] public AudioStream AudioStream { get; set; }
    [Export] public Node2D TriggerPoint { get; set; }
    [Export] public Node2D SpawnPoint { get; set; }
    [Export] public Node2D DestructionPoint { get; set; }
    [Export] public PackedScene SingleBeatPrefab { get; set; }
    [Export] public float PreEmptDuration { get; set; } = 2f;
    /** Parent node where all the children beats will be parented to */
    [Export] public Node Parent { get; set; }

    private SingleBeatPooler _singleBeatPooler;
    private bool _isPlaying = false;
    private ChartModel _chartModel;
    private float _currentSongTime;

    public override void _Ready()
    {
        Debug.Assert(TriggerPoint != null, "TriggerPoint != null");
        Debug.Assert(SpawnPoint != null, "SpawnPoint != null");
        Debug.Assert(DestructionPoint != null, "DestructionPoint != null");
        Debug.Assert(SingleBeatPrefab != null, "SingleBeatPrefab != null");
        Debug.Assert(AudioStream != null, "AudioStream != null");
        Debug.Assert(AudioStreamPlayer != null, "AudioStreamPlayer != null");
        Debug.Assert(Parent != null, "Parent != null");

        _singleBeatPooler = new SingleBeatPooler(this);

        // todo(turnip): remove and make it situational in the future
        // such that it is not triggered by events in here but called externally
        // by the scene manager
        ParseChart();

        // todo(turnip): extract function this is not the correct place for this but for now let's put it here
        StartChart();
    }

    public override void _Process(double delta)
    {
        if (!_isPlaying)
        {
            return;
        }

        _currentSongTime = AudioStreamPlayer.GetPlaybackPosition();
        
        // todo(turnip): if the updating or processing logic here becomes too complex
        // extract the logic elsewhere???

        // todo: determine which beats should be visible

        // todo: give this to the pooler and let it decide which ones idle vs instantiate
        var currentBeats = _chartModel.GetVisualizableBeats(this);
        foreach (var beat in currentBeats)
        {
            switch (beat.GetBeatType())
            {
                case BeatType.Single:
                    _singleBeatPooler.Request(new SinglePooledBeatArgs()
                    {
                        Beat = beat,
                        RhythmPlayer = this
                    });
                    break;
                case BeatType.Slide:
                    // todo
                    break;
                case BeatType.Hold:
                    // todo
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void ParseChart()
    {
        // todo(turnip): implement
        using var f = FileAccess.Open(Chart.ResourcePath, FileAccess.ModeFlags.Read);
        if (f == null)
        {
            GD.PrintErr("Chart: Missing Chart file");
            return;
        }

        var chartEntity = JsonSerializer.Deserialize<ChartEntity>(f.GetAsText());
        _chartModel = ChartModel.FromEntity(chartEntity);

        // todo(turnip): remove
        GD.Print("Done parsing");

        // todo(turnip): choose music based on chart instead of hardcoded-ish here
    }

    private void StartChart()
    {
        // todo: turn on flag so update can do its thing
        _isPlaying = true;
        // todo(turnip): possibly support chart metadata for delay
        AudioStreamPlayer.Stream = AudioStream;
        AudioStreamPlayer.Play();
    }

    public float GetCurrentSongTime()
    {
        return _currentSongTime;
    }

    public float GetPreEmptTime()
    {
        return _currentSongTime - PreEmptDuration;
    }
    
    public float GetPreEmptDuration()
    {
        return PreEmptDuration;
    }
}