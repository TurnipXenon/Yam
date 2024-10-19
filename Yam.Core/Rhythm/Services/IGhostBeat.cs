using System.Numerics;

namespace Yam.Core.Rhythm.Services;

public interface IGhostBeat
{
	void SetPosition(Vector2 position);
	Vector2 GetDesiredPosition();
}