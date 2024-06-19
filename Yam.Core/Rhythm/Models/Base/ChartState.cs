namespace Yam.Core.Rhythm.Models.Base;

public record ChartState
{
	public ChartModel Chart { get; set; }
	public GeneralState State { get; set; }
	public TimingSection ActiveTimingSection { get; set; }
	public int ActiveTimingIndex { get; set; }

	public ChartState(ChartModel model, IAudioPosition player)
	{
		this.Chart = model;
		this.State = GeneralState.Active;
		this.ActiveTimingSection = model.TimingSections[0];
		this.ActiveTimingIndex = 0;

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
	}

	// todo: time calculated based on the current zoom distance and the Chart Model’s Approach rate
	public float PreemptTime { get; set; }

	public float AudioPosition => Player?.GetPlaybackPosition() ?? 0;

	public IAudioPosition Player { get; set; }
}