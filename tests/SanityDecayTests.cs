using System;
using System.Collections.Generic;
using Xunit;
using hoardinggame.Core;

namespace hoardinggame.Core.Tests
{
    public class SanityDecayTests
    {
        [Fact]
        public void InitialSanityLevelIsOneHundred()
        {
            var state = new GameState();
            Assert.Equal(100f, state.SanityLevel);
        }

        [Fact]
        public void SanityDecaysOverTime()
        {
            var engine = new GameEngine();
            var initialState = new GameState { SanityLevel = 100f };

            var inputs = new List<GameInput>();
            var observations = new List<GameObservation>();

            // Run for 1 second
            var result = engine.Step(initialState, inputs, observations, 1.0);

            // Should decay by 0.1 per second, so 100 - 0.1 = 99.9
            Assert.Equal(99.9f, result.NewState.SanityLevel, 1);
        }

        [Fact]
        public void SanityDecaysAtCorrectRate()
        {
            var engine = new GameEngine();
            var initialState = new GameState { SanityLevel = 100f };

            var inputs = new List<GameInput>();
            var observations = new List<GameObservation>();

            // Run for 10 seconds
            var result = engine.Step(initialState, inputs, observations, 10.0);

            // Should decay by 1.0 over 10 seconds (0.1 per second)
            Assert.Equal(99.0f, result.NewState.SanityLevel, 1);
        }

        [Fact]
        public void SanityCannotGoBelowZero()
        {
            var engine = new GameEngine();
            var initialState = new GameState { SanityLevel = 0.05f }; // Very low sanity

            var inputs = new List<GameInput>();
            var observations = new List<GameObservation>();

            // Run for 1 second, which would decay by 0.1
            var result = engine.Step(initialState, inputs, observations, 1.0);

            // Should be clamped at 0, not negative
            Assert.Equal(0f, result.NewState.SanityLevel);
        }

        [Fact]
        public void SanityDecayWorksWithSmallTimeSteps()
        {
            var engine = new GameEngine();
            var state = new GameState { SanityLevel = 100f };

            var inputs = new List<GameInput>();
            var observations = new List<GameObservation>();

            // Run 100 steps of 0.01 seconds each (total 1 second)
            for (int i = 0; i < 100; i++)
            {
                var result = engine.Step(state, inputs, observations, 0.01);
                state = result.NewState;
            }

            // Should decay by approximately 0.1 over 1 total second
            Assert.Equal(99.9f, state.SanityLevel, 2);
        }

        [Fact]
        public void StateClonePreservesSanityLevel()
        {
            var originalState = new GameState { SanityLevel = 75.5f };
            var clonedState = originalState.Clone();

            Assert.Equal(75.5f, clonedState.SanityLevel);
            Assert.NotSame(originalState, clonedState);
        }

        [Fact]
        public void StateJsonSerializationPreservesSanityLevel()
        {
            var originalState = new GameState { SanityLevel = 42.3f };
            var json = originalState.ToJson();
            var deserializedState = GameState.FromJson(json);

            Assert.Equal(42.3f, deserializedState.SanityLevel);
        }

        [Fact]
        public void SanityTriggerStillWorks()
        {
            var engine = new GameEngine();
            var initialState = new GameState { SanityLevel = 50f };

            var inputs = new List<GameInput>();
            var observations = new List<GameObservation>
            {
                new SanityTriggerObservation { SanityDelta = -10f }
            };

            var result = engine.Step(initialState, inputs, observations, 0.0);

            // Should be reduced by trigger (50 - 10 = 40)
            Assert.Equal(40f, result.NewState.SanityLevel);
        }

        [Fact]
        public void SanityDecayAndTriggersWorkTogether()
        {
            var engine = new GameEngine();
            var initialState = new GameState { SanityLevel = 50f };

            var inputs = new List<GameInput>();
            var observations = new List<GameObservation>
            {
                new SanityTriggerObservation { SanityDelta = -5f }
            };

            // Run for 1 second with both decay and trigger
            var result = engine.Step(initialState, inputs, observations, 1.0);

            // Should be: 50 (initial) - 5 (trigger) - 0.1 (1 second decay) = 44.9
            Assert.Equal(44.9f, result.NewState.SanityLevel, 1);
        }
    }
}