using System;
using System.Collections.Generic;
using System.Globalization;
using Godot;

namespace Yam.Game.Scripts.Rhythm.Dev;

[Tool]
public partial class ParseOsu : Node
{
    private const string OsuSourceFile = "res://Scenes/Rhythm/Songs/source.osu";
    private const string JsonResultFile = "res://Scenes/Rhythm/Songs/result.json";

    private bool _firstRunCheck = true;

    [Export]
    public bool btn_ParseOsuFile
    {
        get => false;
        set
        {
            if (_firstRunCheck)
            {
                _firstRunCheck = false;
            }
            else
            {
                ParseOsuTiming();
            }
        }
    }

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
        var rawBeatStrings = new List<string>();
        var beatList = new List<IParsedTick>();
        char[] charsToTrim = { ',', '\n' };

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
                        // todo(turnip): refactor so that it's raw data which is then turned into the text file
                        // old version writes it as it goes but we dont want that because we have a case where
                        // osu individual (separate) beat objects are ticks in a singular hold object 
                        var lineParts = line.Split(',');

                        if (lineParts[3] == "128")
                        {
                            // index 3 determines the type of osu object
                            // when it's 2^7 = 128, then it's a hold type osu!mania object
                            beatList.Add(new ParsedBeatObject(lineParts[2], lineParts[5]));
                        }
                        else if (lineParts[0] == "103")
                        {
                            // we've arbitrarily assigned the 2nd track in 5K osu!mania
                            // as our hold object's tick
                            //
                            // they are not individual beat objects in our game but are only
                            // ticks in holds, which are aesthetic reasons in linear holds
                            // but hold more scoring weight in bezier curved hold beats
                            var tick = new ParsedTick(lineParts[2], lineParts[0]);
                            foreach (var candidateBeat in beatList)
                            {
                                if (candidateBeat.TryContainTick(tick))
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // this is just a normal beat object
                            beatList.Add(new ParsedTick(lineParts[2], lineParts[0]));
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        beatList.ForEach((beat) =>
        {
            var s = "\t{\n"
                    + $"\t\t\"Time\": {beat.GetTime().ToString(CultureInfo.InvariantCulture)},"
                    + $"\t\t\"UCoord\": {beat.GetUCoord().ToString(CultureInfo.InvariantCulture)}";
            if (beat.IsComplex())
            {
                s += ",\n" +
                     "\t\t\"BeatList\": [\n";
                beat.GetTickList().ForEach((tick) =>
                {
                    s += $"\t\t\t{{ \"Time\": {tick.GetTime().ToString(CultureInfo.InvariantCulture)}, " +
                         $"\"UCoord\": {tick.GetUCoord().ToString(CultureInfo.InvariantCulture)} }},\n";
                });
                s = s.TrimEnd(charsToTrim);
                s += "\n\t\t]\n";
            }
            else
            {
                s += "\n";
            }

            s += "\t}";
            rawBeatStrings.Add(s);
        });

        resultFile.StoreString($"[\n{string.Join(",\n", rawBeatStrings.ToArray())}\n]");
        GD.Print("Finished parsing osu file");
    }
}