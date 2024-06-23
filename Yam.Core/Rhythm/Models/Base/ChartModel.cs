using System.Collections.Generic;
using System.Linq;

namespace Yam.Core.Rhythm.Models.Base;

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
			return;
		}

		SelfPath = chartResourceResourcePath.Remove(lastSlash);
	}
}