using System;
using System.Collections.Generic;
using Godot;

namespace Yam.Godot.Scripts.Rhythm.Dev;

[Tool]
public partial class ParseOsu : Node
{
    private const string OsuSourceFile = "res://Scenes/Rhythm/Songs/source.osu";
    private const string JsonResultFile = "res://Scenes/Rhythm/Songs/result.json";

    [Export]
    public bool btn_ParseOsuFile
    {
        get => false;
        set => ParseOsuTiming();
    }

    [Export(PropertyHint.MultilineText)] public string ParsedResult;

    private enum ReadingState
    {
        Scanning,
        BeatReading
    }

    private static void ParseOsuTiming()
    {
        using var f = FileAccess.Open(OsuSourceFile, FileAccess.ModeFlags.Read);
        if (f == null)
        {
            GD.PrintErr("ParseOsu: Missing osu file");
            return;
        }

        using var resultFile = FileAccess.Open(JsonResultFile, FileAccess.ModeFlags.Write);
        if (resultFile == null)
        {
            GD.PrintErr("ParseOsu: Missing target result file");
            return;
        }

        var readingState = ReadingState.Scanning;
        var beats = new List<string>();
        while (!f.EofReached())
        {
            var line = f.GetLine();

            switch (readingState)
            {
                case ReadingState.Scanning:

                    if (line.StartsWith("[HitObjects]"))
                    {
                        readingState = ReadingState.BeatReading;
                    }

                    break;
                case ReadingState.BeatReading:

                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var lineParts = line.Split(',');
                        beats.Add("\t{\n" +
                                  $"\t\t\"time\": {lineParts[2]},\n" +
                                  $"\t\t\"uCoord\": {Mathf.RoundToInt(float.Parse(lineParts[0]) / 103f)}\n" +
                                  "\t}");
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        resultFile.StoreString($"[\n{string.Join(",\n", beats.ToArray())}\n]");
        GD.Print("Finished parsing osu file");
    }
}