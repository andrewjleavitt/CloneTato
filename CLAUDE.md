# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This repo contains two games sharing a common engine, all using C# and Raylib (Raylib-cs):

- **Drift** — Twin-stick desert survivor. Mouse/gamepad-aimed auto-fire weapons orbit the player. Survive 20 waves, shop between waves. Three playable heroes (Gunslinger, Blade Dancer, Drifter).
- **NukeDesert** — Nuclear Throne-style room-based roguelike (skeleton). Manual aim+fire, ammo management, dodge rolling, room transitions.
- **DesertEngine** — Shared library: sprite atlas, camera, entity base classes, collision helpers, UI helpers.

## Build & Run

```bash
dotnet build Drift.sln           # build all projects
dotnet run --project Drift.csproj           # run Drift
dotnet run --project NukeDesert/NukeDesert.csproj   # run NT clone
```

Requires .NET 9 SDK. Raylib-cs 6.1.1 is the only external dependency.

## Solution Structure

- `Drift.csproj` (root) — Twin-stick desert survivor, executable
- `DesertEngine/` — Shared class library, referenced by both games
- `NukeDesert/` — Nuclear Throne clone, executable, references DesertEngine

## Architecture

**Rendering**: Both games render to a `RenderTexture2D` at logical resolution, then scale to window with nearest-neighbor for pixel-perfect art. Drift uses 640x360 @ 2-3x. NukeDesert uses 480x400 @ 3x.

**State machine**: `GameStateManager` with enum-based screen transitions. Each screen has static `Update()` and `Draw()` methods receiving shared state + manager.

**Entity pooling**: Pre-allocated arrays with `Active` flags — no runtime allocation during gameplay.

**Systems**: Stateless static classes that operate on GameState each frame, called in sequence from `PlayingScreen.Update()`.

**Input**: SDL2 P/Invoke for gamepad (macOS Xbox controller support), Raylib as fallback. Twin-stick: LStick move, RStick aim, RT auto-fire.

### Drift-specific
- Weapons auto-fire when enemies are in range but **aim toward mouse/right stick** (auto-shoot yes, auto-aim no)
- Three weapon types: Auto (guns), Manual (grenades/mines), Melee (arc damage)
- Weapons visibly orbit the player, fan out toward aim direction
- Three heroes: Gunslinger (balanced), Blade Dancer (melee +40%/ranged -30%), Drifter (glass cannon + companion)
- Screen shake + enlarged damage numbers on critical hits
- STRANDED art pack (Penusbmic) for all character sprites, terrain, UI

### NukeDesert-specific (skeleton)
- Manual aim and fire (click to shoot), ammo/clip system with reload
- Dodge roll on Space (invincibility frames)
- Room-based progression: 5 areas × 3 sub-levels
- Mutations (upgrades) chosen between levels
- Two weapon slots, swap with Q

## Assets

`kenney_desert-shooter-pack_1/` — Kenney Desert Shooter Pack (CC0): sounds + legacy fallback sprites.
`assets/stranded/` — Penusbmic STRANDED art pack: hero sprites, enemy sprites, bosses, terrain, UI.
`gamecontrollerdb.txt` — SDL gamepad mappings for macOS controller support.

## CI/CD

- GitHub Actions CI builds on every push/PR to main
- Release pipeline on version tags (`v*`): publishes to GitHub Releases + itch.io for Linux, Windows, macOS ARM64, macOS x64

## Design Notes

- All collision is circle-based (position + radius)
- Camera follows player with lerp smoothing, clamped to bounds
- Enemy stats scale +12%/wave; boss every 5 waves at 4x
- 17 enemy types across 6 factions + 3 boss types
- NukeDesert: mob definitions per area with boss encounters
