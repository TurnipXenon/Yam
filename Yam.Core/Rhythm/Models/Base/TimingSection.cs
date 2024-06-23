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


	// calculation from osu: https://osu.ppy.sh/wiki/en/Beatmap/Approach_rate
	public float GetPreemptDuration()
	{
		if (ApproachRate > 5)
		{
			return 1.2f - .75f * (ApproachRate - 5) / 5;
		}

		return 1.2f + .75f * (5 - ApproachRate) / 5;
	}

	public static readonly TimingSection DefaultTimingSection = new()
	{
		Timing = float.PositiveInfinity
	};
}