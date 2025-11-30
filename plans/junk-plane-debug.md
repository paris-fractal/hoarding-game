Title: Click‑to‑Spawn Junk on JunkPlane + Junk Plane Constraining

Goal

Add a debug/authoring interaction: while holding J, clicking in the world should raycast against a JunkPlane Area3D and spawn a junk item at the hit position, with a small random impulse. Junk instances should treat their parent JunkPlane as their “home plane”: they always face the plane’s forward axis and are constrained to stay on that plane’s bounds.

Scope

Input side: Player.cs should generate a SpawnJunkInput at the click location on a JunkPlane.
Core side: SpawnJunkInput should carry spawn position (and optionally impulse seed) into GameEngine, which will add a JunkItem to GameState.
Godot side: Junk.cs should:
Assume its parent is a JunkPlane Area3D.
Orient itself to the plane’s forward axis.
Constrain its motion to within the plane’s bounds using _IntegrateForces.
File‑by‑file spec

scripts/Player.cs

Add “debug spawn junk” interaction:
While J key is down and the player left‑clicks (mouse button 1), do a raycast from the active camera through the mouse position, with max distance ≈ 5 meters.
Use GetViewport().GetMousePosition() + Camera3D.ProjectRayOrigin/ProjectRayNormal or ProjectRayOrigin/ProjectPosition to construct the ray.
Use GetWorld3D().DirectSpaceState.IntersectRay(...) (or equivalent Godot 4 API) with:
from = ray origin
to = origin + direction * 5.0f
Collision mask configured to hit Area3D “junk plane” volumes (either via a dedicated physics layer or a group check after hit).
If the raycast hits and the collider is a JunkPlane Area3D (e.g., hit["collider"] is Area3D and ((Node)hit["collider"]).Name == "JunkPlane" or in group "JunkPlane"), then:
Read the hit position (Vector3).
Compute a small random impulse direction:
Mostly in the plane (e.g., X/Z), magnitude configurable (e.g., 0.5–1.5).
No vertical impulse or a very small upward component.
Enqueue a SpawnJunkInput via Orchestrator.Enqueue(...), carrying the hit position (and an impulse/seed if we want it in core).
Implementation details:
Keep this behind a clear condition so normal clicks aren’t affected.
Make sure the input reading respects existing input handling (don’t block other interactions when J not held).
scripts/core/Input.cs

Extend SpawnJunkInput to carry the data we need:
Add properties:
public float PosX, PosY, PosZ for spawn position in world space.
Optional: public float ImpulseX, ImpulseY, ImpulseZ or public float ImpulseStrength and a seeded random direction.
Ensure these fields are simple POCO properties so they serialize fine if needed.
scripts/core/GameEngine.cs

Update HandleSpawnJunk(GameState state, SpawnJunkInput input) to use the new fields:
Replace the current hardcoded JunkItem position with the input’s PosX/PosY/PosZ.
Keep type as "junk_can" for now.
Rotation can be defaulted to 0 for now; actual “face forward” is handled on the Godot side by Junk.
If we decide to keep impulse purely on the Godot side:
You may ignore the impulse fields in core and let Junk apply its own random impulse on spawn.
If we want the impulse to be deterministic from core:
Store an impulse vector or a random seed into JunkItem or produce a GameEffect (e.g., ApplyImpulseEffect) targeted to the spawned junk.
This might be better as a follow‑up PR; for this spec, it’s enough to define the input shape.
scripts/Junk.cs

Change Junk to be physics‑driven:
Either:
Change base class from Node3D to RigidBody3D, or
Wrap a RigidBody3D child inside the Junk scene and move the logic there (preferred if existing scenes already assume Junk root type).
Ensure the body has a collider in its .tscn (box or capsule approximating the sprite).
Add a reference to the parent JunkPlane:
On _Ready, walk up the parent chain to find an Area3D whose name or group marks it as JunkPlane.
Cache its Node3D/Area3D reference; if not found, log a warning and bail from plane‑specific logic.
Orientation behavior:
Each frame (_PhysicsProcess or inside _IntegrateForces):
Determine the plane’s forward axis; e.g., use junkPlane.GlobalTransform.Basis.Z or -Z depending on convention.
Align the sprite to face that axis:
Compute the yaw angle that makes the junk visually face along that vector.
Option: snap yaw to nearest 90° for now; in future you can add 45° corner cases.
Set rotation (only Y) accordingly; keep X/Z rotation locked for stability.
Plane clamping via _IntegrateForces:
Implement _IntegrateForces(PhysicsDirectBodyState3D state) on the RigidBody3D:
Transform state.Transform.Origin into the JunkPlane’s local space (plane.ToLocal(origin)).
Clamp the coordinate that represents “depth” to 0 (or within a shallow range) so the body stays on the plane; optionally clamp X/Y to the plane’s extents if you have a BoxShape3D on the plane to read Size from.
If you clamp along a coordinate, also zero or reflect the corresponding component in state.LinearVelocity so the solver isn’t constantly pushing against the clamp.
Transform the clamped local position back to global and assign to state.Transform.
Do not manually resolve junk–junk overlaps; leave that to the physics solver inside the plane bounds.
Impulse on spawn:
On _Ready (or first _PhysicsProcess), apply a small random impulse to the body to “settle” it:
Random horizontal direction in the plane, magnitude matching what Player requested.
If we passed an impulse vector via JunkItem/SpawnJunkInput, use that instead of generating a new random value.
JunkPlane representation (light spec; likely a separate PR but needed conceptually)

For this PR, we assume:
JunkPlane is an Area3D node placed in the scene, named "JunkPlane" (or in group "JunkPlane").
It has a CollisionShape3D with a BoxShape3D that defines the playable bounds for junk.
The actual JunkPlane script (if any) can be minimal; the key requirement is: we can find this node from a Junk and from a raycast, and we can read its transform and box extents.
Testing

Add or extend GdUnit tests to cover:
SpawnJunkInput:
Construct an engine + state, send a SpawnJunkInput with specific position, assert that GameState.JunkItems contains an item at that position after Step.
Integration test in JunkSystemTest:
Spawn junk via the new input path (by simulating Orchestrator.Enqueue(new SpawnJunkInput { PosX=..., ... })) and simulate frames:
Verify a new junk node appears under the appropriate room node.
Verify its parent is a JunkPlane.
If feasible, a simple scene test:
Place a JunkPlane and a Junk instance.
Move the Junk out of bounds via initial transform, simulate physics steps, and assert it gets clamped back to the plane.
Manual sanity checks (for you in the editor):

Run the game, hold J, click on different parts of the visible JunkPlane:
Junk appears at the clicked location.
Each piece faces the plane’s forward direction.
Junk does not drift off the plane over time.
Multiple spawned objects collide and settle without exploding.
Non‑goals / follow‑ups

Corner behavior (45° orientations, blending between walls) is out of scope for this PR; for now, each junk item simply inherits its parent JunkPlane’s forward orientation.
Persisting per‑junk impulse/velocity in GameState for deterministic replay can be added later.
Using groups/layers for multiple JunkPlanes per room (N/E/S/W) is also a follow‑up; this PR just assumes the basic Area3D shape and transform exist.