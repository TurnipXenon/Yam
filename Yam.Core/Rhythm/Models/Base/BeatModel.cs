using System.Collections.Generic;
using System.Numerics;

namespace Yam.Core.Rhythm.Models.Base;

public record BeatModel
{
    public BeatType Type { get; set; }
    public float Timing { get; set; }
    public VisualizationState VisualizationState { get; set; }
    public List<BeatModel> Subparts { get; set; } = new();
    public Vector2 BezierStart { get; set; }
    public Vector2 BezierEnd { get; set; }
    public DirectionBit DirectionBit { get; set; }
}