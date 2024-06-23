namespace Yam.Core.Rhythm.Models.Base;

public record TimingSection
{
	public float Timing { get; set; }
	public float BPM { get; set; }
	private bool wasBeatLenghtInitialized = false;
	private float _beatLength = -1f;

	public float BeatLength
	{
		get
		{
			if (!wasBeatLenghtInitialized)
			{
				_beatLength = 60f / (BPM * BeatsPerMeter);
				wasBeatLenghtInitialized = true;
			}

			return _beatLength;
		}
	}

	public int BeatsPerMeter { get; set; }
	public float ApproachRate { get; set; } = 5f;

	public static readonly TimingSection DefaultTimingSection = new()
	{
		Timing = float.PositiveInfinity
	};
}