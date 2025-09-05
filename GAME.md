# GAME.md

## high concept
- **genre:** 2.5d, first-person, fixed-yaw (n/e/s/w) psychological horror with light point-and-click.
- **premise:** you inherit your hoarder grandmother’s house. cleaning earns money + unlocks rooms. as sanity drops, the house repopulates with worse junk and reality bends.
- **run model:** roguelite. average player wins in ~7 runs. a clean run is ~20 min; full playthrough ~2 hours.

## inspirations (vibe + flow, not clones)
- inscryption, mouthwashing, milk inside a bag, old detective point-and-clicks.

## player goals
- clear rooms → sell/junk items → buy upgrades → unlock deeper rooms.
- avoid soft-locking yourself (doorways blocked) and sanity collapse.

## camera + movement
- **first-person** with 90° yaw snaps (a/d) and forward step (w).
- narrow fov (50–60). minimal bob. depth kept tight to stabilize pixel size.

## world structure
- **rooms are “fake 2d in 3d”**: simple 3d boxes (floor + 4 walls + ceiling). visuals are 2d textures/sprites.
- each room has 4 “faces”; the same 3d room is viewed from n/e/s/w. no per-face state desync.

## furnishing approach (no 3d modeling)
- **big furniture:** low-geo primitives (csg boxes/quads) skinned with **trim sheets** (tileable textures) + grime **decals**.
- **junk/props:** png sprites with alpha as **sprite3d** billboards. a handful per room are dynamic rigidbodies; most are static.
- **depth cheats:** edge cards + blob shadows; optional 2–3 layer parallax stacks for piles.

## core loop
1) enter room → assess mess + blocked doors.
2) pick items; **tetris-pack** into a grid bag.
3) exit room; sell/junk; buy upgrades; unlock new rooms.
4) sanity shifts: affect visuals, spawn rates, junk severity, audio, mild geometry lies late-game.

## systems (owned by the core engine)
- **inventory bag:** grid-packing of odd shapes (rotations, flips). no overlaps; solvable states. greedy assist + manual override.
- **economy:** pricing curves, condition modifiers, marketplace randomness (bounded).
- **sanity:** time pressure + triggers; tiers gate post-fx, spawn intensity, repopulation rates, rare events.
- **room decorate:** deterministic placement from authored spawn tables (piles, hotspots, furniture shells).
- **locks + scheduler:** time-aware gating (e.g., door anim 0.25s then transition).

## physics (kept minimal, but real)
- **dynamic junk:** 5–10 rigidbodies per active pile; simple colliders; high friction; sleep by default; wake on interaction.
- **door blocking:** an `area3d` in each doorway. if any **dynamic** body overlaps, the door is logically “blocked.”
- **ownership split:** physics owns transforms/velocities; core reasons over coarse flags (exists/dynamic/blocked).

## deterministic rng + procgen
- master seed per run; sub-seeds per room/system via `rng.split(tag)`.
- rooms/assets are authored; **layout + junk choice** are generated.
- record/replay: seed + input script ⇒ effect timeline is identical.

## tech stack
- **engine:** godot 4.x (desktop targets), c# scripts.
- **core engine:** separate pure c# library (no godot deps).
- **tests:** xunit for core; gut/gdunit (headless) for engine smoke/physics tolerance.
- **ci:** github actions runs `dotnet test` + `godot --headless` tests. optional nightly monte-carlo balance.

## architecture (state machine + orchestrator)
- contract: **inputs/observations → `Step(state, dt, rng)` → `state' + effects[]`**.
- **inputs:** player commands (`OpenDoor`, `PickItem`, …) + observations from engine (`DoorApertureOccupied`, `BodyCameToRest`, …).
- **effects:** tokens the renderer consumes (`PlayAnim`, `Sfx`, `LockInput(0.25)`, `SpawnLoose`, `Despawn`, `Transition`, …).
- **scheduler:** events queued at absolute `state.time` (`EnterRoom@+0.25s`, etc.). no `DateTime.Now`.

### frame loop (engine side)
1) physics tick.
2) gather observations + player inputs.
3) `state, effects = Step(state, inputs∪obs, dt, rng)`.
4) apply effects (spawn/anim/sfx/impulses/locks/ui). render.

### save/load
- **core:** serialize `state` (json).
- **physics snapshot:** for each dynamic rigidbody ⇒ `id, transform, linear/angular velocity, sleeping`.
- load: build scene → spawn bodies from core → freeze → apply snapshot → unfreeze → tick once so `area3d` overlaps refresh.

## content data (author-only json)
- rooms, doors (ids, links, apertures), spawn tables, items (shape mask, size, creepiness, base price), upgrades, events.
- stable ids everywhere; mapping from ids ↔ scene nodes.

## ui/ux notes
- interactables use hotspots; outline/jitter on hover.
- bag has crisp feedback: placement preview, snap, rotate/flip, rejection reasons.
- clear “blocked door” state + hint (“something’s in the way”).

## difficulty targets (first pass)
- door blocking should occur in ~30–50% of rooms if the player rushes.
- sanity banding: visible tier shifts every ~3–5 minutes under normal play.
- early-run win rate ~10–20%; endgame win rate ~60–70% with unlocks.

## vertical slice (what “done” looks like)
- one complete room, 8–12 items, working bag tetris, marketplace screen, one upgrade, sanity tier post-fx, door block/unblock, save/load, tests+ci green.

## open questions (to resolve before full prod)
- final ppm value and target internal resolution.
- upgrade list + meta-progression tree.
- exact sanity triggers (time vs actions vs discoveries).
- marketplace ui flavor (fictional fb vs generic pawn).
