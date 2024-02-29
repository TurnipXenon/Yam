using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

    // todo Timing points

    // todo: HitObjects
    public readonly List<HitObject> HitObjectList = new();

    private enum MapReadingState
    {
        Searching,
        Difficulty,
        ReadingHitObject,
    }

    public static RhythmData FromOsuMapFile(string path)
    {
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