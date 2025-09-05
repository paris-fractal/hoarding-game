using System;
using System.Collections.Generic;
using Xunit;
using hoardinggame.Core;

namespace hoardinggame.Core.Tests
{
    public class PlayerRotationTests
    {
        [Fact]
        public void InitialPlayerRotationIsZero()
        {
            var state = new GameState();
            Assert.Equal(0f, state.PlayerRotation);
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
            
            // Second step: allow activity to complete
            var result = engine.Step(intermediateResult.NewState, new List<GameInput>(), observations, 1.0);

            Assert.Equal(270f, result.NewState.PlayerRotation);
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

            Assert.Equal(90f, result.NewState.PlayerRotation);
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

            Assert.Equal(0f, result.NewState.PlayerRotation);
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

            Assert.Equal(270f, result.NewState.PlayerRotation);
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
            Assert.Equal(270f, finalResult.NewState.PlayerRotation);
        }

        [Fact]
        public void StateClonePreservesRotation()
        {
            var originalState = new GameState { PlayerRotation = 180f };
            var clonedState = originalState.Clone();

            Assert.Equal(180f, clonedState.PlayerRotation);
            Assert.NotSame(originalState, clonedState);
        }

        [Fact]
        public void StateJsonSerializationPreservesRotation()
        {
            var originalState = new GameState { PlayerRotation = 270f };
            var json = originalState.ToJson();
            var deserializedState = GameState.FromJson(json);

            Assert.Equal(270f, deserializedState.PlayerRotation);
        }
    }
}