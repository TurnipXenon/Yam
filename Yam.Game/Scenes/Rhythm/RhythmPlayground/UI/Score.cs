using System;
using System.Diagnostics;
using System.Globalization;
using Godot;
using Yam.Core.Rhythm.Chart;
using Yam.Game.Scripts.Rhythm;

namespace Yam.Game.Scenes.Rhythm.RhythmPlayground.UI;

public partial class Score : Label
{
    [Export] public RhythmSimulator RhythmSimulator;

    public float ScoreValue { private set; get; }
    private const int ZeroPadding = 5;
    private string _scoreFormat = "00000";

    public override void _Ready()
    {
        Debug.Assert(RhythmSimulator != null, "RhythmSimulator != null");
        RhythmSimulator.BeatSimulationResultEvent += OnBeatResult;
    }

    private void OnBeatResult(object sender, BeatResultEvent e)
    {
        ScoreValue += BeatInputResultUtil.GetScore(e.Result);
        Text = ((int)Math.Round(ScoreValue)).ToString(CultureInfo.InvariantCulture)
            .PadLeft(ZeroPadding, '0');
    }

    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
        {
            RhythmSimulator.BeatSimulationResultEvent -= OnBeatResult;
        }
    }
}