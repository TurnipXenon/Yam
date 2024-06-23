using Yam.Core.Rhythm.Models.Base;
using Yam.Core.Rhythm.Models.States;

namespace Yam.Core.Test.Utils;

public static class TestingUtils
{
	internal static BeatState NewBeatState(float timing = 0f)
	{
		return new BeatState(
			timingSection: new TimingSection(),
			beatModel: new BeatModel
			{
				Timing = timing
			}
		);
	}
}