using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using Yam.scenes.rhythm.models;

public partial class RhythmTestMain : Node
{
    private enum MapReadingState
    {
        Searching,
        Difficulty,
        ReadingHitObject,
    }

    private const string RawSongBasePath = "res://scenes/rhythm/songs/raw/";

    [Export]
    public Node SpawnPoint;

    [Export]
    public Node TriggerPoint;

    [Export]
    public Node DestructionPoint;

    [Export]
    public PackedScene HitObjectPrefab;

    private readonly List<RhythmData> _rhythmDataList = new();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Debug.Assert(SpawnPoint != null);
        Debug.Assert(TriggerPoint != null);
        Debug.Assert(DestructionPoint != null);
        Debug.Assert(HitObjectPrefab != null);

        Task.Run(RunRandomCoroutine);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // todo: simulate what needs to exist
    }

    async Task RunRandomCoroutine()
    {
        GD.Print("Testing...");
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

            GD.Print(DirAccess.GetOpenError().ToString());
            var contentFiles = songBaseDir.GetFiles();
            foreach (var content in contentFiles)
            {
                // todo: remove hardcode to finding "[Cake]"
                if (!content.Contains("[Cake].osu")) continue;
                
                // todo
                ReadMap($"{songBasePath}{content}");
            }

            // todo: find mp3

            // todo: put indefinite delay in const
            await Task.Yield();
        }

        // create object
        GD.Print("Finished");
    }

    private void ReadMap(string path)
    {
        var f = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if (f == null)
        {
            GD.PrintErr("TODO: handle error in ReadMap");
            return;
        }

        var rhythmData = new RhythmData(); // todo: maybe transfer to RhythmData
        var body = f.GetAsText();
        var readingStateStack = new Stack<MapReadingState>();
        readingStateStack.Push(MapReadingState.Searching);
        foreach (var line in body.Split("\n"))
        {
            switch (readingStateStack.Peek())
            {
                case MapReadingState.Searching:
                    switch (line.StripEdges())
                    {
                        case "[HitObjects]":
                            readingStateStack.Push(MapReadingState.ReadingHitObject);
                            break;
                        case "[Difficulty]":
                            readingStateStack.Push(MapReadingState.Difficulty);
                            break;
                    }

                    break;

                case MapReadingState.ReadingHitObject when CheckIfLineEmpty(line, ref readingStateStack):
                {
                    continue;
                }
                case MapReadingState.ReadingHitObject:
                    rhythmData.HitObjectList.Add(HitObject.FromOsuHitObjectString(line));
                    break;

                case MapReadingState.Difficulty when CheckIfLineEmpty(line, ref readingStateStack):
                {
                    continue;
                }
                case MapReadingState.Difficulty:
                    var lineParts = line.StripEdges().Split(":");
                    Debug.Assert(lineParts.Length >= 2);
                    var property = lineParts[0];
                    var value = lineParts[1];
                    switch (property)
                    {
                        case "ApproachRate":
                            rhythmData.ApproachRate = float.Parse(value);
                            break;
                        // todo: add more properties
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        _rhythmDataList.Add(rhythmData);
        GD.Print("Done");
    }

    private bool CheckIfLineEmpty(string line, ref Stack<MapReadingState> stateStack)
    {
        Debug.Assert(stateStack != null);

        if (line.StripEdges() == "")
        {
            stateStack.Pop();
            return true;
        }

        return false;
    }
}