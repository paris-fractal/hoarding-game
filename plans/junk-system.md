# Junk System Implementation Plan

## Goal
Implement a junk system that tracks all loose items in the world with position, rotation, and unique identifiers.

## Design

### Junk Record
- Create `Junk` record with:
  - `Id` (string) - unique identifier  
  - `PosX`, `PosY`, `PosZ` (float) - world position
  - `RotX`, `RotY`, `RotZ` (float) - rotation in degrees

### GameState Changes
- Add `List<Junk> JunkItems` property to track all junk in the world
- Update `Clone()` method to properly clone the junk list
- Ensure JSON serialization/deserialization works with the new property

### Effect System Changes
- Replace `SpawnLooseEffect` with `SpawnJunkEffect`:
  - Add rotation properties (`RotX`, `RotY`, `RotZ`)
  - Update to work with the junk system
  - Modify `Execute()` to add junk to game state (when state management is added)

### Implementation Steps
1. Create `Junk` record in a new file or existing appropriate location
2. Add `JunkItems` list to `GameState`
3. Update `GameState.Clone()` and JSON serialization
4. Replace `SpawnLooseEffect` with `SpawnJunkEffect` in Effect.cs
5. Write comprehensive unit tests

## Benefits
- Centralized tracking of all world items
- Persistent item state across game saves/loads
- Foundation for item interaction systems
- Clean separation between state and rendering