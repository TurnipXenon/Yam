namespace Yam.Core.Rhythm.Services.BeatPooler;

public interface IPooledBeatHost
{
	void Deactivate();
	void Activate();
	void DestroyResource();
}