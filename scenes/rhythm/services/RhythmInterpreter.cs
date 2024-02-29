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

    private RhythmData _rhythmData;
    private ulong _incurredElapsedTime = 0;
    private ulong _preemptTime = 0;
    private ulong _startTime = 0;
    private bool _active;
    private Queue<HitObjectData> _hitObjectQueue;
    private Node _hitNode;

    public void SetRhythmData(RhythmData rhythmData)
    {
        _rhythmData = rhythmData;
    }

    public void Start(RhythmTestMain hostNode)
    {
        _hitObjectQueue = new Queue<HitObjectData>(_rhythmData.HitObjectList);
        _preemptTime = (ulong)(1200 + 600 * Mathf.Max(0, 5 - _rhythmData.ApproachRate) / 5);
        GD.Print($"Preempt: {_preemptTime}");

        _hostNode = hostNode;
        _incurredElapsedTime = 0;
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

        var elapsedTime = Time.GetTicksMsec() - (_startTime + _incurredElapsedTime);
        // var afterPreemptTime = elapsedTime - _preemptTime;

        // todo

        // todo: do we need to instantiate a HitObject?
        var latestHitObject = _hitObjectQueue.Peek();
        // should show?
        GD.Print($"{elapsedTime} >= {latestHitObject.Timing - _preemptTime}");
        if (elapsedTime >= latestHitObject.Timing - _preemptTime && _hitNode == null)
        {
            _hitNode = _hostNode.HitObjectPrefab.Instantiate();
            var hitObject = (HitObject)_hitNode;
            hitObject.SetData(latestHitObject, _hostNode, _preemptTime);
            _hostNode.AddChild(_hitNode);
        }

        // todo: how do we figure out the timeframe a hit object should exist?

        // todo: do we need to destroy a HitObject

        // todo: move all the existing HitObject
    }
}