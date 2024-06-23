using System.Numerics;

namespace Yam.Core.Rhythm.Services.BeatPooler;

public interface IPooledBeatHost
{
	void Deactivate();
	void Activate();
	void DestroyResource();
	void SetPosition(Vector2 vector2);
}