using System.Collections.Generic;
using Yam.Core.Rhythm.Models.Base;

namespace Yam.Core.Rhythm.Models.Wrappers;

public record ChartState
{
	public GeneralState State { get; set; }
	public TimingSection ActiveTimingSection { get; set; }
	public int ActiveTimingIndex { get; set; }

	private ChartModel Chart { get; set; }
	private List<BeatState> BeatStateList { get; set; } = new();

	public ChartState(ChartModel model, IAudioPosition player)
	{
		this.Chart = model;
		this.State = GeneralState.Active;
		this.ActiveTimingSection = model.TimingSections[0];
		this.ActiveTimingSection.ApproachRate = this.Chart.ApproachRate;
		this.ActiveTimingIndex = 0;

		// todo: might remove
		// calculation from osu: https://osu.ppy.sh/wiki/en/Beatmap/Approach_rate
		if (this.Chart.ApproachRate > 5)
		{
			this.PreemptTime = 1.2f - .75f * (this.Chart.ApproachRate - 5) / 5;
		}
		else
		{
			this.PreemptTime = 1.2f + .75f * (5 - this.Chart.ApproachRate) / 5;
		}

		this.Player = player;

		// todo: recreate the beats from the model and give them states, such that we know whether they are visualized or not, use the GeneralState enum to track this
		BeatStateList.Capacity = this.Chart.Beats.Count;
		this.Chart.Beats.ForEach(beatModel =>
		{
			// todo: in the future we might have timing sections
			BeatStateList.Add(new BeatState(new BeatState.Props
			{
				BeatModel = beatModel,
				TimingSection = this.ActiveTimingSection
			}));
		});
	}

	// todo: time calculated based on the current zoom distance and the Chart Model’s Approach rate
	public float PreemptTime { get; set; }

	public float AudioPosition => Player?.GetPlaybackPosition() ?? 0;

	public IAudioPosition Player { get; set; }

	internal BeatState GetBeatOrDefault(int index)
	{
		if (0 <= index && index < BeatStateList.Count)
		{
			return BeatStateList[index];
		}

		return BeatState.DefaultBeatState;
	}
}