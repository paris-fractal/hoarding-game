# Effect System Implementation Plan

## Goal
Implement an effect system that allows activities to create visual/audio effects that are processed by the renderer. Start with RotatePlayerEffect for smooth player rotation animations using Godot's Tween system.

## Design

### Effect System Architecture
- Effects are created by Activities during their lifecycle (OnStart, OnEnd)
- Effects are collected in GameEngine.Step() and returned to the renderer
- Effects contain all data needed for the renderer to execute them
- Effects are fire-and-forget - no state tracking in core game logic

### RotatePlayerEffect Implementation
- Inherits from GameEffect base class
- Contains:
  - Target rotation angle
  - Duration (matches activity duration)
  - Start time (for synchronization)
  - Node ID/reference for the player object
- Executed by renderer using Godot Tween system

### Activity Integration
- RotatePlayerActivity creates RotatePlayerEffect in OnStart()
- Effect duration matches activity duration (0.5s)
- Visual rotation starts immediately when activity starts
- State update happens when activity ends

### Effect Processing Flow
1. Activity.OnStart() creates RotatePlayerEffect
2. GameEngine collects effect in effects list
3. GameEngine.Step() returns effects to renderer
4. Renderer executes effect using Godot Tween
5. Activity.OnEnd() updates game state when visual completes

## Benefits
- Clean separation between game logic and visual presentation
- Synchronizes visual animations with state changes
- Extensible for other effects (movement, sound, particles, etc.)
- Leverages Godot's built-in animation system