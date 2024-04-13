using System.Collections.Generic;
using Godot;
using Yam.scenes.rhythm.models;

namespace Yam.scenes.rhythm.services;

public class RhythmInterpreter
{
    /// <summary>
    /// <c>_hostNode</c> allows this service to have access to Godot stuff
    /// </summary>
    /// todo: improve this by making it reference an interface instead of
    /// directly to Godot being able to migrate out of Godot if need 
    private RhythmTestMain _hostNode;

    public ChartMetadata Chart;
    public float PreemptTime = 0;
    private float _startTime = 0;
    private bool _active;
    private List<HitObjectData> _hitObjectList;
    private List<HitObject> _beatLineList = new();
    private int _beatIndex = 0;
    public float AudioPosition;

    public void SetActiveChart(ChartMetadata chartMetadata)
    {
        Chart = chartMetadata;
    }

    /// <summary>
    ///  todo: improve
    /// </summary>
    public TimingPointData CurrentTiming =>
        Chart.TimingPointList.Count > 0
            ? Chart.TimingPointList[0]
            : new TimingPointData();

    public void Start(RhythmTestMain hostNode)
    {
        _hostNode = hostNode;

        // Play audio
        using var file = FileAccess.Open(Chart.AudioPath, FileAccess.ModeFlags.Read);
        var sound = new AudioStreamMP3();
        sound.Data = file.GetBuffer((long)file.GetLength());
        _hostNode.AudioPlayer!.Stream = sound;
        _hostNode.AudioPlayer.Play();

        _hitObjectList = new List<HitObjectData>(Chart.HitObjectList);
        PreemptTime = (1.2f + 0.6f * Mathf.Max(0, 5 - Chart.ApproachRate) / 5);
        _startTime = Time.GetTicksMsec();

        _active = true;
    }

    public void Process()
    {
        if (!_active)
        {
            return;
        }

        if (_hostNode.AudioPlayer.Playing)
        {
            AudioPosition = _hostNode.AudioPlayer.GetPlaybackPosition();
        }

        if (Input.IsActionJustPressed("toggle_pause"))
        {
            if (_hostNode.AudioPlayer.Playing)
            {
                _hostNode.AudioPlayer.Stop();
            }
            else
            {
                _hostNode.AudioPlayer.Play(AudioPosition);
            }
        }


        // todo: do we need to instantiate a HitObject?
        if (_beatIndex < _hitObjectList.Count)
        {
            var latestHitObject = _hitObjectList[_beatIndex];
            // should show?
            if (AudioPosition >= latestHitObject.Timing - PreemptTime)
            {
                var hitNode = _hostNode.HitObjectPrefab.Instantiate();
                var hitObject = (HitObject)hitNode;
                hitObject.SetData(latestHitObject, _hostNode, PreemptTime, this);
                _hostNode.AddChild(hitNode);
                _beatIndex++;
            }
        }
    }
}