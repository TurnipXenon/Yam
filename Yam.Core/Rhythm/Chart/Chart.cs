using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Yam.Core.Rhythm.Chart;

public class Chart
{
    public const int ChannelSize = 5;

    public List<Channel> ChannelList { get; set; } = new(ChannelSize);

    private Chart()
    {
        for (var i = 0; i < ChannelSize; i++)
        {
            ChannelList.Add(new Channel());
        }
    }

    // todo(turnip): create a test for this
    public static Chart FromEntity(ChartEntity chartEntity)
    {
        var chart = new Chart();
        chartEntity.BeatList.ForEach(beatEntity =>
        {
            var beat = Beat.FromEntity(beatEntity);

            var wasAdded = false;
            for (var index = 0; index < chart.ChannelList.Count; index++)
            {
                var channel = chart.ChannelList[index];
                if (channel.Count == 0 || !channel.Last().Overlaps(beat))
                {
                    wasAdded = true;
                    channel.Add(beat);
                    break;
                }
            }

            if (!wasAdded)
            {
                GD.PrintErr($"Beat not added: {beat.Time}");
            }

            // todo: take note of this logic
            // if the last beat in the list overlaps with the beat, add to another channel
        });

        return chart;
    }

    public List<Beat> GetVisualizableBeats(IRhythmPlayer rhythmPlayer)
    {
        List<Beat> beats = new();
        ChannelList.ForEach(b =>
        {
            var nb = b.TryToGetBeatToVisualize(rhythmPlayer);
            if (nb != null)
            {
                beats.Add(nb);
            }
        });
        return beats;
    }
}