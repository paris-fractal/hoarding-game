using System;
using System.Collections.Generic;

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
                ProcessObservation(newState, observation, effects);
            }

            // Process inputs
            foreach (var input in inputs)
            {
                ProcessInput(newState, input, effects);
            }

            return new StepResult
            {
                NewState = newState,
                Effects = effects
            };
        }

        private void ProcessInput(GameState state, GameInput input, List<GameEffect> effects)
        {
            switch (input)
            {
                case OpenDoorInput openDoor:
                    HandleOpenDoor(state, openDoor, effects);
                    break;
                case PickItemInput pickItem:
                    HandlePickItem(state, pickItem, effects);
                    break;
                case MovePlayerInput movePlayer:
                    HandleMovePlayer(state, movePlayer, effects);
                    break;
                case SellItemInput sellItem:
                    HandleSellItem(state, sellItem, effects);
                    break;
                case BuyUpgradeInput buyUpgrade:
                    HandleBuyUpgrade(state, buyUpgrade, effects);
                    break;
            }
        }

        private void ProcessObservation(GameState state, GameObservation observation, List<GameEffect> effects)
        {
            switch (observation)
            {
                case DoorApertureOccupiedObservation doorOccupied:
                    HandleDoorOccupied(state, doorOccupied, effects);
                    break;
                case BodyCameToRestObservation bodyRest:
                    HandleBodyRest(state, bodyRest, effects);
                    break;
                case PlayerReachedPositionObservation playerReached:
                    HandlePlayerReached(state, playerReached, effects);
                    break;
                case ItemPickupCompletedObservation pickupCompleted:
                    HandlePickupCompleted(state, pickupCompleted, effects);
                    break;
                case SanityTriggerObservation sanityTrigger:
                    HandleSanityTrigger(state, sanityTrigger, effects);
                    break;
            }
        }

        private void HandleOpenDoor(GameState state, OpenDoorInput input, List<GameEffect> effects)
        {
            // TODO: Implement door opening logic
            effects.Add(new PlayAnimEffect
            {
                NodeId = input.DoorId,
                AnimationName = "open",
                Duration = 0.25f,
                Timestamp = state.Time
            });
        }

        private void HandlePickItem(GameState state, PickItemInput input, List<GameEffect> effects)
        {
            // TODO: Implement item picking logic
            effects.Add(new SfxEffect
            {
                SoundId = "pickup",
                Volume = 0.7f,
                Timestamp = state.Time
            });
        }

        private void HandleMovePlayer(GameState state, MovePlayerInput input, List<GameEffect> effects)
        {
            // TODO: Implement player movement logic
            effects.Add(new LockInputEffect
            {
                Duration = 0.5f,
                Timestamp = state.Time
            });
        }

        private void HandleSellItem(GameState state, SellItemInput input, List<GameEffect> effects)
        {
            // TODO: Implement item selling logic
            state.Money += 10; // Placeholder
        }

        private void HandleBuyUpgrade(GameState state, BuyUpgradeInput input, List<GameEffect> effects)
        {
            // TODO: Implement upgrade purchasing logic
        }

        private void HandleDoorOccupied(GameState state, DoorApertureOccupiedObservation observation, List<GameEffect> effects)
        {
            // TODO: Update door blocking state
        }

        private void HandleBodyRest(GameState state, BodyCameToRestObservation observation, List<GameEffect> effects)
        {
            // TODO: Update physics state tracking
        }

        private void HandlePlayerReached(GameState state, PlayerReachedPositionObservation observation, List<GameEffect> effects)
        {
        }

        private void HandlePickupCompleted(GameState state, ItemPickupCompletedObservation observation, List<GameEffect> effects)
        {
            // TODO: Update inventory state
        }

        private void HandleSanityTrigger(GameState state, SanityTriggerObservation observation, List<GameEffect> effects)
        {
            state.SanityLevel = Math.Max(0, state.SanityLevel + observation.SanityDelta);
        }

        private void UpdateSanity(GameState state, float deltaTime, List<GameEffect> effects)
        {
            // Gradual sanity decay over time
            state.SanityLevel = Math.Max(0, state.SanityLevel - deltaTime * 0.1f);
        }
    }
}