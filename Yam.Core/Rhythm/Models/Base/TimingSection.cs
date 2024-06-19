namespace Yam.Core.Rhythm.Models.Base;

public record TimingSection
{
    public float Timing { get; set; }
    public float BPM { get; set; }
    public float BeatLength { get; set; }
    public int BeatsPerMeter { get; set; }
    public float ApproachRate { get; set; }
}