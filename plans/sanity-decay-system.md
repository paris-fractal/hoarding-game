# Sanity Decay System Implementation Plan

## Goal
Implement a sanity level system that gradually decays over time as the player progresses through the horror game.

## Current State
- `GameState.SanityLevel` already exists (float, initialized to 100f)
- `GameEngine.UpdateSanity()` method exists but is not called
- `HandleSanityTrigger()` handles external sanity modifications

## Design

### GameEngine Changes
- Call `UpdateSanity()` in the main `Step()` method to process sanity decay each frame
- The existing `UpdateSanity()` method already implements:
  - Gradual decay using `deltaTime * 0.1f` (10 sanity points per second)
  - Minimum bound of 0 (prevents negative sanity)

### GameState
- No changes needed - `SanityLevel` property already exists
- Already properly serialized/deserialized and cloned

### Decay Rate
- Current implementation: 0.1 sanity points per second (6 points per minute)
- This means full sanity (100) would decay to 0 in ~16.7 minutes
- Rate can be adjusted later based on gameplay testing

## Implementation Steps
1. Add `UpdateSanity(newState, (float)deltaTime, effects)` call to `GameEngine.Step()`
2. Write unit tests to verify sanity decay over time
3. Ensure sanity doesn't go below 0 or above initial maximum

## Benefits
- Creates mounting tension as sanity decreases
- Foundation for future sanity-based gameplay mechanics
- Simple, time-based system that works with existing state management