# Junk Realization System Implementation Plan

## Goal
Implement a system in JunkSystem.cs that synchronizes the GameState.JunkItems with actual 3D objects in the world. The system should:
- Instantiate junk objects for items in GameState that don't exist in the world
- Remove junk objects that no longer exist in GameState
- Track which junk items have been realized to avoid duplicates
- Load the correct PackedScene based on junk type

## Current State Analysis
- `GameState` has a `List<Junk> JunkItems` property to track junk
- `Junk` record contains Id, type, position (PosX/Y/Z), and rotation (RotX/Y/Z)
- JunkSystem.cs exists but currently spawns junk_can every frame (problematic)
- junk_can.tscn exists at `res://objects/junk/junk_can.tscn`
- Junk.cs script exists for individual junk objects

## Design

### JunkSystem Tracking
- Add `Dictionary<string, Node3D> realizedJunkObjects` to track junk ID -> scene node
- Add `GameState currentGameState` reference to access junk data
- Add method to get GameState from the game engine

### Core Logic
The `_Process(double dt)` method should:
1. Get current GameState from the game engine
2. Handle junk removal: Remove objects that exist in world but not in GameState
3. Handle junk creation: Create objects for GameState junk not yet realized
4. Update positions/rotations if needed (future enhancement)

### Junk Removal Logic
- Compare `realizedJunkObjects.Keys` with current `GameState.JunkItems.Select(j => j.Id)`
- For junk IDs that exist in realized objects but not in GameState:
  - Remove the node from scene with `QueueFree()`
  - Remove from `realizedJunkObjects` dictionary

### Junk Creation Logic
- For each junk in GameState not in `realizedJunkObjects`:
  - Load the appropriate PackedScene based on junk.type
  - Instantiate the scene
  - Set position and rotation based on junk data
  - Store junk ID in the object for identification
  - Add to scene and track in `realizedJunkObjects`

### Scene Loading
- Create method `GetJunkScene(string junkType)` that maps junk type to scene path
- Initially support "junk_can" -> "res://objects/junk/junk_can.tscn"
- Extensible for future junk types

### Object Identification
- Add property to Junk.cs script to store the GameState ID
- This enables bidirectional lookup between GameState and world objects

## Implementation Steps
1. Update Junk.cs to include GameState ID property
2. Update JunkSystem.cs with tracking dictionary and GameState access
3. Implement junk removal logic in _Process method
4. Implement junk creation logic in _Process method
5. Create scene loading method with type mapping
6. Write unit tests for both creation and removal logic
7. Test with dotnet test

## Testing Strategy
- Test empty GameState (no junk should be spawned)
- Test single junk item realization
- Test multiple junk items creation
- Test junk removal when item removed from GameState
- Test mixed creation and removal in same frame
- Test that already-realized junk is not duplicated
- Test unknown junk types are handled gracefully
- Mock GameState access for unit testing

## Future Considerations
- Position/rotation updates for moved junk items
- Performance optimization for large numbers of junk items
- Support for additional junk types beyond junk_can
- Integration with physics system for dynamic junk