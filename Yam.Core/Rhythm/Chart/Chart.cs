using System.Collections.Generic;
using System.Linq;
using Yam.Core.Common;
using Yam.Core.Rhythm.Input;

namespace Yam.Core.Rhythm.Chart;

public class Chart
{
    public GameLogger Logger = new();
    
    public const int ChannelSize = 5;

    public List<BeatChannel> ChannelList { get; set; } = new(ChannelSize);

    private Chart()
    {
        for (var i = 0; i < ChannelSize; i++)
        {
            ChannelList.Add(new BeatChannel());
        }
    }

    // todo(turnip): create a test for this
    public static Chart FromEntity(ChartEntity chartEntity, List<ReactionWindow> reactionWindow)
    {
        var chart = new Chart();
        chartEntity.BeatList.ForEach(beatEntity =>
        {
            var beat = Beat.FromEntity(beatEntity, reactionWindow);

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
                // todo(turnip): fix static call to logger figure out how to cancel logs in static context???
                chart.Logger.PrintErr($"Beat not added: {beat.Time}");
            }

            // todo: take note of this logic
            // if the last beat in the list overlaps with the beat, add to another channel
        });

        return chart;
    }

    public List<Beat> GetVisualizableBeats(IRhythmSimulator rhythmSimulator)
    {
        List<Beat> beats = new();
        ChannelList.ForEach(b =>
        {
            var nb = b.TryToGetBeatToVisualize(rhythmSimulator);
            if (nb != null)
            {
                beats.Add(nb);
            }
        });
        return beats;
    }

    public void SimulateBeatInput(IRhythmSimulator rhythmSimulator, IRhythmInput input)
    {
        ChannelList.Sort((a, b) => a.GetLatestInputTime().CompareTo(b.GetLatestInputTime()));

        ChannelList.ForEach(c => c.SimulateBeatInput(rhythmSimulator, input));
    }
}