using Godot;

namespace Yam.Godot.Scripts.Rhythm.RhythmData;

/**
 * todo(turnip): document better and differentiate from a point projected on an x-y coordinate
 *
 */
[GlobalClass]
public partial class DataPoint : Resource
{
    [Export] public float UCoordinate;
    [Export] public float Time = -1;
}