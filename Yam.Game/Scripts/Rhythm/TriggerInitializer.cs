using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace Yam.Game.Scripts.Rhythm;

/// <summary>
/// Trigger point reference and trigger visualizer references
/// </summary>
/// <remarks>
/// The max y coordinate is the coordinate of this Node during _Ready
/// </remarks>
public partial class TriggerInitializer : Node2D
{
    [Export] public PackedScene TriggerPrefab;
    [Export] public RhythmSimulator Simulator;

    public readonly List<TriggerVisualizer> Visualizers = new();

    public override void _Ready()
    {
        Debug.Assert(TriggerPrefab != null);
        Debug.Assert(Simulator != null);

        Visible = false;
        Simulator.OnStartChart += StartGame;
    }

    private void StartGame()
    {
        var chartModel = Simulator.ChartModel;
        foreach (var channel in chartModel.ChannelList)
        {
            var visualizer = TriggerPrefab.Instantiate<TriggerVisualizer>();
            visualizer.Initialize(channel, Simulator, GetParent(), this);
            Visualizers.Add(visualizer);
        }
    }
}