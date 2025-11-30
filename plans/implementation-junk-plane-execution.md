# Plan: Junk Plane Debug Spawn & Constraining

1) Core input/engine
- Extend `SpawnJunkInput` to carry spawn position fields (and keep timestamp for determinism).
- Update `HandleSpawnJunk` in `GameEngine` to use the provided position when adding a `JunkItem`.

2) Player debug interaction
- In `Player._Input`, detect J-held + left click and raycast from camera up to ~5m against the JunkPlane area.
- When the hit collider is a JunkPlane, enqueue `SpawnJunkInput` with the hit position (and timestamp from state).
- Keep normal interactions untouched when J isn’t held.

3) Junk behavior on plane
- Convert `Junk` to a rigidbody-driven node that locates its parent `JunkPlane`.
- On spawn, align yaw to the plane’s forward axis and apply a small horizontal impulse.
- Use `_IntegrateForces` to clamp motion within the plane’s bounds (depth locked to plane, limited extents from box shape) and zero out violating velocity.

4) Tests
- Add/adjust GdUnit/core tests: ensure `SpawnJunkInput` position is propagated into `GameState`; basic plane association behavior for `Junk` if feasible.
- Run `dotnet test` to validate.
