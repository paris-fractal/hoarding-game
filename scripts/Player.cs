using Godot;
using System;
using hoardinggame.Core;

public partial class Player : Node3D
{
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			// Only process rotation inputs if we're not currently rotating
			var currentRotation = Orchestrator.GetCurrentState().PlayerRotation;
			if (Mathf.IsEqualApprox(currentRotation, this.RotationDegrees.Y, 1.0f))
			{
				if (keyEvent.Keycode == Key.A)
				{
					Orchestrator.Enqueue(new RotatePlayerInput
					{
						Direction = RotatePlayerInput.RotationDirection.Left,
						Timestamp = (float)Orchestrator.GetCurrentState().Time
					});
				}
				else if (keyEvent.Keycode == Key.D)
				{
					Orchestrator.Enqueue(new RotatePlayerInput
					{
						Direction = RotatePlayerInput.RotationDirection.Right,
						Timestamp = (float)Orchestrator.GetCurrentState().Time
					});
				}
			}
		}
	}

	public override void _Process(double delta)
	{
		var desiredYRotation = Orchestrator.GetCurrentState().PlayerRotation;

		if (!Mathf.IsEqualApprox(desiredYRotation, this.RotationDegrees.Y, 0.1f))
		{
			// Calculate shortest rotation path
			var rotationDifferential = desiredYRotation - this.RotationDegrees.Y;

			// Normalize to shortest path: if difference > 180°, go the other way
			if (rotationDifferential > 180f)
				rotationDifferential -= 360f;
			else if (rotationDifferential < -180f)
				rotationDifferential += 360f;

			rotationDifferential = Mathf.Clamp(rotationDifferential, -360 * (float)delta, 360 * (float)delta);
			this.RotationDegrees = this.RotationDegrees with { Y = this.RotationDegrees.Y + rotationDifferential };
			GD.Print("Rotating to " + desiredYRotation);
		}
	}
}
