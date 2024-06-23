using System.Collections.Generic;
using Yam.Core.Rhythm.Models.Base;
using Yam.Core.Rhythm.Services;

namespace Yam.Core.Rhythm.Models.Wrappers;

internal interface IChartState
{
	BeatState GetBeatOrDefault(int index);
}

internal record ChartState : IChartState
{
	private GeneralState State { get; set; }
	private TimingSection ActiveTimingSection { get; set; }
	private int ActiveTimingIndex { get; set; }

	private ChartModel Chart { get; set; }
	private List<BeatState> BeatStateList { get; set; } = new();

	public ChartState(ChartModel model, IAudioPosition player)
	{
		Chart = model;
		State = GeneralState.Active;
		ActiveTimingSection = model.TimingSections[0];
		ActiveTimingSection.ApproachRate = this.Chart.ApproachRate;
		ActiveTimingIndex = 0;

		// todo: recreate the beats from the model and give them states, such that we know whether they are visualized or not, use the GeneralState enum to track this
		BeatStateList.Capacity = Chart.Beats.Count;
		Chart.Beats.ForEach(beatModel =>
		{
			// todo: in the future we might have timing sections
			BeatStateList.Add(new BeatState(
				beatModel: beatModel,
				timingSection: ActiveTimingSection
			));
		});
	}

	public BeatState GetBeatOrDefault(int index)
	{
		if (0 <= index && index < BeatStateList.Count)
		{
			return BeatStateList[index];
		}

		return BeatState.DefaultBeatState;
	}
}