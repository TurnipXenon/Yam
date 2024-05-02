namespace Yam.scenes.rhythm.models.@base;

public record ChartState
{
    public ChartModel Chart { get; set; }
    public GeneralState State { get; set; }
    public TimingSection ActiveTimingSection { get; set; }
    public int ActiveTimingIndex { get; set; }

    // todo: time calculated based on the current zoom distance and the Chart Model’s Approach rate
    public float PreemptTime { get; set; }


    public float AudioPosition => _player?.GetPlaybackPosition() ?? 0;

    private IAudioPosition _player;

    public void SetAudioPositionSource(IAudioPosition player)
    {
        _player = player;
    }
}