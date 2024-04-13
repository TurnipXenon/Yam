using System.Text.Json.Serialization;

namespace Yam.scenes.rhythm.models;

/// <summary>
/// <c>HitObject</c> (or we could generically as <c>Beat</c>) is the representation
/// of a musical beat that players should react to.
/// </summary>
public class HitObjectData
{
    /// <summary>
    /// Type of the current <c>HitObject</c> based on osu file format
    /// </summary>
    /// <remarks>
    /// Source: https://osu.ppy.sh/wiki/en/Client/File_formats/osu_%28file_format%29#type
    /// </remarks>
    public enum Type
    {
        Unknown = -1,
        Unmarked = 0,
        HitCircle = 1,
        Slide = 2,
        HoldStart = 3,
        HoldMiddle = 4,
        HoldRelease = 5,
        SlideToHoldRelease = 6,
    }

    /// <summary>
    /// <c>Timing</c> is the time in milliseconds when the <c>HitObject</c> should be interacted with.
    /// </summary>
    /// <remarks>
    /// <c>Timing</c> is the duration (in milliseconds) from the beginning of the song up to the time that the
    /// <c>HitObject</c> should exactly be interacted with. Or visually, should be displayed (with best effort)
    /// as the exact point in time when the object should be interacted with.
    /// </remarks>
    [JsonInclude]
    public float Timing;

    [JsonInclude]
    public int Y;

    [JsonInclude]
    public Type HitObjectType;

    [JsonInclude]
    public int Channel;

    [JsonInclude]
    public bool HoldCurvePriority = false;

    [JsonInclude]
    public int BezCurveStartX;

    [JsonInclude]
    public int BezCurveStartY;

    [JsonInclude]
    public int BezCurveEndX;

    [JsonInclude]
    public int BezCurveEndY;
}