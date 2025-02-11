using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Godot;
using Yam.Core.Rhythm.Chart;
using Yam.Core.Rhythm.Input;
using Yam.Game.Scripts.Rhythm.Game.SingleBeat;
using Yam.Game.Scripts.Rhythm.Game.SlideBeat;
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
    [Export] public PackedScene SlidePrefab { get; set; }
    [Export] public PackedScene ResultPrefab { get; set; }
    [Export] public float PreEmptDuration { get; set; } = 2f;

    // todo(turnip): turn into export so visible in Godot inspector
    /// <summary>
    /// Ordered from most narrow (better) to widest (worse)
    /// </summary>
    public List<ReactionWindow> RelativeReactionWindow { get; set; } = Beat.DefaultRelativeReactionWindow.ToList();

    /** Parent node where all the children beats will be parented to */
    [Export]
    public Node Parent { get; set; }

    public InputVisualizer InputVisualizer { get; set; }

    public SlideBeatPooler SlideBeatPooler;
    public SingleBeatPooler SingleBeatPooler;
    public SingleBeatPooler TickPooler;
    public IRhythmInputProvider InputProvider;
    public ChartModel ChartModel;
    private bool _isPlaying;
    private float _currentSongTime;
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
        Debug.Assert(TickPrefab != null, "SingleBeatPrefab != null");
        Debug.Assert(SlidePrefab != null, "SingleBeatPrefab != null");
        Debug.Assert(AudioStream != null, "AudioStream != null");
        Debug.Assert(AudioStreamPlayer != null, "AudioStreamPlayer != null");
        Debug.Assert(Parent != null, "Parent != null");
        Debug.Assert(ResultPrefab != null, "ResultPrefab != null");

        SingleBeatPooler = new SingleBeatPooler(this, SingleBeatPrefab);
        TickPooler = new SingleBeatPooler(this, TickPrefab);
        SlideBeatPooler = new SlideBeatPooler(this, SlidePrefab);
        InputProvider = new GodotInputProvider();

        // todo(turnip): remove and make it situational in the future
        // such that it is not triggered by events in here but called externally
        // by the scene manager
        ParseChart();
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
        ChartModel.SimulateBeatInput(this, SpecialInput.GameInput);
        InputProvider.Poll(delta);

        // todo(turnip): if the updating or processing logic here becomes too complex
        // extract the logic elsewhere???

        // todo: determine which beats should be visible

        // todo: give this to the pooler and let it decide which ones idle vs instantiate
        var visualizableBeats = ChartModel.GetVisualizableBeats(this);

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
                    SlideBeatPooler.Request(new PooledSlideBeatArgs()
                    {
                        Beat = beat,
                        RhythmSimulator = this
                    });
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
        ChartModel = ChartModel.FromEntity(chartEntity, RelativeReactionWindow);

        // todo(turnip): remove
        GD.Print("Done parsing");

        // todo(turnip): choose music based on chart instead of hardcoded-ish here
    }

    public void StartChart()
    {
        // todo: turn on flag so update can do its thing
        _isPlaying = true;
        // todo(turnip): possibly support chart metadata for delay
        AudioStreamPlayer.Stream = AudioStream;
        AudioStreamPlayer.Play();
        _songStart = Time.GetTicksMsec() / 1000f;
        _songDriftAdjustment = 0f;
        EmitSignalOnStartChart();
    }

    [Signal]
    public delegate void OnStartChartEventHandler();

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

    public float GetDirectionTolerance()
    {
        return Beat.DefaultDirectionTolerance;
    }

    public event EventHandler<BeatResultEvent> BeatSimulationResultEvent = delegate { };

    public override void _UnhandledInput(InputEvent @event)
    {
        ChartModel.SimulateBeatInput(this, InputProvider.ProcessEvent(@event));
    }

    public void InvokeBeatResultEvent(IBeatVisualizer beatVisualizer, IBeat beat, BeatInputResult result)
    {
        BeatSimulationResultEvent.Invoke(this, new BeatResultEvent(beat, result));
        var resultLabel = ResultPrefab.Instantiate<Scenes.Rhythm.RhythmPlayground.Result.ResultLabel>();
        resultLabel.Initialize(this, result, beat.GetVisualizer());
    }
}