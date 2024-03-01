using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Yam.scenes.rhythm.models;

public class RhythmData
{
    // todo: other metadata and difficulty properties
    /// <summary>
    /// <c>ApproachRate</c> is the speed at which the HitObjects move on screen.
    /// </summary>
    /// <remarks>
    /// This property affects, not only the speed, but also the judgment duration.
    /// </remarks>
    public float ApproachRate;

    public string AudioFilename;

    // todo Timing points

    // todo: HitObjects
    public readonly List<HitObjectData> HitObjectList = new();

    private enum MapReadingState
    {
        Searching,
        Metadata,
        ReadingHitObject,
    }

    public static RhythmData FromOsuMapFile(string songBasePath, string mapName)
    {
        var path = $"{songBasePath}{mapName}";
        var rhythmData = new RhythmData(); // todo: maybe transfer to RhythmData

        var f = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if (f == null)
        {
            GD.PrintErr("TODO: handle error in ReadMap");
            return rhythmData;
        }

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
                        case "[General]":
                        case "[Difficulty]":
                            readingStateStack.Push(MapReadingState.Metadata);
                            break;
                    }

                    break;

                case MapReadingState.ReadingHitObject when CheckIfLineEmpty(line, ref readingStateStack):
                {
                    continue;
                }
                case MapReadingState.ReadingHitObject:
                    rhythmData.HitObjectList.Add(HitObjectData.FromOsuHitObjectString(line));
                    break;

                case MapReadingState.Metadata when CheckIfLineEmpty(line, ref readingStateStack):
                {
                    continue;
                }
                case MapReadingState.Metadata:
                    var lineParts = line.StripEdges().Split(":");
                    Debug.Assert(lineParts.Length >= 2);
                    var property = lineParts[0];
                    var value = lineParts[1].StripEdges();
                    switch (property)
                    {
                        // General
                        case "AudioFilename":
                            rhythmData.AudioFilename = $"{songBasePath}{value}";
                            GD.Print($"Filename: {rhythmData.AudioFilename}");
                            break;

                        // Difficulty
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

        GD.Print("Done");
        return rhythmData;
    }

    private static bool CheckIfLineEmpty(string line, ref Stack<MapReadingState> stateStack)
    {
        Debug.Assert(stateStack != null);

        if (line.StripEdges() == "")
        {
            stateStack.Pop();
            return true;
        }

        return false;
    }

    public override string ToString()
    {
        return string.Join("\n", HitObjectList);
    }
}