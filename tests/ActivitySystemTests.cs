using System.Collections.Generic;
using System.Linq;
using GdUnit4;
using static GdUnit4.Assertions;
using hoardinggame.Core;

namespace hoardinggame.Core.Tests
{
    [TestSuite]
    public class ActivitySystemTests
    {
        [TestCase]
        public void ActivityStartsAtCorrectTime()
        {
            var engine = new GameEngine();
            var state = new GameState();
            var activity = new RotatePlayerActivity(1, 2, RotatePlayerInput.RotationDirection.Left);
            state.Activities.Add(activity);

            var result = engine.Step(state, new List<GameInput>(), new List<GameObservation>(), 1.0f);

            AssertThat(activity.HasStarted).IsTrue();
            AssertThat(activity.HasEnded).IsFalse();
        }

        [TestCase]
        public void ActivityEndsAtCorrectTime()
        {
            var engine = new GameEngine();
            var state = new GameState();
            var activity = new RotatePlayerActivity(1, 2, RotatePlayerInput.RotationDirection.Left);
            state.Activities.Add(activity);

            var result = engine.Step(state, new List<GameInput>(), new List<GameObservation>(), 2.0f);

            AssertThat(activity.HasStarted).IsTrue();
            AssertThat(activity.HasEnded).IsTrue();
            AssertThat(result.NewState.Activities).IsEmpty();
        }

        [TestCase]
        public void ActivityDoesNotStartBeforeStartTime()
        {
            var engine = new GameEngine();
            var state = new GameState();
            var activity = new RotatePlayerActivity(2, 3, RotatePlayerInput.RotationDirection.Left);
            state.Activities.Add(activity);

            var result = engine.Step(state, new List<GameInput>(), new List<GameObservation>(), 1.0f);

            AssertThat(activity.HasStarted).IsFalse();
            AssertThat(activity.HasEnded).IsFalse();
            AssertThat(result.NewState.Activities).HasSize(1);
        }

        [TestCase]
        public void MultipleActivitiesProcessCorrectly()
        {
            var engine = new GameEngine();
            var state = new GameState();
            var activity1 = new RotatePlayerActivity(1, 2, RotatePlayerInput.RotationDirection.Left);
            var activity2 = new RotatePlayerActivity(1.5f, 3, RotatePlayerInput.RotationDirection.Left);
            state.Activities.Add(activity1);
            state.Activities.Add(activity2);

            var result = engine.Step(state, new List<GameInput>(), new List<GameObservation>(), 2.5f);

            AssertThat(activity1.HasStarted).IsTrue();
            AssertThat(activity1.HasEnded).IsTrue();
            AssertThat(activity2.HasStarted).IsTrue();
            AssertThat(activity2.HasEnded).IsFalse();
            AssertThat(result.NewState.Activities).HasSize(1);
        }

        [TestCase]
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

            AssertThat(result.NewState.Activities).HasSize(2); // RotatePlayerActivity + LockInputActivity
            AssertThat(result.NewState.Activities.Any(a => a is RotatePlayerActivity)).IsTrue();
            AssertThat(result.NewState.Activities.Any(a => a is LockInputActivity)).IsTrue();
            AssertThat(result.NewState.PlayerRotation).IsEqual(0f); // Should not change until activity ends
        }

        [TestCase]
        public void RotatePlayerActivityUpdatesStateWhenComplete()
        {
            var engine = new GameEngine();
            var state = new GameState { PlayerRotation = 0f };
            var activity = new RotatePlayerActivity(1.0f, 2.0f, RotatePlayerInput.RotationDirection.Right);
            state.Activities.Add(activity);

            var result = engine.Step(state, new List<GameInput>(), new List<GameObservation>(), 2.0f);

            AssertThat(result.NewState.PlayerRotation).IsEqual(270f);
            AssertThat(result.NewState.Activities).IsEmpty();
        }

        [TestCase]
        public void StateClonePreservesActivities()
        {
            var originalState = new GameState();
            var activity = new RotatePlayerActivity(1, 2, RotatePlayerInput.RotationDirection.Left);
            originalState.Activities.Add(activity);

            var clonedState = originalState.Clone();

            AssertThat(clonedState.Activities).HasSize(1);
            AssertThat(clonedState.Activities).IsNotSame(originalState.Activities);
        }
    }
}