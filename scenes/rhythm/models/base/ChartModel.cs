using System;
using System.Collections.Generic;

namespace Yam.scenes.rhythm.models.@base;

public record ChartModel
{
	public string AudioRelativePath { get; set; }
	public string SelfPath { get; set; }
	public string SongName { get; set; }
	public float ApproachRate { get; set; }
	public List<BeatModel> Beats { get; set; } = new();
	public List<TimingSection> TimingSections { get; set; } = new();

	public void SetSelfPath(string chartResourceResourcePath)
	{
		var lastSlash = chartResourceResourcePath.LastIndexOf("/");
		if (lastSlash == -1)
		{
			Console.WriteLine($"Err: invalid self path format: {chartResourceResourcePath}");
			return;
		}

		SelfPath = chartResourceResourcePath.Remove(lastSlash);
		Console.WriteLine(SelfPath);
	}
}