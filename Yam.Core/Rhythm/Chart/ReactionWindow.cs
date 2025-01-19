using Godot;

namespace Yam.Core.Rhythm.Chart;

public class ReactionWindow
{
    public float Threshold { get; set; }
    public Vector2 Range { get; set; }
    public BeatInputResult BeatInputResult { get; set; }

    public ReactionWindow(float threshold, BeatInputResult result)
    {
        Threshold = threshold;
        BeatInputResult = result;
    }
    
    public ReactionWindow(float threshold, BeatInputResult result, float origin)
    {
        Threshold = threshold;
        BeatInputResult = result;
        Range = new Vector2(origin - threshold, origin + threshold);
    }
}