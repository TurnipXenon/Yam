using System.Numerics;

namespace Yam.Core.Rhythm.Services.NotePooler;

public interface IPooledNoteHost
{
	void Deactivate();
	void Activate();
	void DestroyResource();
	void SetPosition(Vector2 vector2);
}