using System.Diagnostics;
using Godot;
using Yam.Game.Scripts.Rhythm;

namespace Yam.Game.Scenes.Rhythm.RhythmPlayground.UI;

public partial class StartButton : Button
{
    [Export]
    public RhythmSimulator Simulator;

    public override void _Ready()
    {
        Debug.Assert(Simulator != null);
    }

    private void OnClick()
    {
        Hide();
        Simulator.StartChart();
    }
}