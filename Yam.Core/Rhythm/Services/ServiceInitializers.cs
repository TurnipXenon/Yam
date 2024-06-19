using Yam.Core.Rhythm.Clients;
using Yam.Core.Rhythm.Servers;

namespace Yam.Core.Rhythm.Services;

public static class ServiceInitializers
{
	public static IChartEditor CreateEditor(IRhythmGameHost host)
	{
		var editor = new ChartEditor
		{
			Host = host
		};
		return editor;
	}
}