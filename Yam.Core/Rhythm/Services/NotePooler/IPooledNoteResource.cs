using System.Numerics;

namespace Yam.Core.Rhythm.Services.NotePooler;

public interface IPooledNoteResource : IAudioPosition
{
	public PooledNote RequestResource();
	public Vector2 GetSpawningPoint();
	public Vector2 GetTriggerPoint();
	public Vector2 GetDestructionPoint();
}