# Overview

This is a point-and-click horror game written in C# and Godot. It is split into two pieces:
 - `/scripts/core` contains the state machine for the game. As an AI agent, you will do almost
 all of your work in this repo.
 - everything else is the assets, rendering logic, etc. of the game.

See `GAME.md` for more details.

# Style

Use clean, simple code, preferring a few strong, simple abstractions over lots of smaller ones.
Use lowerCamelCase for private variables, not the understore convention.

# Workflow

1. Make an implementation plan and write it into `/plans`.
2. Write several unit tests that will confirm this is implemented correctly.
3. Follow the implementation plan step by step.
4. Run the unit tests (TODO: add a way to run the unit tests), making changes to the code until the tests pass.
5. You're done!