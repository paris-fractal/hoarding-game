using Godot;
using System;
using hoardinggame.Core;
using CommandLine;

public partial class Player : Node3D
{
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
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
			else if (keyEvent.Keycode == Key.M)
			{
				Orchestrator.Enqueue(new SpawnJunkInput());
			}
		}

		if (@event is InputEventMouseButton mouseButtonEvent
			&& mouseButtonEvent.ButtonIndex == MouseButton.Left
			&& mouseButtonEvent.Pressed
			&& Input.IsKeyPressed(Key.J))
		{
			HandleSpawnJunkClick(mouseButtonEvent.Position);
		}
	}

	public override void _Process(double delta)
	{
	}

	private void HandleSpawnJunkClick(Vector2 mousePosition)
	{
		GD.Print("Spawning Junk");
		var camera = GetNodeOrNull<Camera3D>("camera");
		if (camera == null)
		{
			GD.PrintErr("Player camera not found; cannot spawn junk.");
			return;
		}

		var spaceState = GetWorld3D()?.DirectSpaceState;
		if (spaceState == null)
		{
			GD.PrintErr("World space not available; cannot spawn junk.");
			return;
		}

		var origin = camera.ProjectRayOrigin(mousePosition);
		var direction = camera.ProjectRayNormal(mousePosition).Normalized();
		var target = origin + direction * 5.0f;

		var query = PhysicsRayQueryParameters3D.Create(origin, target);
		query.CollideWithAreas = true;
		query.CollideWithBodies = false;

		var hit = spaceState.IntersectRay(query);
		if (hit.Count == 0)
		{
			GD.Print("Did not intersect with any bodies");
			return;
		}

		if (!hit.ContainsKey("collider"))
		{
			GD.Print("No colliders intersected");
			return;
		}

		var colliderObj = hit["collider"].Obj;
		if (colliderObj is not Area3D area)
		{
			GD.Print("object is not area:" + colliderObj);
			return;
		}

		if (area.Name != "JunkPlane" && !area.IsInGroup("JunkPlane"))
		{
			GD.Print("object is not junkplane:" + colliderObj);
			return;
		}

		if (!hit.ContainsKey("position"))
		{
			GD.Print("no position");
			return;
		}

		object positionValue = hit["position"].Obj;
		if (positionValue is not Vector3 hitPosition)
		{
			GD.Print("no v3 position:" + positionValue);
			return;
		}

		var impulse = GenerateImpulseInPlane(area);

		GD.Print("Spwaning junk at " + hitPosition);
		Orchestrator.Enqueue(new SpawnJunkInput
		{
			PosX = hitPosition.X,
			PosY = hitPosition.Y,
			PosZ = hitPosition.Z,
			ImpulseX = impulse.X,
			ImpulseY = impulse.Y,
			ImpulseZ = impulse.Z,
			Timestamp = (float)Orchestrator.GetCurrentState().Time
		});
	}

	private Vector3 GenerateImpulseInPlane(Area3D plane)
	{
		var rng = new RandomNumberGenerator();
		rng.Randomize();

		var dir2D = new Vector2(rng.RandfRange(-1f, 1f), rng.RandfRange(-1f, 1f));
		if (dir2D.LengthSquared() < 0.001f)
		{
			dir2D = new Vector2(1f, 0f);
		}
		dir2D = dir2D.Normalized();

		var strength = rng.RandfRange(0.5f, 1.5f);

		var planeX = plane.GlobalTransform.Basis.X;
		var planeY = plane.GlobalTransform.Basis.Y;
		var inPlaneDirection = (planeX * dir2D.X + planeY * dir2D.Y).Normalized();

		return inPlaneDirection * strength;
	}
}
