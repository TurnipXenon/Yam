using System.Numerics;

namespace Yam.Core.Rhythm.Services.BeatPooler;

public interface IPooledBeatResource : IAudioPosition
{
	public PooledBeat RequestResource();
	public Vector2 GetSpawningPoint();
	public Vector2 GetTriggerPoint();
	public Vector2 GetDestructionPoint();
}