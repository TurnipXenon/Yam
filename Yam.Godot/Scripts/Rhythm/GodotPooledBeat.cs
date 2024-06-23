using Godot;
using Yam.Core.Rhythm.Services.BeatPooler;

namespace Yam.Godot.Scripts.Rhythm;

public partial class GodotPooledBeat : Node2D, IPooledBeatHost
{
	public PooledBeat PooledBeat;

	public override void _Ready()
	{
		PooledBeat = new PooledBeat(this);
	}

	public override void _Process(double delta)
	{
		PooledBeat?.Tick();
	}

	public void Deactivate()
	{
		// todo: put it in a far away place no one can see and set visibility to invisible
		throw new System.NotImplementedException();
	}

	public void Activate()
	{
		// todo: do the opposite of deactivate
		throw new System.NotImplementedException();
	}

	public void DestroyResource()
	{
		QueueFree();
	}
}