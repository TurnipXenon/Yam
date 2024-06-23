using System.Collections.Generic;
using System.Numerics;

namespace Yam.Core.Rhythm.Models.Base;

public record BeatModel
{
	// -10 is an arbitrary number far away from edge cases around 0f
	private const float NullTiming = -10;

	// pattern: https://en.wikipedia.org/wiki/Null_object_pattern
	public static readonly BeatModel NullBeatModel = new()
	{
		Timing = NullTiming
	};

	public BeatType Type { get; set; }
	public float Timing { get; set; }
	public List<BeatModel> Subparts { get; set; } = new();
	public Vector2 BezierStart { get; set; }
	public Vector2 BezierEnd { get; set; }
	public DirectionBit DirectionBit { get; set; }
}