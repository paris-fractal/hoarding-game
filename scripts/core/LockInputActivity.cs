namespace hoardinggame.Core
{
    public class LockInputActivity : Activity
    {
        public LockInputActivity() : base()
        {
            // Parameterless constructor for JSON serialization
        }

        public LockInputActivity(float start, float end) 
            : base(start, end)
        {
        }

        public override void OnStart(GameState state)
        {
            // Input locking begins - this would be checked by input processing
        }

        public override void OnEnd(GameState state)
        {
            // Input locking ends - input processing can resume normally
        }

        public bool IsInputLocked(float currentTime)
        {
            return HasStarted && !HasEnded && currentTime >= Start && currentTime < End;
        }
    }
}