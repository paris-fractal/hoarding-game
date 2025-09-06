using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace hoardinggame.Core
{
    public abstract class Activity
    {
        public float Start { get; set; }
        public float End { get; set; }
        public bool HasStarted { get; set; }
        public bool HasEnded { get; set; }

        protected Activity()
        {
            // Parameterless constructor for JSON serialization
        }

        protected Activity(float start, float end)
        {
            if (end <= start)
                throw new ArgumentException("End time must be after start time");

            Start = start;
            End = end;
            HasStarted = false;
            HasEnded = false;
        }

        public void TryStart(float currentTime, GameState state, List<GameEffect> effects)
        {
            if (!HasStarted && currentTime >= Start)
            {
                HasStarted = true;
                OnStart(state, effects);
            }
        }

        public void TryEnd(float currentTime, GameState state, List<GameEffect> effects)
        {
            if (HasStarted && !HasEnded && currentTime >= End)
            {
                HasEnded = true;
                OnEnd(state, effects);
            }
        }

        public abstract void OnStart(GameState state, List<GameEffect> effects);
        public abstract void OnEnd(GameState state, List<GameEffect> effects);
    }
}