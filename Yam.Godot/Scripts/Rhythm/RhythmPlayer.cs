using System.Text.Json;
using Godot;
using Yam.Core.Rhythm.Chart;
using ChartModel = Yam.Core.Rhythm.Chart.Chart;

namespace Yam.Godot.Scripts.Rhythm;

public partial class RhythmPlayer : Node
{
    [Export] public Resource Chart { get; set; }

    public override void _Ready()
    {
        // todo(turnip): remove and make it situational in the future
        // such that it is not triggered by events in here but called externally
        // by the scene manager
        ParseChart();
    }

    public override void _Process(double delta)
    {
        // todo(turnip): if the updating or processing logic here becomes too complex
        // extract the logic elsewhere???
     }

    private void ParseChart()
    {
        // todo(turnip): implement
        using var f = FileAccess.Open(Chart.ResourcePath, FileAccess.ModeFlags.Read);
        if (f == null)
        {
            GD.PrintErr("Chart: Missing Chart file");
            return;
        }

        var chartEntity = JsonSerializer.Deserialize<ChartEntity>(f.GetAsText());
        // todo: chartEntity to chart; we will have to process channels
        var chart = ChartModel.FromEntity(chartEntity);
        GD.Print("Done");
    }
}