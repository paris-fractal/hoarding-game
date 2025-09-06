using System;
using Godot;

namespace hoardinggame.Core
{
	public abstract class GameEffect
	{
		public double Timestamp { get; set; }

		public abstract void Execute();
	}

	public class PlayAnimEffect : GameEffect
	{
		public string NodeId { get; set; } = string.Empty;
		public string AnimationName { get; set; } = string.Empty;
		public float Duration { get; set; }

		public override void Execute()
		{
			var node = Orchestrator.Instance.GetNode(NodeId);
			// TODO: Implement animation playing
		}
	}

	public class SfxEffect : GameEffect
	{
		public string SoundId { get; set; } = string.Empty;
		public float Volume { get; set; } = 1.0f;
		public float Pitch { get; set; } = 1.0f;

		public override void Execute()
		{
			// TODO: Implement sound playing
		}
	}

	public class SpawnLooseEffect : GameEffect
	{
		public string ItemId { get; set; } = string.Empty;
		public float PosX { get; set; }
		public float PosY { get; set; }
		public float PosZ { get; set; }
		public bool IsDynamic { get; set; } = true;

		public override void Execute()
		{
			// TODO: Implement item spawning
		}
	}

	public class DespawnEffect : GameEffect
	{
		public string ItemId { get; set; } = string.Empty;

		public override void Execute()
		{
			// TODO: Implement item despawning
		}
	}

	public class TransitionEffect : GameEffect
	{
		public int ToRoomId { get; set; }
		public int FacingDirection { get; set; }
		public float Duration { get; set; } = 0.5f;

		public override void Execute()
		{
			// TODO: Implement room transition
		}
	}

	public class UpdateUIEffect : GameEffect
	{
		public string UIElement { get; set; } = string.Empty;
		public object Data { get; set; } = new();

		public override void Execute()
		{
			// TODO: Implement UI updates
		}
	}

	public class ApplyImpulseEffect : GameEffect
	{
		public string BodyId { get; set; } = string.Empty;
		public float ForceX { get; set; }
		public float ForceY { get; set; }
		public float ForceZ { get; set; }

		public override void Execute()
		{
			var body = Orchestrator.Instance.GetNode(BodyId);
			// TODO: Implement physics impulse
		}
	}

	public class RotatePlayerEffect : GameEffect
	{
		public float FromRotation { get; set; }
		public float ToRotation { get; set; }
		public float Duration { get; set; }

		public override void Execute()
		{
			GD.Print("rotating to " + ToRotation);
			var player = Orchestrator.Instance.GetNode<Node3D>("Player");
			if (FromRotation == 270 && ToRotation == 0)
			{
				GD.Print("resetting rotation to -90");
				player.RotationDegrees = player.RotationDegrees with { Y = -90 };
			}
			if (FromRotation == 0 && ToRotation == 270)
			{
				GD.Print("resetting rotation to 360");
				player.RotationDegrees = player.RotationDegrees with { Y = 360 };

			}
			player.CreateTween()
				.TweenProperty(player, "rotation:y", Mathf.DegToRad(ToRotation), Duration)
				.SetTrans(Godot.Tween.TransitionType.Cubic)
				.SetEase(Godot.Tween.EaseType.InOut);
		}
	}
}
