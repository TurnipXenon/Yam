using Godot;
using Yam.Core.Rhythm.Services.BeatPooler;
using SystemVector2 = System.Numerics.Vector2;

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
		Visible = false;
		Position = new Vector2(-100, -100);
	}

	public void Activate()
	{
		Visible = true;
	}

	public void DestroyResource()
	{
		QueueFree();
	}

	public void SetPosition(SystemVector2 vector2)
	{
		Position = new Vector2(vector2.X, vector2.Y);
	}
}