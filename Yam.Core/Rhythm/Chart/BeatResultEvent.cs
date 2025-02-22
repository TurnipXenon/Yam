namespace Yam.Core.Rhythm.Chart;

public class BeatResultEvent
{
    public IBeat? Beat;
    public BeatInputResult? Result;

    public BeatResultEvent()
    {
    }

    public BeatResultEvent(IBeat? beat, BeatInputResult? result)
    {
        Beat = beat;
        Result = result;
    }
}