using Yam.Core.Rhythm.Models.Wrappers;

namespace Yam.Core.Rhythm.Servers;

// the functions are virtual to allow Moq to create a proxy class for it
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
internal class BeatPooler
{
	public virtual void RequestBeat(BeatState beat)
	{
		// todo: implement
	}
}