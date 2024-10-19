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
	public bool IsReady => _isReady;
	private bool _isReady = false;
	private GhostBeatHandler _ghostHandler;
	private readonly IPooledNoteResource _resource;
	private readonly IGhostBeat? _ghostBeat;

	// todo: add pooler
	public ChartEditorVisualizer(
		IRhythmGameHost host,
		IChartState chartState,
		IPooledNoteResource resource,
		NotePooler? notePooler = null,
		IGhostBeat? ghostBeat = null
	)
	{
		_host = host;
		_host.RegisterListener(this);
		_chartState = chartState; // todo: do we still need this?
		_resource = resource;
		_ghostBeat = ghostBeat;

		_pooler = notePooler ?? new NotePooler(resource);
		CreateNotes();
	}

	private async void CreateNotes()
	{
		_isReady = false;
		await Task.Yield(); // force async
		CreateNotesTask(_noteStates);
		_isReady = true;
		_ghostHandler = new GhostBeatHandler(this, _host, _resource, _ghostBeat);
	}

	internal List<NoteState> CreateNotesTask(List<NoteState> tickStates)
	{
		tickStates.Clear();
		var currentSectionIndex = -1;
		var currentTSLowerBound = _chartState.GetTimingSectionOrDefault(0);
		var currentTSUpperBound = _chartState.GetTimingSectionOrDefault(1);
		float currentTime = 0f;
		float currentBeatLength = 1f;
		int currentNoteIndex = 0;
		int maxNotesPerMeasure = 4;
		NoteState lastNote = null;
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

			var newNote = new NoteState(currentTSLowerBound)
			{
				Timing = currentTime,
				Type = currentNoteIndex == 0 ? NoteType.Downbeat : NoteType.Normal,
				PreviousNote = lastNote
			};
			tickStates.Add(newNote);
			currentTime += currentBeatLength;
			currentNoteIndex = (currentNoteIndex + 1) % maxNotesPerMeasure;

			// for allowing GhostBeatHandler to figure out next note without costing log(N)
			if (lastNote != null)
			{
				lastNote.NextNote = newNote;
			}

			lastNote = newNote;
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
		if (!_isReady)
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


	public void OnRewind()
	{
		// todo: add test
		// we want to find either the first beat visualized or the first beat that should be destroyed
		_currentLowerBound = 0;
		PooledNote? lastNote = null;
		while (lastNote == null || !lastNote.IsDestroyable())
		{
			var note = GetNoteOrDefault(_currentLowerBound);
			if (note.VisualizationState == VisualizationState.Visualized)
			{
				// it already exists so, let's just skip
				break;
			}

			if (note.ShouldBePreEmpted(_host.GetPlaybackPosition()))
			{
				lastNote = _pooler.RequestNote(note);
			}
			else if (note == NoteState.DefaultNoteState)
			{
				// we ran out of beats to show so just break
				break;
			}
		}
	}
}