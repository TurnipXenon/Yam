using Yam.Core.Rhythm.Models.Base;
using Yam.Core.Rhythm.Services;

namespace Yam.Core.Rhythm.Clients;

public interface IRhythmGameHost : IAudioPosition
{
	void PlaySong(string songPath);
	void RegisterListener(IGameListeners listener);
}