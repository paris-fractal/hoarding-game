using System;
using System.Collections.Generic;
using GdUnit4;
using hoardinggame.Core;

namespace hoardinggame.Core.Tests
{
    [TestSuite]
    public class SanityDecayTests
    {
        [TestCase]
        public void InitialSanityLevelIsOneHundred()
        {
            var state = new GameState();
            AssertThat(state.SanityLevel).IsEqual(100f);
        }

        [TestCase]
        public void SanityDecaysOverTime()
        {
            var engine = new GameEngine();
            var initialState = new GameState { SanityLevel = 100f };

            var inputs = new List<GameInput>();
            var observations = new List<GameObservation>();

            // Run for 1 second
            var result = engine.Step(initialState, inputs, observations, 1.0);

            // Should decay by 0.5 per second, so 100 - 0.5 = 99.5
            AssertThat(result.NewState.SanityLevel).IsEqualApproximately(99.5f, 0.1f);
        }

        [TestCase]
        public void SanityDecaysAtCorrectRate()
        {
            var engine = new GameEngine();
            var initialState = new GameState { SanityLevel = 100f };

            var inputs = new List<GameInput>();
            var observations = new List<GameObservation>();

            // Run for 10 seconds
            var result = engine.Step(initialState, inputs, observations, 10.0);

            // Should decay by 5.0 over 10 seconds (0.5 per second)
            AssertThat(result.NewState.SanityLevel).IsEqualApproximately(95.0f, 0.1f);
        }

        [TestCase]
        public void SanityCannotGoBelowZero()
        {
            var engine = new GameEngine();
            var initialState = new GameState { SanityLevel = 0.25f }; // Very low sanity

            var inputs = new List<GameInput>();
            var observations = new List<GameObservation>();

            // Run for 1 second, which would decay by 0.5
            var result = engine.Step(initialState, inputs, observations, 1.0);

            // Should be clamped at 0, not negative
            AssertThat(result.NewState.SanityLevel).IsEqual(0f);
        }

        [TestCase]
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

            // Should decay by approximately 0.5 over 1 total second
            AssertThat(state.SanityLevel).IsEqualApproximately(99.5f, 0.01f);
        }

        [TestCase]
        public void StateClonePreservesSanityLevel()
        {
            var originalState = new GameState { SanityLevel = 75.5f };
            var clonedState = originalState.Clone();

            AssertThat(clonedState.SanityLevel).IsEqual(75.5f);
            AssertThat(clonedState).IsNotSame(originalState);
        }

        [TestCase]
        public void StateJsonSerializationPreservesSanityLevel()
        {
            var originalState = new GameState { SanityLevel = 42.3f };
            var json = originalState.ToJson();
            var deserializedState = GameState.FromJson(json);

            AssertThat(deserializedState.SanityLevel).IsEqual(42.3f);
        }

        [TestCase]
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
            AssertThat(result.NewState.SanityLevel).IsEqual(40f);
        }

        [TestCase]
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

            // Should be: 50 (initial) - 5 (trigger) - 0.5 (1 second decay) = 44.5
            AssertThat(result.NewState.SanityLevel).IsEqualApproximately(44.5f, 0.1f);
        }
    }
}