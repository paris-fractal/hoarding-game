using System.Collections.Generic;
using Xunit;
using hoardinggame.Core;

namespace hoardinggame.Core.Tests
{
    public class ActivitySystemTests
    {
        [Fact]
        public void ActivityStartsAtCorrectTime()
        {
            var engine = new GameEngine();
            var state = new GameState();
            var activity = new TestActivity(1.0f, 2.0f);
            state.Activities.Add(activity);

            var result = engine.Step(state, new List<GameInput>(), new List<GameObservation>(), 1.0f);

            Assert.True(activity.HasStarted);
            Assert.False(activity.HasEnded);
        }

        [Fact]
        public void ActivityEndsAtCorrectTime()
        {
            var engine = new GameEngine();
            var state = new GameState();
            var activity = new TestActivity(1.0f, 2.0f);
            state.Activities.Add(activity);

            var result = engine.Step(state, new List<GameInput>(), new List<GameObservation>(), 2.0f);

            Assert.True(activity.HasStarted);
            Assert.True(activity.HasEnded);
            Assert.Empty(result.NewState.Activities);
        }

        [Fact]
        public void ActivityDoesNotStartBeforeStartTime()
        {
            var engine = new GameEngine();
            var state = new GameState();
            var activity = new TestActivity(2.0f, 3.0f);
            state.Activities.Add(activity);

            var result = engine.Step(state, new List<GameInput>(), new List<GameObservation>(), 1.0f);

            Assert.False(activity.HasStarted);
            Assert.False(activity.HasEnded);
            Assert.Single(result.NewState.Activities);
        }

        [Fact]
        public void MultipleActivitiesProcessCorrectly()
        {
            var engine = new GameEngine();
            var state = new GameState();
            var activity1 = new TestActivity(1.0f, 2.0f);
            var activity2 = new TestActivity(1.5f, 3.0f);
            state.Activities.Add(activity1);
            state.Activities.Add(activity2);

            var result = engine.Step(state, new List<GameInput>(), new List<GameObservation>(), 2.5f);

            Assert.True(activity1.HasStarted);
            Assert.True(activity1.HasEnded);
            Assert.True(activity2.HasStarted);
            Assert.False(activity2.HasEnded);
            Assert.Single(result.NewState.Activities);
        }

        [Fact]
        public void RotatePlayerInputCreatesActivity()
        {
            var engine = new GameEngine();
            var initialState = new GameState { PlayerRotation = 0f };
            
            var inputs = new List<GameInput>
            {
                new RotatePlayerInput { Direction = RotatePlayerInput.RotationDirection.Right }
            };
            var observations = new List<GameObservation>();

            var result = engine.Step(initialState, inputs, observations, 1.0f);

            Assert.Equal(2, result.NewState.Activities.Count); // RotatePlayerActivity + LockInputActivity
            Assert.Contains(result.NewState.Activities, a => a is RotatePlayerActivity);
            Assert.Contains(result.NewState.Activities, a => a is LockInputActivity);
            Assert.Equal(0f, result.NewState.PlayerRotation); // Should not change until activity ends
        }

        [Fact]
        public void RotatePlayerActivityUpdatesStateWhenComplete()
        {
            var engine = new GameEngine();
            var state = new GameState { PlayerRotation = 0f };
            var activity = new RotatePlayerActivity(1.0f, 2.0f, RotatePlayerInput.RotationDirection.Right);
            state.Activities.Add(activity);

            var result = engine.Step(state, new List<GameInput>(), new List<GameObservation>(), 2.0f);

            Assert.Equal(270f, result.NewState.PlayerRotation);
            Assert.Empty(result.NewState.Activities);
        }

        [Fact]
        public void StateClonePreservesActivities()
        {
            var originalState = new GameState();
            var activity = new TestActivity(1.0f, 2.0f);
            originalState.Activities.Add(activity);
            
            var clonedState = originalState.Clone();

            Assert.Single(clonedState.Activities);
            Assert.NotSame(originalState.Activities, clonedState.Activities);
        }
    }

    public class TestActivity : Activity
    {
        public bool HasStarted { get; private set; }
        public bool HasEnded { get; private set; }

        public TestActivity(float start, float end) : base(start, end)
        {
        }

        public override void OnStart(GameState state)
        {
            HasStarted = true;
        }

        public override void OnEnd(GameState state)
        {
            HasEnded = true;
        }
    }
}