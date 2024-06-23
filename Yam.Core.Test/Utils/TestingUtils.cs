using Yam.Core.Rhythm.Models.Base;
using Yam.Core.Rhythm.Models.Wrappers;

namespace Yam.Core.Test.Utils;

public static class TestingUtils
{
	internal static BeatState NewBeatState()
	{
		return new BeatState(new BeatState.Props
		{
			TimingSection = new TimingSection(),
			BeatModel = new BeatModel()
		});
	}
}