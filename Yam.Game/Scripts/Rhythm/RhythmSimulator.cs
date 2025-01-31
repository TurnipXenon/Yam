using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Godot;
using Yam.Core.Common;
using Yam.Core.Rhythm.Chart;
using Yam.Core.Rhythm.Input;
using Yam.Game.Scripts.Rhythm.Game.SingleBeat;
using Yam.Game.Scripts.Rhythm.Input;
using ChartModel = Yam.Core.Rhythm.Chart.Chart;
using HoldBeat = Yam.Game.Scripts.Rhythm.Game.HoldBeat.HoldBeat;

namespace Yam.Game.Scripts.Rhythm;

public partial class RhythmSimulator : Node, IRhythmSimulator
{
    [Export] public Resource Chart { get; set; }
    [Export] public AudioStreamPlayer AudioStreamPlayer { get; set; }
    [Export] public AudioStream AudioStream { get; set; }
    [Export] public Node2D TriggerPoint { get; set; }
    [Export] public Node2D SpawnPoint { get; set; }
    [Export] public Node2D DestructionPoint { get; set; }
    [Export] public PackedScene SingleBeatPrefab { get; set; }
    [Export] public PackedScene TickPrefab { get; set; }
    [Export] public float PreEmptDuration { get; set; } = 2f;

    // todo(turnip): turn into export so visible in Godot inspector
    /// <summary>
    /// Ordered from most narrow (better) to widest (worse)
    /// </summary>
    public List<ReactionWindow> RelativeReactionWindow { get; set; } = Beat.DefaultRelativeReactionWindow.ToList();

    /** Parent node where all the children beats will be parented to */
    [Export]
    public Node Parent { get; set; }

    public SingleBeatPooler SingleBeatPooler;
    public SingleBeatPooler TickPooler;
    private bool _isPlaying = false;
    private ChartModel _chartModel;
    private float _currentSongTime;
    private GodotInputProvider _inputProvider;
    private float _lastReactionUpdate;
    private List<ReactionWindow> _processedReactionWindow = new();
    private float _songStart;
    private float _songDriftAdjustment;

    public override void _Ready()
    {
        Debug.Assert(TriggerPoint != null, "TriggerPoint != null");
        Debug.Assert(SpawnPoint != null, "SpawnPoint != null");
        Debug.Assert(DestructionPoint != null, "DestructionPoint != null");
        Debug.Assert(SingleBeatPrefab != null, "SingleBeatPrefab != null");
        Debug.Assert(AudioStream != null, "AudioStream != null");
        Debug.Assert(AudioStreamPlayer != null, "AudioStreamPlayer != null");
        Debug.Assert(Parent != null, "Parent != null");

        SingleBeatPooler = new SingleBeatPooler(this, SingleBeatPrefab);
        TickPooler = new SingleBeatPooler(this, TickPrefab);
        _inputProvider = new GodotInputProvider();

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

        var currentTime = Time.GetTicksMsec() / 1000f;
        _currentSongTime = currentTime - _songStart;
        
        // relying on song start instead of getting playback position
        // we will have issues later when we enable pausing and song looping
        // GD.Print($"{_currentSongTime} vs {AudioStreamPlayer.GetPlaybackPosition()}");

        // simulate idle time for input misses
        _chartModel.SimulateBeatInput(this, SpecialInput.GameInput);

        // todo(turnip): if the updating or processing logic here becomes too complex
        // extract the logic elsewhere???

        // todo: determine which beats should be visible

        // todo: give this to the pooler and let it decide which ones idle vs instantiate
        var visualizableBeats = _chartModel.GetVisualizableBeats(this);

        foreach (var beat in visualizableBeats)
        {
            switch (beat.GetBeatType())
            {
                case BeatType.Single:
                    SingleBeatPooler.Request(new PooledSingleBeatArgs()
                    {
                        Beat = beat,
                        RhythmSimulator = this
                    });
                    break;

                case BeatType.Slide:
                    // todo
                    break;

                case BeatType.Hold:
                    var holdBeat = new HoldBeat();
                    holdBeat.Initialize(this, beat);
                    Parent.AddChild(holdBeat);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void ParseChart()
    {
        using var f = FileAccess.Open(Chart.ResourcePath, FileAccess.ModeFlags.Read);
        if (f == null)
        {
            GD.PrintErr("Chart: Missing Chart file");
            return;
        }

        var chartEntity = JsonSerializer.Deserialize<ChartEntity>(f.GetAsText());
        _chartModel = ChartModel.FromEntity(chartEntity, RelativeReactionWindow);

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
        _songStart = Time.GetTicksMsec() / 1000f;
        _songDriftAdjustment = 0f;
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

    public List<ReactionWindow> GetReactionWindowList()
    {
        if (Math.Abs(_lastReactionUpdate - _currentSongTime) > 0.001)
        {
            _processedReactionWindow =
                RelativeReactionWindow.Select(w => new ReactionWindow(w.Threshold, w.BeatInputResult, _currentSongTime))
                    .ToList();
            _lastReactionUpdate = _currentSongTime;
        }

        return _processedReactionWindow;
    }


    public override void _UnhandledInput(InputEvent @event)
    {
        // todo(turnip): prioritize simulating a slide direction
        _chartModel.SimulateBeatInput(this, _inputProvider.ProcessEvent(@event));
    }
}