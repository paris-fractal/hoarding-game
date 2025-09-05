# Player Rotation State Migration

## Goal
Move player rotation state from `Player.cs` into the core state machine, so that `Player.cs` consults the `Orchestrator` to determine the desired rotation rather than managing it locally.

## Current State Analysis
- **Player.cs**: Contains `desiredYRotation` field and handles A/D key inputs directly, managing rotation interpolation locally
- **GameState.cs**: Already has `PlayerRotation` field but it's unused
- **GameEngine.cs**: Has `MovePlayerInput` with `TurnLeft`/`TurnRight` directions but rotation logic is not implemented
- **Orchestrator.cs**: Has infrastructure to process inputs and update state, but no rotation input handling

## Implementation Plan

### 1. Add Player Rotation Input
- Create `RotatePlayerInput` class in `Input.cs` with left/right rotation direction
- This will be sent from `Player.cs` when A/D keys are pressed

### 2. Implement Rotation Logic in GameEngine
- Add rotation handling in `ProcessInput()` method for the new `RotatePlayerInput`
- Update `GameState.PlayerRotation` field with immediate 90-degree snaps (0, 90, 180, 270)
- Normalize rotation values to stay within 0-360 range

### 3. Add Public State Access to Orchestrator
- Add `GetCurrentState()` method to `Orchestrator` so `Player.cs` can read the current game state
- Return the current rotation from `state.PlayerRotation`

### 4. Refactor Player.cs
- Remove local `desiredYRotation` field 
- In `_Input()`, send `RotatePlayerInput` to `Orchestrator` instead of updating local state
- In `_Process()`, read desired rotation from `Orchestrator.GetCurrentState().PlayerRotation`
- Keep the smooth interpolation logic but use the core state as the target

### 5. Integration Points
- Player inputs go: `Player.cs` → `Orchestrator.Enqueue()` → `GameEngine.ProcessInput()` → `GameState.PlayerRotation`
- Player rendering reads: `Player.cs` → `Orchestrator.GetCurrentState().PlayerRotation` → smooth interpolation

## Success Criteria
- A/D key presses update `GameState.PlayerRotation` immediately through the core state machine
- `Player.cs` has no local rotation state, only reads from `Orchestrator`
- Smooth rotation interpolation still works as before
- Player rotation state is part of save/load through the core state serialization