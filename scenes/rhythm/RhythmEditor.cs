using System.Diagnostics;
using System.Text.Json;
using Godot;
using Yam.scenes.rhythm.models.@base;

public partial class RhythmEditor : Node2D
{
    [Export] public Resource ChartResource { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ParseAndPlayChart();
    }

    private void ParseAndPlayChart()
    {
        Debug.Assert(ChartResource != null);
        var f = FileAccess.Open(ChartResource.ResourcePath, FileAccess.ModeFlags.Read);
        if (f == null)
        {
            GD.PrintErr("TODO: handle error in ReadMap");
            return;
        }

        var chartModel = JsonSerializer.Deserialize<ChartModel>(f.GetAsText());
        GD.Print(chartModel);
        // todo pass chart model to interpreter
    }
}