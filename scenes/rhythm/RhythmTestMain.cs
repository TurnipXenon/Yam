using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using Yam.scenes.rhythm.models;
using Yam.scenes.rhythm.services;

public partial class RhythmTestMain : Node
{
    private const string RawSongBasePath = "res://scenes/rhythm/songs/";

    [Export]
    public Node2D SpawnPoint;

    [Export]
    public Node2D TriggerPoint;

    [Export]
    public Node2D DestructionPoint;

    [Export]
    public PackedScene HitObjectPrefab;

    [Export]
    public AudioStreamPlayer AudioPlayer;

    private readonly List<ChartMetadata> _chartList = new();
    private readonly RhythmInterpreter _interpreter = new();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Debug.Assert(SpawnPoint != null);
        Debug.Assert(TriggerPoint != null);
        Debug.Assert(DestructionPoint != null);
        Debug.Assert(HitObjectPrefab != null);
        Debug.Assert(AudioPlayer != null);

        Task.Run(RunRandomCoroutine);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // todo: simulate what needs to exist
        _interpreter.Process();
    }

    async Task RunRandomCoroutine()
    {
        var baseDir = DirAccess.Open(RawSongBasePath);
        if (baseDir == null)
        {
            GD.PrintErr("TODO: base path for raw files error");
            return;
        }

        var dirs = baseDir.GetDirectories();
        foreach (var songDirName in dirs)
        {
            // todo: find json attribute?
            // todo: create one if not found

            // todo: find which *.osu file to find
            var songBasePath = $"{RawSongBasePath}{songDirName}/";
            var songBaseDir = DirAccess.Open(songBasePath);
            if (songBaseDir == null)
            {
                GD.PrintErr("TODO: songBaseDir file open error");
                continue;
            }

            var contentFiles = songBaseDir.GetFiles();
            foreach (var content in contentFiles)
            {
                if (!content.Contains("chart_meta.json")) continue;

                var chartMetadata = ChartMetadata.FromChartMeta(songBasePath, content);
                _chartList.Add(chartMetadata);
            }

            // todo: find mp3

            // todo: put indefinite delay in const
            await Task.Yield();
        }

        _interpreter.SetActiveChart(_chartList[0]);
        _interpreter.Start(this);
    }
}