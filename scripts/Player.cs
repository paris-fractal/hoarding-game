using Godot;
using System;

public partial class Player : Node3D
{
	private float desiredYRotation = 0;

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent
			&& keyEvent.Pressed
			&& desiredYRotation == this.RotationDegrees.Y)
		{
			if (keyEvent.Keycode == Key.A)
			{
				desiredYRotation += 90;
			}
			if (keyEvent.Keycode == Key.D)
			{
				desiredYRotation -= 90;
			}
		}
	}

	public override void _Process(double delta)
	{
		if (desiredYRotation != this.RotationDegrees.Y)
		{
			var rotationDifferential = desiredYRotation - this.RotationDegrees.Y;
			rotationDifferential = Mathf.Clamp(rotationDifferential, -360 * (float)delta, 360 * (float)delta);
			this.RotationDegrees = this.RotationDegrees with { Y = this.RotationDegrees.Y + rotationDifferential };
			GD.Print("Rotating to " + desiredYRotation);
		}
	}
}
