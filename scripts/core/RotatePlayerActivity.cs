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

        public override void OnStart(GameState state)
        {
            // Visual rotation animation would be triggered here
            // This would be handled by the game's visual system
        }

        public override void OnEnd(GameState state)
        {
            // Update the player's rotation in the game state
            float currentRotation = state.PlayerRotation;
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

            state.PlayerRotation = newRotation;
        }
    }
}