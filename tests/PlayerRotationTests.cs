using System;
using System.Collections.Generic;
using GdUnit4;
using hoardinggame.Core;

namespace hoardinggame.Core.Tests
{
    [TestSuite]
    public class PlayerRotationTests
    {
        [Fact]
        public void InitialPlayerRotationIsZero()
        {
            var state = new GameState();
            AssertThat(state.PlayerRotation).IsEqual(0f);
        }

        [Fact]
        public void TurnRightRotatesPlayerNegative90Degrees()
        {
            var engine = new GameEngine();
            var initialState = new GameState { PlayerRotation = 0f };

            var inputs = new List<GameInput>
            {
                new RotatePlayerInput { Direction = RotatePlayerInput.RotationDirection.Right }
            };
            var observations = new List<GameObservation>();

            // First step: process input and create activity
            var intermediateResult = engine.Step(initialState, inputs, observations, 0.0);
            AssertThat(intermediateResult.Effects).HasSize(1);
            AssertThat(intermediateResult.Effects[0]).IsInstanceOf<RotatePlayerEffect>();
            var rotateEffect = (RotatePlayerEffect)intermediateResult.Effects[0];
            AssertThat(rotateEffect.ToRotation).IsEqual(270F);
            AssertThat(intermediateResult.NewState.PlayerRotation).IsEqual(0f);

            // Second step: allow activity to complete
            var result = engine.Step(intermediateResult.NewState, new List<GameInput>(), observations, 1.0);

            AssertThat(result.NewState.PlayerRotation).IsEqual(270f);
        }

        [Fact]
        public void TurnLeftRotatesPlayerPositive90Degrees()
        {
            var engine = new GameEngine();
            var initialState = new GameState { PlayerRotation = 0f };

            var inputs = new List<GameInput>
            {
                new RotatePlayerInput { Direction = RotatePlayerInput.RotationDirection.Left }
            };
            var observations = new List<GameObservation>();

            // First step: process input and create activity
            var intermediateResult = engine.Step(initialState, inputs, observations, 0.0);

            // Second step: allow activity to complete
            var result = engine.Step(intermediateResult.NewState, new List<GameInput>(), observations, 1.0);

            AssertThat(result.NewState.PlayerRotation).IsEqual(90f);
        }

        [Fact]
        public void RotationWrapsAroundCorrectly()
        {
            var engine = new GameEngine();
            var initialState = new GameState { PlayerRotation = 270f };

            var inputs = new List<GameInput>
            {
                new RotatePlayerInput { Direction = RotatePlayerInput.RotationDirection.Left }
            };
            var observations = new List<GameObservation>();

            // First step: process input and create activity
            var intermediateResult = engine.Step(initialState, inputs, observations, 0.0);

            // Second step: allow activity to complete
            var result = engine.Step(intermediateResult.NewState, new List<GameInput>(), observations, 1.0);

            AssertThat(result.NewState.PlayerRotation).IsEqual(0f);
        }

        [Fact]
        public void RotationWrapsAroundNegativeCorrectly()
        {
            var engine = new GameEngine();
            var initialState = new GameState { PlayerRotation = 0f };

            var inputs = new List<GameInput>
            {
                new RotatePlayerInput { Direction = RotatePlayerInput.RotationDirection.Right }
            };
            var observations = new List<GameObservation>();

            // First step: process input and create activity
            var intermediateResult = engine.Step(initialState, inputs, observations, 0.0);

            // Second step: allow activity to complete
            var result = engine.Step(intermediateResult.NewState, new List<GameInput>(), observations, 1.0);

            AssertThat(result.NewState.PlayerRotation).IsEqual(270f);
        }

        [Fact]
        public void MultipleRotationsNowProcessSequentially()
        {
            var engine = new GameEngine();
            var initialState = new GameState { PlayerRotation = 0f };

            var observations = new List<GameObservation>();

            // First rotation input
            var firstInput = new List<GameInput>
            {
                new RotatePlayerInput { Direction = RotatePlayerInput.RotationDirection.Right }
            };

            // Process first input and create activity
            var firstResult = engine.Step(initialState, firstInput, observations, 0.0);

            // Try second rotation input while first is still active (should be blocked)
            var secondInput = new List<GameInput>
            {
                new RotatePlayerInput { Direction = RotatePlayerInput.RotationDirection.Right }
            };

            var secondResult = engine.Step(firstResult.NewState, secondInput, observations, 0.1);

            // Allow first activity to complete
            var finalResult = engine.Step(secondResult.NewState, new List<GameInput>(), observations, 1.0);

            // Only first rotation should be processed due to input locking
            AssertThat(finalResult.NewState.PlayerRotation).IsEqual(270f);
        }

        [Fact]
        public void StateClonePreservesRotation()
        {
            var originalState = new GameState { PlayerRotation = 180f };
            var clonedState = originalState.Clone();

            AssertThat(clonedState.PlayerRotation).IsEqual(180f);
            AssertThat(clonedState).IsNotSame(originalState);
        }

        [Fact]
        public void StateJsonSerializationPreservesRotation()
        {
            var originalState = new GameState { PlayerRotation = 270f };
            var json = originalState.ToJson();
            var deserializedState = GameState.FromJson(json);

            AssertThat(deserializedState.PlayerRotation).IsEqual(270f);
        }
    }
}