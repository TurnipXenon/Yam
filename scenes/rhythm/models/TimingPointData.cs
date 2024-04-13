using System.Text.Json.Serialization;

namespace Yam.scenes.rhythm.models;

public class TimingPointData
{
    [JsonInclude]
    public int Time;

    [JsonInclude]
    public float BeatLength;

    [JsonInclude]
    public int Meter;
}