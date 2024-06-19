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

		this._player = player;
	}

	// todo: time calculated based on the current zoom distance and the Chart Model’s Approach rate
	public float PreemptTime { get; set; }

	public float AudioPosition => _player?.GetPlaybackPosition() ?? 0;

	private IAudioPosition _player;

	public void SetAudioPositionSource(IAudioPosition player)
	{
		_player = player;
	}
}