# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

CloneTato is a Brotato-style top-down arena survivor game built with C# and Raylib (Raylib-cs). The player selects a character, survives 20 waves of enemies that spawn from arena edges, collects XP to level up, and shops for weapons/items between waves. Weapons auto-fire at the nearest enemy.

## Build & Run

```bash
dotnet restore         # first time only
dotnet build           # compile
dotnet run             # build and launch
```

Requires .NET 8 SDK. The Raylib-cs NuGet package (v6.1.1) is the only dependency.

## Architecture

**Game loop**: `Program.cs` renders to a `RenderTexture2D` at 480x270 (logical resolution), then scales 3x to 1440x810 window with nearest-neighbor filtering for pixel-perfect art.

**State machine**: `GameStateManager` drives screen transitions: MainMenu → CharacterSelect → Playing ↔ Shop/LevelUp → Victory/GameOver. Each screen has `Update(dt, state, manager)` and `Draw(state, manager)` methods.

**Central state**: `GameState` holds all entity pools, equipped weapons, items, wave state, gold, XP, and stats. All systems and screens receive this shared state object.

**Entity pooling**: Enemies (300), Projectiles (500), XPOrbs (400), DamageNumbers (100) are pre-allocated. Entities have an `Active` flag — no runtime allocation during gameplay.

**Systems**: Stateless static classes that operate on GameState each frame: `PlayerSystem`, `EnemySystem`, `WeaponSystem`, `WaveSystem`, `CollisionSystem`. Called in sequence from `PlayingScreen.Update()`.

## Key Directories

- `src/Core/` — GameState, GameStateManager, Camera, CollisionSystem
- `src/Entities/` — Player, Enemy, Projectile, XPOrb, DamageNumber
- `src/Data/` — Stats struct, definition classes (CharacterDef, WeaponDef, EnemyDef, ItemDef, WaveConfig) with static databases
- `src/Systems/` — Per-frame logic (PlayerSystem, EnemySystem, WeaponSystem, WaveSystem)
- `src/Screens/` — One class per game screen (MainMenu, CharacterSelect, Playing, Shop, LevelUp, GameOver, Victory)
- `src/Assets/` — AssetManager (loads all textures/sounds), SpriteAtlas (cuts sprites from packed tilesheets)
- `src/UI/` — UIRenderer (HUD, buttons, text helpers)

## Assets

`kenney_desert-shooter-pack_1/` — Kenney Desert Shooter Pack (CC0 license):
- `PNG/*/Tilemap/tilemap_packed.png` — Packed tilesheets (NO spacing between tiles)
- Players: 4x4 grid, 24px tiles (4 characters × 4 frames)
- Enemies: 4x4 grid, 24px tiles (4 enemy types × 4 frames)
- Weapons: 10x4 grid, 24px tiles (40 weapon sprites)
- Tiles: 18x13 grid, 16px tiles (234 terrain/building tiles)
- Interface: 18x11 grid, 16px tiles (198 UI elements including two font rows)
- `Sounds/` — 40 OGG files with variants (coin-a..d, shoot-a..h, hurt-a..e, etc.)

## Design Notes

- All collision is circle-based (position + radius)
- Camera follows player with lerp smoothing, clamped to arena bounds
- Enemy stats scale by +12% per wave; boss enemies (every 5 waves) get 4x scale
- Weapons auto-target nearest enemy within range and fire on cooldown
- The shop offers weapons and stat items, with tier unlocks based on wave progress
