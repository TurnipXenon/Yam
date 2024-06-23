using Yam.Core.Rhythm.Models.Base;

namespace Yam.Core.Rhythm.Models.Wrappers;

internal class BeatState
{
	// pattern: https://en.wikipedia.org/wiki/Null_object_pattern
	public static readonly BeatState DefaultBeatState = new(
		beatModel: BeatModel.NullBeatModel,
		timingSection: new TimingSection()
	);

	internal BeatModel Beat { get; set; }
	private TimingSection TimingSection { get; set; }
	internal float PreemptTime { get; set; }
	internal float PreemptDuration { get; }
	internal VisualizationState VisualizationState { get; set; }
	public float Timing => Beat.Timing;

	internal BeatState(BeatModel beatModel, TimingSection timingSection)
	{
		Beat = beatModel;
		TimingSection = timingSection;

		// calculation from osu: https://osu.ppy.sh/wiki/en/Beatmap/Approach_rate
		PreemptDuration = 0f;
		if (TimingSection.ApproachRate > 5)
		{
			PreemptDuration = 1.2f - .75f * (TimingSection.ApproachRate - 5) / 5;
		}
		else
		{
			PreemptDuration = 1.2f + .75f * (5 - TimingSection.ApproachRate) / 5;
		}

		PreemptTime = Beat.Timing - PreemptDuration;
	}


	public bool ShouldBePreEmpted(float playbackPosition)
	{
		if (Beat == BeatModel.NullBeatModel)
		{
			return false;
		}

		return playbackPosition > PreemptTime;
	}
}