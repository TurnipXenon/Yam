namespace Yam.Core.Rhythm.Chart;

public class BeatResultEvent
{
    public Beat? Beat;
    public BeatInputResult? Result;

    public BeatResultEvent()
    {
    }

    public BeatResultEvent(Beat? beat, BeatInputResult? result)
    {
        Beat = beat;
        Result = result;
    }
}