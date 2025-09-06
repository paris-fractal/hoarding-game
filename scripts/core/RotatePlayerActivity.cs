using System.Collections.Generic;
using Godot;

namespace hoardinggame.Core
{
    public class RotatePlayerActivity : Activity
    {
        public RotatePlayerInput.RotationDirection Direction { get; set; }

        public RotatePlayerActivity() : base()
        {
            // Parameterless constructor for JSON serialization
        }

        public RotatePlayerActivity(float start, float end, RotatePlayerInput.RotationDirection direction)
            : base(start, end)
        {
            Direction = direction;
        }

        public override void OnStart(GameState state, List<GameEffect> effects)
        {
            // Create visual rotation effect
            var rotateEffect = new RotatePlayerEffect
            {
                FromRotation = state.PlayerRotation,
                ToRotation = CalculateTargetRotation(state.PlayerRotation),
                Duration = End - Start,
                Timestamp = state.Time
            };
            effects.Add(rotateEffect);
        }

        public override void OnEnd(GameState state, List<GameEffect> effects)
        {
            state.PlayerRotation = CalculateTargetRotation(state.PlayerRotation);
        }

        private float CalculateTargetRotation(float currentRotation)
        {
            float newRotation;

            if (Direction == RotatePlayerInput.RotationDirection.Right)
            {
                newRotation = currentRotation - 90f;
            }
            else
            {
                newRotation = currentRotation + 90f;
            }

            // Normalize to 0-360 range
            while (newRotation < 0f)
                newRotation += 360f;
            while (newRotation >= 360f)
                newRotation -= 360f;

            return newRotation;
        }
    }
}