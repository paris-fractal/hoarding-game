using System;
using System.Collections.Generic;
using System.Linq;

namespace hoardinggame.Core
{
    public class StepResult
    {
        public GameState NewState { get; set; } = new();
        public List<GameEffect> Effects { get; set; } = new();
    }

    public interface IGameEngine
    {
        StepResult Step(GameState currentState, List<GameInput> inputs, List<GameObservation> observations, double deltaTime);
    }

    public class GameEngine : IGameEngine
    {
        public StepResult Step(GameState currentState, List<GameInput> inputs, List<GameObservation> observations, double deltaTime)
        {
            var newState = currentState.Clone();
            var effects = new List<GameEffect>();

            newState.Time += deltaTime;

            // Process observations
            foreach (var observation in observations)
            {
                ProcessObservation(newState, observation);
            }

            // Process inputs
            if (!IsInputLocked(newState))
            {
                foreach (var input in inputs)
                {
                    ProcessInput(newState, input);
                }
            }

            // Process activities
            ProcessActivities(newState, effects);

            // Process sanity decay
            UpdateSanity(newState, (float)deltaTime, effects);

            return new StepResult
            {
                NewState = newState,
                Effects = effects
            };
        }

        private void ProcessInput(GameState state, GameInput input)
        {
            switch (input)
            {
                case OpenDoorInput openDoor:
                    HandleOpenDoor(state, openDoor);
                    break;
                case PickItemInput pickItem:
                    HandlePickItem(state, pickItem);
                    break;
                case MovePlayerInput movePlayer:
                    HandleMovePlayer(state, movePlayer);
                    break;
                case SellItemInput sellItem:
                    HandleSellItem(state, sellItem);
                    break;
                case BuyUpgradeInput buyUpgrade:
                    HandleBuyUpgrade(state, buyUpgrade);
                    break;
                case RotatePlayerInput rotatePlayer:
                    HandleRotatePlayer(state, rotatePlayer);
                    break;
            }
        }

        private void ProcessObservation(GameState state, GameObservation observation)
        {
            switch (observation)
            {
                case DoorApertureOccupiedObservation doorOccupied:
                    HandleDoorOccupied(state, doorOccupied);
                    break;
                case BodyCameToRestObservation bodyRest:
                    HandleBodyRest(state, bodyRest);
                    break;
                case PlayerReachedPositionObservation playerReached:
                    HandlePlayerReached(state, playerReached);
                    break;
                case ItemPickupCompletedObservation pickupCompleted:
                    HandlePickupCompleted(state, pickupCompleted);
                    break;
                case SanityTriggerObservation sanityTrigger:
                    HandleSanityTrigger(state, sanityTrigger);
                    break;
            }
        }

        private void HandleOpenDoor(GameState state, OpenDoorInput input)
        {
            // TODO: Implement door opening logic
        }

        private void HandlePickItem(GameState state, PickItemInput input)
        {
            // TODO: Implement item picking logic
        }

        private void HandleMovePlayer(GameState state, MovePlayerInput input)
        {
            // TODO: Implement player movement logic
            // Note: Input locking is now handled by LockInputActivity, not effects
        }

        private void HandleSellItem(GameState state, SellItemInput input)
        {
            // TODO: Implement item selling logic
            state.Money += 10; // Placeholder
        }

        private void HandleBuyUpgrade(GameState state, BuyUpgradeInput input)
        {
            // TODO: Implement upgrade purchasing logic
        }

        private void HandleRotatePlayer(GameState state, RotatePlayerInput input)
        {

            // Create a rotation activity instead of immediately updating state
            const float rotationDuration = 0.5f; // Duration for visual rotation
            float startTime = (float)state.Time;
            float endTime = startTime + rotationDuration;

            var rotationActivity = new RotatePlayerActivity(startTime, endTime, input.Direction);
            var lockInputActivity = new LockInputActivity(startTime, endTime);

            state.Activities.Add(rotationActivity);
            state.Activities.Add(lockInputActivity);
        }

        private void HandleDoorOccupied(GameState state, DoorApertureOccupiedObservation observation)
        {
            // TODO: Update door blocking state
        }

        private void HandleBodyRest(GameState state, BodyCameToRestObservation observation)
        {
            // TODO: Update physics state tracking
        }

        private void HandlePlayerReached(GameState state, PlayerReachedPositionObservation observation)
        {
        }

        private void HandlePickupCompleted(GameState state, ItemPickupCompletedObservation observation)
        {
            // TODO: Update inventory state
        }

        private void HandleSanityTrigger(GameState state, SanityTriggerObservation observation)
        {
            state.SanityLevel = Math.Max(0, state.SanityLevel + observation.SanityDelta);
        }

        private void UpdateSanity(GameState state, float deltaTime, List<GameEffect> effects)
        {
            // Gradual sanity decay over time
            state.SanityLevel = Math.Max(0, state.SanityLevel - deltaTime * 0.5f);
        }

        private void ProcessActivities(GameState state, List<GameEffect> effects)
        {
            float currentTime = (float)state.Time;

            // Process each activity
            foreach (var activity in state.Activities.ToList())
            {
                activity.TryStart(currentTime, state, effects);
                activity.TryEnd(currentTime, state, effects);
            }

            // Remove completed activities
            state.Activities.RemoveAll(activity => activity.HasEnded);
        }

        private bool IsInputLocked(GameState state)
        {
            return state.Activities.OfType<LockInputActivity>()
                .Any(lockActivity => lockActivity.IsInputLocked());
        }
    }
}