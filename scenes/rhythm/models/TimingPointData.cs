using System.Text.Json.Serialization;

namespace Yam.scenes.rhythm.models;

public class TimingPointData
{
    [JsonInclude]
    public int Time;

    [JsonInclude]
    // ReSharper disable once InconsistentNaming
    public float BPM;

    [JsonInclude]
    public int Meter;

    private float _beatLength = -1;

    public float BeatLength
    {
        get
        {
            if (_beatLength <= 0)
            {
                _beatLength = 60000 / BPM;
            }

            return _beatLength;
        }
    }
}