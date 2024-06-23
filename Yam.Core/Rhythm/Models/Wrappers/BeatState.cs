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
	internal VisualizationState VisualizationState { get; set; }

	internal BeatState(BeatModel beatModel, TimingSection timingSection)
	{
		Beat = beatModel;
		TimingSection = timingSection;

		// calculation from osu: https://osu.ppy.sh/wiki/en/Beatmap/Approach_rate
		var preemptDuration = 0f;
		if (TimingSection.ApproachRate > 5)
		{
			preemptDuration = 1.2f - .75f * (TimingSection.ApproachRate - 5) / 5;
		}
		else
		{
			preemptDuration = 1.2f + .75f * (5 - TimingSection.ApproachRate) / 5;
		}

		PreemptTime = Beat.Timing - preemptDuration;
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