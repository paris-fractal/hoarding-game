# Overview

This is a point-and-click horror game written in C# and Godot. It is split into two pieces:
 - `/scripts/core` contains the state machine for the game. As an AI agent, you will do almost
 all of your work in this repo.
 - everything else is the assets, rendering logic, etc. of the game.

Make sure everything we do conforms to the goals in `GAME.md`.

# Style

Use clean, simple code, preferring a few strong, simple abstractions over lots of smaller ones.
Use lowerCamelCase for private variables, not the understore convention.

# Workflow

1. Read the plan in the `plans` folder that the user is asking you to implement.
2. Write several unit tests that will confirm this is implemented correctly.
3. Follow the implementation plan step by step.
4. Run the unit tests with `dotnet test`, making changes to the code until the tests pass.
5. You're done!

# Concepts

## Overview

The game is divided into two systems: the game engine and the godot rendering engine.

The game engine is composed of an [Orchestrator](./scripts/Orchestrator.cs) which keeps
track of a giant god-object called `GameState`. It runs the [GameEngine](./scripts/core/GameEngine.cs) once per frame,
computing the next value of the `GameState`. Almost all the logic for it is in `/core`.
Almost nothing in `/core` should know that Godot even exists.

The Godot rendering piece is dispersed elsewhere around the codebase. It should never mutate
`GameState` directly, instead dispatching inputs and observations about the state of the physics engine
to the `Orchestrator`. 

## Activity

An [Activity](./scripts/core/Activity.cs) is a piece of logic that takes place over game time.
An activity should never directly touch Godot. Instead, it models how state is expected
to change over time. To change Godot in response to time passing, spawn an Effect at the
correct moment and implement that logic in there.

## Effect

A [GameEffect](./scripts/core/Effect.cs) is an instruction to Godot to do something not related to the game state directly.
For instance, sound effects, particles, and tweens, which aren't modelled by the game state.
They are allowed to set up and execute code that affects the scene directly.