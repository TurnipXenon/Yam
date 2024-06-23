using System.Collections.Generic;
using System.Threading.Tasks;
using Yam.Core.Rhythm.Clients;
using Yam.Core.Rhythm.Models.States;
using Yam.Core.Rhythm.Services;
using Yam.Core.Rhythm.Services.NotePooler;

namespace Yam.Core.Rhythm.Servers;

internal class ChartEditorVisualizer : IGameListeners
{
	private readonly IRhythmGameHost _host;

	private readonly IChartState _chartState;

	private int _currentLowerBound;
	internal List<NoteState> _noteStates = new();
	private NotePooler _pooler;
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

		_pooler = new NotePooler(resource);
		CreateTicks();
	}

	private async void CreateTicks()
	{
		isReady = false;
		await Task.Yield(); // force async
		CreateTicksTask(_noteStates);
		isReady = true;
	}

	internal List<NoteState> CreateTicksTask(List<NoteState> tickStates)
	{
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

			tickStates.Add(new NoteState(currentTSLowerBound)
			{
				Timing = currentTime,
				Type = currentNoteIndex == 0 ? NoteType.Downbeat : NoteType.Normal
			});
			currentTime += currentBeatLength;
			currentNoteIndex = (currentNoteIndex + 1) % maxNotesPerMeasure;
		}

		return tickStates;
	}

	private NoteState GetNoteOrDefault(int index)
	{
		if (0 <= index && index < _noteStates.Count)
		{
			return _noteStates[index];
		}

		return NoteState.DefaultNoteState;
	}


	public void Tick(double delta)
	{
		if (!isReady)
		{
			return;
		}

		// todo: let's run it in a couroutine or should our package should even know that implementation detail?
		for (var i = 0; i < 5; i++)
		{
			var note = GetNoteOrDefault(_currentLowerBound + i);
			if (note.ShouldBePreEmpted(_host.GetPlaybackPosition()))
			{
				_pooler.RequestNote(note);
				if (i == 0)
				{
					// only increase lower bound if the requested beat was the lower bound
					// a beat is lower bound if i = 0
					_currentLowerBound++;
					i--;
				}
			}
		}
	}
}