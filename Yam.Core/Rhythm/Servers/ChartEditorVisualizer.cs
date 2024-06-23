using System.Collections.Generic;
using Yam.Core.Rhythm.Clients;
using Yam.Core.Rhythm.Models.Base;
using Yam.Core.Rhythm.Models.States;
using Yam.Core.Rhythm.Services;
using Yam.Core.Rhythm.Services.NotePooler;

namespace Yam.Core.Rhythm.Servers;

internal class ChartEditorVisualizer : IGameListeners
{
	private readonly IRhythmGameHost _host;

	private readonly IChartState _chartState;

	// private int currentLowerBound;
	internal List<NoteState> _tickStates = new();
	private BeatTickPooler _pooler;
	private bool isReady = false;

	// todo: add pooler
	public ChartEditorVisualizer(IRhythmGameHost host, IChartState chartState, IPooledNoteResource resource)
	{
		_host = host;
		_host.RegisterListener(this);
		_chartState = chartState; // todo: do we still need this?

		// todo: run everything below here in a coroutine
		// 

		// todo: initialize all beat ticks

		_pooler = new BeatTickPooler(resource);
		CreateTicks();
	}

	private async void CreateTicks()
	{
		CreateTicksTask(_tickStates);
	}

	internal List<NoteState> CreateTicksTask(List<NoteState> tickStates)
	{
		isReady = false;
		tickStates.Clear();
		var currentSectionIndex = -1;
		var currentTSLowerBound = _chartState.GetTimingSectionOrDefault(0);
		var currentTSUpperBound = _chartState.GetTimingSectionOrDefault(1);
		float currentTime = 0f;
		float currentBeatLength = 1f;
		int currentNoteIndex = 0;
		int maxNotesPerMeasure = 4;
		// todo: possibly add a yield here somewhere
		while (currentTime < _host.GetStreamLength())
		{
			if (currentSectionIndex == -1 || currentTime > currentTSUpperBound.Timing)
			{
				// will always go here for first try
				currentSectionIndex++;
				currentTSLowerBound = _chartState.GetTimingSectionOrDefault(currentSectionIndex);
				currentTSUpperBound = _chartState.GetTimingSectionOrDefault(currentSectionIndex + 1);
				currentTime = currentTSLowerBound.Timing;
				currentBeatLength = currentTSLowerBound.BeatLength;
				currentNoteIndex = 0;
				maxNotesPerMeasure = currentTSLowerBound.BeatsPerMeter;
			}

			tickStates.Add(new NoteState
			{
				Timing = currentTime,
				Type = currentNoteIndex == 0 ? NoteType.Downbeat : NoteType.Normal
			});
			currentTime += currentBeatLength;
			currentNoteIndex = (currentNoteIndex + 1) % maxNotesPerMeasure;
		}

		isReady = true;
		return tickStates;
	}


	public void Tick(double delta)
	{
		if (!isReady)
		{
			return;
		}
		// todo: let's run it in a couroutine or should our package should even know that implementation detail?
		// for (var i = 0; i < 5; i++)
		// {
		// 	var beat = _chartState.GetBeatOrDefault(currentLowerBound + i);
		// 	if (beat.ShouldBePreEmpted(_host.GetPlaybackPosition()))
		// 	{
		// 		_pooler.RequestBeat(beat);
		// 		if (i == 0)
		// 		{
		// 			// only increase lower bound if the requested beat was the lower bound
		// 			// a beat is lower bound if i = 0
		// 			currentLowerBound++;
		// 			i--;
		// 		}
		// 	}
		// }
	}
}