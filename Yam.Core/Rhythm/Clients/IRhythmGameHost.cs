using Yam.Core.Rhythm.Servers;
using Yam.Core.Rhythm.Services;

namespace Yam.Core.Rhythm.Clients;

public interface IRhythmGameHost
{
	float GetAudioPosition();
	void PlaySong(string songPath);
	void RegisterListener(IGameListeners listener);
}