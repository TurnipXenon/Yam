using System.Collections.Generic;

namespace Yam.scenes.rhythm.models.@base;

public record ChartModel
{
    public string AudioRelativePath { get; set; }
    public string SelfPath { get; set; }
    public string SongName { get; set; }
    public float ApproachRate { get; set; }
    public List<BeatModel> Beats { get; set; }
    public List<TimingSection> TimingSections { get; set; }
}