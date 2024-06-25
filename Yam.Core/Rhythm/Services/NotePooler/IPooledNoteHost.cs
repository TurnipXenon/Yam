using System.Numerics;
using Yam.Core.Rhythm.Models.States;

namespace Yam.Core.Rhythm.Services.NotePooler;

public interface IPooledNoteHost
{
	void Deactivate();
	void Activate(NoteType noteType);
	void DestroyResource();
	void SetPosition(Vector2 vector2);
}