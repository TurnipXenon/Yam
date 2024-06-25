using Godot;
using Yam.Core.Rhythm.Services.NotePooler;
using Vector2 = System.Numerics.Vector2;

namespace Yam.Godot.Scripts.Rhythm;

public partial class EditorResource : Node, IPooledNoteResource
{
	[Export] public RhythmEditorMain SceneManager { get; set; }
	[Export] public PackedScene NotePrefab { get; set; }

	public float GetPlaybackPosition() => SceneManager.GetPlaybackPosition();

	public float GetStreamLength() => SceneManager.GetStreamLength();

	public PooledNote RequestResource()
	{
		var pooledNote = NotePrefab.Instantiate<GodotPooledNote>();
		SceneManager.AddChild(pooledNote);
		return pooledNote.PooledNote;
	}

	public Vector2 GetSpawningPoint() => SceneManager.GetSpawningPoint();

	public Vector2 GetTriggerPoint() => SceneManager.GetTriggerPoint();

	public Vector2 GetDestructionPoint() => SceneManager.GetDestructionPoint();
}