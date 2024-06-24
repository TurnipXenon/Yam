using Yam.Core.Rhythm.Clients;
using Yam.Core.Rhythm.Servers;
using Yam.Core.Rhythm.Services.BeatPooler;
using Yam.Core.Rhythm.Services.NotePooler;

namespace Yam.Core.Rhythm.Services;

public static class ServiceInitializers
{
	public static IChartEditor CreateEditor(
		IRhythmGameHost host,
		IPooledBeatResource visualizerResource,
		IPooledNoteResource editorResource
	)
	{
		var editor = new ChartEditor
		{
			Host = host,
			ChartVisualizerResource = visualizerResource,
			EditorVisualizerResource = editorResource
		};
		return editor;
	}
}