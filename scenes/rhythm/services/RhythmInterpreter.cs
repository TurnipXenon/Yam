using System.Collections.Generic;
using Godot;
using Yam.scenes.rhythm.models;

namespace Yam.scenes.rhythm.services;

public class RhythmInterpreter
{
    private const int ObjectWindowSize = 5;

    /// <summary>
    /// <c>_hostNode</c> allows this service to have access to Godot stuff
    /// </summary>
    /// todo: improve this by making it reference an interface instead of
    /// directly to Godot being able to migrate out of Godot if need 
    private RhythmTestMain _hostNode;

    private ChartMetadata _chartMetadata;
    private float _preemptTime = 0;
    private float _startTime = 0;
    private bool _active;
    private List<HitObjectData> _hitObjectList;
    private int index = 0;
    public float AudioPosition;

    public void SetActiveChart(ChartMetadata chartMetadata)
    {
        _chartMetadata = chartMetadata;
    }

    public void Start(RhythmTestMain hostNode)
    {
        _hostNode = hostNode;
        // Play audio
        using var file = FileAccess.Open(_chartMetadata.AudioPath, FileAccess.ModeFlags.Read);
        var sound = new AudioStreamMP3();
        sound.Data = file.GetBuffer((long)file.GetLength());
        _hostNode.AudioPlayer!.Stream = sound;
        _hostNode.AudioPlayer.Play();

        _hitObjectList = new List<HitObjectData>(_chartMetadata.HitObjectList);
        _preemptTime = (1.2f + 0.6f * Mathf.Max(0, 5 - _chartMetadata.ApproachRate) / 5);
        GD.Print("Preempt", _preemptTime);
        _startTime = Time.GetTicksMsec();
        // todo: play song

        _active = true;
    }

    public void Process()
    {
        if (!_active)
        {
            return;
        }
        // todo: prioritize timingpoints
        // todo: SlideMultiplier property
        // todo: beatLength
        // todo: Length property?
        // todo: get duration

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
        if (index < _hitObjectList.Count)
        {
            var latestHitObject = _hitObjectList[index];
            // should show?
            if (AudioPosition >= latestHitObject.Timing - _preemptTime)
            {
                GD.Print(latestHitObject.Timing);
                var hitNode = _hostNode.HitObjectPrefab.Instantiate();
                var hitObject = (HitObject)hitNode;
                hitObject.SetData(latestHitObject, _hostNode, _preemptTime, this);
                _hostNode.AddChild(hitNode);
                index++;
            }
        }

        // todo: how do we figure out the timeframe a hit object should exist?

        // todo: do we need to destroy a HitObject

        // todo: move all the existing HitObject
    }
}