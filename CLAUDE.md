# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This repo contains two games sharing a common engine, all using C# and Raylib (Raylib-cs):

- **CloneTato** — Brotato-style top-down arena survivor. Mouse-aimed auto-fire weapons orbit the player. Survive 20 waves, shop between waves.
- **NukeDesert** — Nuclear Throne-style room-based roguelike (skeleton). Manual aim+fire, ammo management, dodge rolling, room transitions.
- **DesertEngine** — Shared library: sprite atlas, camera, entity base classes, collision helpers, UI helpers.

## Build & Run

```bash
dotnet build CloneTato.sln    # build all three projects
dotnet run --project CloneTato.csproj        # run Brotato clone
dotnet run --project NukeDesert/NukeDesert.csproj   # run NT clone
```

Requires .NET 9 SDK. Raylib-cs 6.1.1 is the only external dependency.

## Solution Structure

- `CloneTato.csproj` (root) — Brotato clone, executable
- `DesertEngine/` — Shared class library, referenced by both games
- `NukeDesert/` — Nuclear Throne clone, executable, references DesertEngine

## Architecture

**Rendering**: Both games render to a `RenderTexture2D` at logical resolution, then scale to window with nearest-neighbor for pixel-perfect art. CloneTato uses 480x270 @ 3x. NukeDesert uses 480x400 @ 3x.

**State machine**: `GameStateManager` with enum-based screen transitions. Each screen has static `Update()` and `Draw()` methods receiving shared state + manager.

**Entity pooling**: Pre-allocated arrays with `Active` flags — no runtime allocation during gameplay.

**Systems**: Stateless static classes that operate on GameState each frame, called in sequence from `PlayingScreen.Update()`.

### CloneTato-specific
- Weapons auto-fire when enemies are in range but **aim toward mouse cursor** (auto-shoot yes, auto-aim no)
- Three weapon types: Auto (guns), Manual (grenades/mines), Melee (arc damage)
- Weapons visibly orbit the player, fan out toward aim direction
- Fixed-distance reticle (60px from player, not a mouse cursor replacement)
- Screen shake + enlarged damage numbers on critical hits

### NukeDesert-specific (skeleton)
- Manual aim and fire (click to shoot), ammo/clip system with reload
- Dodge roll on Space (invincibility frames)
- Room-based progression: 5 areas × 3 sub-levels
- Mutations (upgrades) chosen between levels
- Two weapon slots, swap with Q

## Assets

`kenney_desert-shooter-pack_1/` — Kenney Desert Shooter Pack (CC0):
- `PNG/*/Tilemap/tilemap_packed.png` — Packed tilesheets (NO spacing)
- Players: 4x4 grid, 24px | Enemies: 4x4, 24px | Weapons: 10x4, 24px
- Tiles: 18x13, 16px | Interface: 18x11, 16px
- `Sounds/` — 40 OGG files with variants (coin-a..d, shoot-a..h, etc.)

## Design Notes

- All collision is circle-based (position + radius)
- Camera follows player with lerp smoothing, clamped to bounds
- CloneTato: enemy stats scale +12%/wave; boss every 5 waves at 4x
- NukeDesert: mob definitions per area with boss encounters
