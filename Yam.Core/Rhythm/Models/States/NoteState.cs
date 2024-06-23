using System;
using Yam.Core.Rhythm.Models.Base;

namespace Yam.Core.Rhythm.Models.States;

internal enum NoteType
{
	Normal,
	Downbeat // first note of a beat
}

internal class NoteState
{
	private const float NullTiming = -10f;

	public float Timing { get; set; }
	public VisualizationState VisualizationState { get; set; } = VisualizationState.Unowned;
	public NoteType Type { get; set; } = NoteType.Normal;
	public float PreemptDuration { get; }
	private bool preemptTimeCalculated = false;

	public float PreemptTime
	{
		get
		{
			if (!preemptTimeCalculated)
			{
				_preemptTime = Timing - PreemptDuration;
				preemptTimeCalculated = true;
			}

			return _preemptTime;
		}
	}

	internal NoteState(TimingSection timingSection)
	{
		PreemptDuration = timingSection.GetPreemptDuration();
	}

	public static readonly NoteState DefaultNoteState = new(TimingSection.DefaultTimingSection)
		{ Timing = NullTiming - 1f }; // make sure the timing is below null timing

	private float _preemptTime;

	public bool ShouldBePreEmpted(float playbackPosition)
	{
		if (Timing < NullTiming)
		{
			return false;
		}

		return playbackPosition > PreemptTime;
	}
}