using System;
using Godot;

namespace hoardinggame
{
    public partial class Junk : RigidBody3D
    {
        public string GameStateId { get; set; } = "";

        private Area3D junkPlane;
        private BoxShape3D planeBounds;

        public override void _Ready()
        {
            junkPlane = FindParentJunkPlane();
            planeBounds = GetPlaneBoxShape(junkPlane);

            if (junkPlane == null)
            {
                GD.PrintErr($"{Name}: no JunkPlane ancestor found; plane constraints disabled.");
                return;
            }

            ApplyPlaneAlignedRotation();
        }

        public override void _IntegrateForces(PhysicsDirectBodyState3D state)
        {
            var startPose = this.GlobalPosition.ToString();
            if (junkPlane == null)
            {
                return;
            }

            var localPosition = junkPlane.ToLocal(state.Transform.Origin);

            if (planeBounds != null)
            {
                var halfExtents = planeBounds.Size / 2f;
                localPosition.X = Mathf.Clamp(localPosition.X, -halfExtents.X, halfExtents.X);
                localPosition.Y = Mathf.Clamp(localPosition.Y, -halfExtents.Y, halfExtents.Y);
            }

            localPosition.Z = 0f;
            var globalPosition = junkPlane.ToGlobal(localPosition);

            var planeAlignedBasis = Basis.FromEuler(new Vector3(0, GetPlaneYaw(), 0));
            state.Transform = new Transform3D(planeAlignedBasis, globalPosition);

            var localVelocity = junkPlane.GlobalTransform.Basis.Inverse() * state.LinearVelocity;
            localVelocity.Z = 0f;

            if (planeBounds != null)
            {
                var halfExtents = planeBounds.Size / 2f;
                if (Mathf.Abs(localPosition.X + halfExtents.X) < 0.0001f && localVelocity.X < 0) localVelocity.X = 0;
                if (Mathf.Abs(localPosition.X - halfExtents.X) < 0.0001f && localVelocity.X > 0) localVelocity.X = 0;
                if (Mathf.Abs(localPosition.Y + halfExtents.Y) < 0.0001f && localVelocity.Y < 0) localVelocity.Y = 0;
                if (Mathf.Abs(localPosition.Y - halfExtents.Y) < 0.0001f && localVelocity.Y > 0) localVelocity.Y = 0;
            }

            state.LinearVelocity = junkPlane.GlobalTransform.Basis * localVelocity;
        }

        private Area3D FindParentJunkPlane()
        {
            Node current = GetParent();
            while (current != null)
            {
                if (current is Area3D area && (area.Name == "JunkPlane" || area.IsInGroup("JunkPlane")))
                {
                    return area;
                }
                current = current.GetParent();
            }

            return null;
        }

        private BoxShape3D GetPlaneBoxShape(Area3D plane)
        {
            if (plane == null)
            {
                return null;
            }

            foreach (Node child in plane.GetChildren())
            {
                if (child is CollisionShape3D shape && shape.Shape is BoxShape3D boxShape)
                {
                    return boxShape;
                }
            }

            return null;
        }

        private float GetPlaneYaw()
        {
            if (junkPlane == null)
            {
                return 0f;
            }

            var forward = -junkPlane.GlobalTransform.Basis.Z;
            forward.Y = 0f;

            if (forward.LengthSquared() < 0.0001f)
            {
                return 0f;
            }

            forward = forward.Normalized();
            return Mathf.Atan2(forward.X, forward.Z);
        }

        private void ApplyPlaneAlignedRotation()
        {
            Rotation = new Vector3(0, GetPlaneYaw(), 0);
        }
    }
}
