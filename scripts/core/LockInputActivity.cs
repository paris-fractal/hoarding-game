using System.Collections.Generic;
using Godot;

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

        public override void OnStart(GameState state, List<GameEffect> effects)
        {
            // Input locking begins - this would be checked by input processing
        }

        public override void OnEnd(GameState state, List<GameEffect> effects)
        {
            // Input locking ends - input processing can resume normally
        }

        public bool IsInputLocked()
        {
            return HasStarted && !HasEnded;
        }
    }
}