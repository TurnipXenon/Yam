using System.Numerics;
using Yam.Core.Rhythm.Clients;
using Yam.Core.Rhythm.Models.States;
using Yam.Core.Rhythm.Services;
using Yam.Core.Rhythm.Services.NotePooler;

namespace Yam.Core.Rhythm.Servers;

internal class GhostBeatHandler : IGameListeners
{
	private readonly ChartEditorVisualizer _editor;
	private NoteState? _currentNote;
	private readonly IPooledNoteResource _resource;
	private readonly Vector2 _destructionPoint;
	private readonly Vector2 _triggerPoint;
	private readonly Vector2 _spawningPoint;
	private readonly IGhostBeat? _actor;

	public GhostBeatHandler(
		ChartEditorVisualizer editor,
		IRhythmGameHost host,
		IPooledNoteResource resource,
		IGhostBeat? actor = null
	)
	{
		_editor = editor;
		_resource = resource;
		_actor = actor;
		_spawningPoint = _resource.GetSpawningPoint();
		_triggerPoint = _resource.GetTriggerPoint();
		_destructionPoint = _resource.GetDestructionPoint();
		host.RegisterListener(this);
	}

	public void Tick(double delta)
	{
		// condition where ghost beat should be at current note if next.position > cursorPosition > current.position

		// todo: NOW: use the postiion calculation in PooledNote to get the current position of our next note
		// and the next note

		// todo find the best spot to place ourselves
		// todo: move the host

		if (_currentNote == null)
		{
			// todo: move the host out of site
			return;
		}

		// _currentNote.NextNote.
	}

	public void OnRewindOrNotesRecreated(NoteState startingNote)
	{
		// todo: maybe not needed
		// _currentNote = startingNote;
		// run to the very start!
	}
}