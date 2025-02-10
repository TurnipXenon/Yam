using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Yam.Core.Common;
using Yam.Core.Rhythm.Input;

namespace Yam.Core.Rhythm.Chart;

public class Chart
{
    private GameLogger _logger = new();

    public GameLogger Logger
    {
        get => _logger;
        set
        {
            _logger = value;
            ChannelList.ForEach(c => c.Logger = _logger);
        }
    }

    public const int ChannelSize = 2;

    public List<BeatChannel> ChannelList { get; set; } = new(ChannelSize);

    private Chart(ITestOutputHelper? xUnitLogger)
    {
        for (var i = 0; i < ChannelSize; i++)
        {
            ChannelList.Add(new BeatChannel(xUnitLogger));
        }
    }

    public static Chart FromEntity(ChartEntity chartEntity,
        List<ReactionWindow> reactionWindow,
        ITestOutputHelper? xUnitLogger = null)
    {
        var chart = new Chart(xUnitLogger);

        if (xUnitLogger != null)
        {
            chart.Logger.XUnitLogger = xUnitLogger;
        }

        chartEntity.BeatList.ForEach(beatEntity =>
        {
            var beat = Beat.FromEntity(beatEntity, reactionWindow, xUnitLogger);

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

    // todo(turnip): create a test for this

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

    private MultiHoldInput? _multiHoldInput;

    public void SimulateBeatInput(IRhythmSimulator rhythmSimulator, IRhythmInput realInput)
    {
        if (realInput.GetSource() == InputSource.Unknown)
        {
            return;
        }

        // ingestedInput will be overriden under certain conditions below
        var ingestedInput = realInput;

        if (_multiHoldInput != null
            && realInput.GetSource() == InputSource.Player
            && !realInput.IsValidDirection())
        {
            if (_multiHoldInput.AddInput(realInput))
            {
                ingestedInput = _multiHoldInput;
            }
        }
        else if (_multiHoldInput != null)
        {
            // do nothing
        }
        else if (realInput.GetSource() == InputSource.Player)
        {
            // the sorting and input grouping is only important for input from player
            var shouldSort = false;
            var lastTime = -100f;
            foreach (var beatChannel in ChannelList)
            {
                if (lastTime > beatChannel.GetLatestInputTime())
                {
                    shouldSort = true;
                }

                lastTime = beatChannel.GetLatestInputTime();
            }

            if (shouldSort)
            {
                ChannelList.Sort((a, b) => a.GetLatestInputTime().CompareTo(b.GetLatestInputTime()));
            }

            // we need to be from smallest to largest when doing this
            // so a sort should happen first
            var similarTimeDetected = false;
            var lastChannel = ChannelList[0];
            List<BeatChannel> similarChannelList = new();
            for (var i = 1; i < ChannelList.Count; i++)
            {
                var currentChannel = ChannelList[i];
                if (currentChannel.GetLatestInputTime() == 0)
                {
                    continue;
                }

                if (Math.Abs(currentChannel.GetLatestInputTime() - lastChannel.GetLatestInputTime()) < float.Epsilon)
                {
                    if (!similarTimeDetected)
                    {
                        similarChannelList.Add(lastChannel);
                        similarTimeDetected = true;
                    }

                    similarChannelList.Add(currentChannel);
                }
                else if (similarTimeDetected)
                {
                    break;
                }

                lastChannel = currentChannel;
            }

            if (similarChannelList.Count > 0 &&
                similarChannelList.Exists(c => c.TryToGetBeatForInput()?.GetBeatType() == BeatType.Hold))
            {
                // todo: create fake input??? if one is a hold
                Logger.Print($"Detected equal inputs: {similarChannelList.Count}");
                _multiHoldInput = new MultiHoldInput(rhythmSimulator, realInput);
                ingestedInput = _multiHoldInput;
            }

            // todo(turnip): check if at least one hold is in the list
        }

        ChannelList.ForEach(c => c.SimulateBeatInput(rhythmSimulator, ingestedInput));

        if (_multiHoldInput != null && _multiHoldInput.IsStillAcceptingInputs())
        {
            _multiHoldInput = null;
        }
    }
}