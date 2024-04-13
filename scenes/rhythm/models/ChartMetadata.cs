using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

namespace Yam.scenes.rhythm.models;

public class ChartMetadata
{
    [JsonInclude]
    public string SongName;

    [JsonInclude]
    public string AudioFilename;

    [JsonInclude]
    public List<TimingPointData> TimingPointList = new();

    [JsonInclude]
    public List<HitObjectData> HitObjectList = new();

    private enum MapReadingState
    {
        Searching,
        Metadata,
        ReadingHitObject,
    }

    public static ChartMetadata FromChartMeta(string songBasePath, string mapName)
    {
        var path = $"{songBasePath}{mapName}";

        var f = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if (f == null)
        {
            GD.PrintErr("TODO: handle error in ReadMap");
            return new ChartMetadata();
        }

        var body = f.GetAsText();
        var chartMeta = JsonSerializer.Deserialize<ChartMetadata>(body);

        GD.Print("Done");
        chartMeta.TimingPointList.Add(new TimingPointData
        {
            Time = 11,
            BeatLength = 12,
            Meter = 4
        });
        GD.Print(chartMeta.TimingPointList.Count);
        GD.Print(JsonSerializer.Serialize(chartMeta));
        return chartMeta;
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