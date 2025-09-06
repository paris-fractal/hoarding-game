using Godot;
using System;
using hoardinggame.Core;

public partial class Player : Node3D
{
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
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

	public override void _Process(double delta)
	{
	}
}
