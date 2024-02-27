using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Yam.scenes.rhythm.models;

public partial class RhythmTestMain : Node
{
    enum MapReadingState
    {
        Searching,
        ReadingHitObject,
    }

    private const string RawSongBasePath = "res://scenes/rhythm/songs/raw/";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
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
                if (content.Contains("[Cake].osu"))
                {
                    GD.Print(content);
                    ReadMap($"{songBasePath}{content}");
                }
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
                    if (line.Contains("[HitObjects]"))
                    {
                        readingStateStack.Push(MapReadingState.ReadingHitObject);
                    }

                    GD.Print(line);
                    break;

                case MapReadingState.ReadingHitObject:
                    if (line.Split(",").Length <= 1)
                    {
                        readingStateStack.Pop();
                        continue;
                    }

                    GD.Print(line.Split(",").Join("---"));

                    // todo: get timing points
                    rhythmData.HitObjectList.Add(HitObject.FromOsuHitObjectString(line));

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        GD.Print("Done");
    }
}