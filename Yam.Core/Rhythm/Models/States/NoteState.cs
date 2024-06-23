namespace Yam.Core.Rhythm.Models.States;

internal enum NoteType
{
	Normal,
	Downbeat // first note of a beat
}

internal class NoteState
{
	public float Timing { get; set; }
	public VisualizationState VisualizationState { get; set; } = VisualizationState.Unowned;
	public NoteType Type { get; set; } = NoteType.Normal;
}