# Activity System Implementation Plan

## Goal
Implement an activity system that allows the game to track pending operations that have a start time, end time, and actions that occur at both points. This will decouple visual animations from state changes.

## Design

### Activity Base Class
- `Activity` abstract base class with:
  - `start` time (float)
  - `end` time (float) 
  - `OnStart()` abstract method - called when activity begins
  - `OnEnd()` abstract method - called when activity completes

### GameState Changes
- Add `List<Activity> activities` to track pending activities

### GameEngine Changes
- Process activities each frame:
  - Start activities that have reached their start time
  - Complete activities that have reached their end time
  - Remove completed activities

### RotatePlayerActivity Implementation
- Inherits from Activity
- `OnStart()` - triggers visual rotation animation
- `OnEnd()` - updates player rotation in GameState

### Input Handler Changes
- `RotatePlayerInput` creates a `RotatePlayerActivity` instead of immediately updating state
- Activity duration should match the visual rotation time

## Benefits
- Clean separation between visual effects and state changes
- Extensible for other timed operations (walking, interactions, etc.)
- Prevents state/visual desync issues