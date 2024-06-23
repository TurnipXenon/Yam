using Yam.Core.Rhythm.Clients;
using Yam.Core.Rhythm.Servers;
using Yam.Core.Rhythm.Services.BeatPooler;

namespace Yam.Core.Rhythm.Services;

public static class ServiceInitializers
{
	public static IChartEditor CreateEditor(IRhythmGameHost host, IPooledBeatResource beatResource)
	{
		var editor = new ChartEditor
		{
			Host = host,
			BeatResource = beatResource
		};
		return editor;
	}
}