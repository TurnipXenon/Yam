using System;
using Godot;
using Yam.Core.Rhythm.Models.States;
using Yam.Core.Rhythm.Services.NotePooler;
using SystemVector2 = System.Numerics.Vector2;

namespace Yam.Godot.Scripts.Rhythm;

public partial class GodotPooledNote : Sprite2D, IPooledNoteHost
{
	[Export] private Texture2D DownbeatNoteSprite;
	[Export] private Texture2D NormalNoteSprite;

	public PooledNote PooledNote;

	public override void _Ready()
	{
		PooledNote = new PooledNote(this);
	}

	public override void _Process(double delta)
	{
		PooledNote?.Tick();
	}

	public void Deactivate()
	{
		Visible = false;
		Position = new Vector2(-100, -100);
	}

	public void Activate(NoteType type)
	{
		switch (type)
		{
			case NoteType.Normal:
				Texture = NormalNoteSprite;
				break;
			case NoteType.Downbeat:
				Texture = DownbeatNoteSprite;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, null);
		}
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