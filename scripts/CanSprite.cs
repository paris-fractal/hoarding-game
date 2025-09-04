using Godot;
using System;

public partial class CanSprite : Sprite3D
{
	private Boolean toggle = false;
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent
			&& keyEvent.Pressed
			&& keyEvent.Keycode == Key.Space)
		{
			toggle = !toggle;
		}
	}

	public override void _Process(double delta)
	{
		if (toggle)
		{
			this.NoDepthTest = true;
			this.Scale = new Vector3(2, 2, 2);
		}
		else
		{
			this.NoDepthTest = false;
			this.Scale = new Vector3(1, 1, 1);
		}
	}
}
