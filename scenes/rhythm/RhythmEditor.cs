using System.Linq;
using Godot;
using Yam.scenes.rhythm.models.@base;

public partial class Main : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var chartModel = new ChartModel();
        var b = new BeatModel
        {
            Type = BeatType.Tap,
            Timing = 1.818f
        };
        chartModel.Beats.Add(b);
        chartModel.Beats.Add(b);
        var s = System.Text.Json.JsonSerializer.Serialize(chartModel);
        GD.Print("Test ", s);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}