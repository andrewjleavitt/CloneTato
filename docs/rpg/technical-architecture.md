# Dead Meridian — Technical Architecture

*Plan for building on DesertEngine + STRANDED assets. Not implementation yet — just the map.*

## Project Structure

```
DeadMeridian/
├── DeadMeridian.csproj          # New executable, references DesertEngine
├── src/
│   ├── Core/
│   │   ├── RPGGameState.cs       # Central state: player, NPCs, quests, area
│   │   ├── RPGStateManager.cs    # Screen transitions (title, playing, dialog, inventory, pause)
│   │   └── SaveSystem.cs         # JSON serialize/deserialize game state
│   ├── World/
│   │   ├── Area.cs               # Area definition: tilemap, NPCs, enemies, exits
│   │   ├── AreaLoader.cs         # Load area data from definition files
│   │   ├── AreaTransition.cs     # Door/exit zones that connect areas
│   │   └── WorldMap.cs           # Graph of connected areas
│   ├── Entities/
│   │   ├── Player.cs             # Player state, input, animation
│   │   ├── Companion.cs          # AI companion: follow, gather, fight, dialog
│   │   ├── NPC.cs                # Non-hostile characters with dialog triggers
│   │   ├── Enemy.cs              # Hostile entities (reuse + extend from engine)
│   │   └── Interactable.cs       # Chests, workbenches, signs, doors
│   ├── Combat/
│   │   ├── TurnManager.cs        # Turn order, AP tracking, turn phases
│   │   ├── ActionSystem.cs       # Move, shoot, melee, use item — all cost AP
│   │   ├── CoverSystem.cs        # Obstacle-based defense bonuses
│   │   ├── CompanionAI.cs        # Droid's autonomous combat decisions
│   │   ├── WeaponSystem.cs       # Gun + blade stats, ammo, range, accuracy
│   │   └── DamageSystem.cs       # Damage calc, armor, death
│   ├── Dialog/
│   │   ├── DialogSystem.cs       # Typewriter text, choice rendering
│   │   ├── DialogData.cs         # Conversation trees (node-based)
│   │   └── DialogLoader.cs       # Load dialog from data files
│   ├── Quest/
│   │   ├── QuestSystem.cs        # Track active/completed quests, check conditions
│   │   ├── QuestData.cs          # Quest definitions
│   │   └── FactionReputation.cs  # Reputation tracking per faction
│   ├── Inventory/
│   │   ├── Inventory.cs          # Items, weapons, key items
│   │   └── ItemData.cs           # Item definitions
│   ├── UI/
│   │   ├── DialogBox.cs          # 9-patch panel + typewriter text
│   │   ├── InventoryScreen.cs    # Item grid display
│   │   ├── QuestLog.cs           # Active/completed quest list
│   │   ├── HUD.cs                # Health, ammo, interact prompts
│   │   └── ShopScreen.cs         # Merchant buy/sell interface
│   └── Screens/
│       ├── TitleScreen.cs
│       ├── PlayingScreen.cs      # Main gameplay loop
│       ├── InventoryScreen.cs
│       └── GameOverScreen.cs
├── data/
│   ├── areas/                    # Area definitions (JSON or custom format)
│   ├── dialog/                   # Dialog trees (JSON)
│   ├── quests/                   # Quest definitions (JSON)
│   └── items/                    # Item/weapon definitions (JSON)
└── assets/ → symlink or copy from CloneTato/assets/stranded/
```

## What to Reuse from Existing Code

### From DesertEngine (use directly)
| System | Status | Notes |
|--------|--------|-------|
| SpriteAtlas | Ready | Works for all STRANDED spritesheets |
| GameCamera | Ready | Lerp follow + bounds clamping |
| CollisionHelper | Ready | Circle overlap for interaction + combat |
| UIHelper | Extend | Add dialog box, 9-patch support |
| DamageNumber | Ready | Float-up damage text |
| Entity base | Ready | Position, velocity, rotation, sprite |
| AssetManager | Extend | Add STRANDED-native asset loading paths |

### From NukeDesert (port/adapt)
| System | Status | Notes |
|--------|--------|-------|
| Room transitions | Adapt | Change from random to authored area connections |
| Ammo tracking | Adapt | Scarcity-based ammo as turn-based resource |

*Note: Dodge roll, real-time aim, and real-time weapon swap are NOT relevant for turn-based. These stay in Drift/NukeDesert.*

### From Drift (selective)
| System | Status | Notes |
|--------|--------|-------|
| STRANDED sprite loading | Port | Animation system, multi-atlas management |
| Enemy definitions | Adapt | Stat blocks → turn-based stat blocks |
| Terrain/scatter generation | Adapt | Per-area decoration spawning |
| Shop UI | Adapt | Merchant interface base |

## New Systems to Build

### Priority 1 — Playable foundation
1. **Area system**: Define areas as data (tilemap + entity placements + exits). Load and transition between them.
2. **NPC interaction**: Proximity check → interact prompt → trigger dialog/shop/quest.
3. **Dialog system**: 9-patch text box, typewriter effect, branching choices. Data-driven (JSON dialog trees).
4. **Turn-based combat**: The big new system. Turn manager, AP pool, move/shoot/melee/item actions, enemy AI turns, companion autonomous turns.

### Priority 2 — Game loop
5. **Quest system**: Flag-based state tracking. Quests define conditions and outcomes. NPCs check quest state for dialog branching.
6. **Inventory**: Simple item list. Weapons, key items, consumables. Grid UI.
7. **Save/load**: Serialize player position, inventory, quest flags, faction rep, current area to JSON.

### Priority 3 — Depth
8. **Companion AI (combat)**: Droid makes its own decisions in combat based on personality, situation, and relationship with player.
9. **Companion AI (exploration)**: Follow player, contextual banter, react to world, gather animation on points of interest.
10. **Faction reputation**: Numeric rep per faction. Gates dialog options, quest availability, area access.
11. **Cover system**: Obstacles provide defense bonuses during tactical combat. Destructible cover for depth.
12. **Avoidable encounters**: Some fights can be talked out of or snuck past. Dialog checks, stealth AP costs.

## Data-Driven Design

The game should lean heavily on data files rather than hardcoded content. This makes authoring faster and keeps code separate from content.

### Dialog Format (JSON)
```json
{
  "id": "merchant_traveler_intro",
  "speaker": "Kael",
  "lines": [
    {
      "text": "You're new. I can tell by the way you're squinting.",
      "next": "merchant_traveler_choice1"
    }
  ]
}

{
  "id": "merchant_traveler_choice1",
  "speaker": null,
  "choices": [
    { "text": "I'm looking for someone.", "next": "merchant_traveler_someone", "rep": { "settlers": 1 } },
    { "text": "Just passing through.", "next": "merchant_traveler_passing" },
    { "text": "[Say nothing]", "next": "merchant_traveler_silent", "rep": { "tribes": 1 } }
  ]
}
```

### Area Format (JSON)
```json
{
  "id": "dustport",
  "displayName": "Dustport",
  "biome": "town",
  "bounds": { "width": 800, "height": 600 },
  "tilemap": "dustport_tiles.json",
  "npcs": [
    { "id": "kael", "type": "merchant_traveler", "position": [400, 300], "dialog": "merchant_traveler_intro" }
  ],
  "enemies": [],
  "exits": [
    { "to": "barrens_north", "position": [780, 300], "size": [20, 100], "direction": "east" }
  ]
}
```

### Quest Format (JSON)
```json
{
  "id": "missing_surveyor",
  "title": "The Missing Surveyor",
  "description": "Kael mentioned a surveyor who went into The Barrens three days ago and hasn't come back.",
  "stages": [
    { "id": "find", "description": "Search The Barrens for the surveyor", "condition": "flag:found_surveyor" },
    { "id": "choice", "description": "Decide what to do", "condition": "flag:surveyor_resolved" }
  ],
  "rewards": { "credits": 150, "rep": { "settlers": 2 } }
}
```

## Rendering

- Same approach: render to RenderTexture2D at logical resolution, scale to window
- Resolution: 480x270 or 480x360 (TBD — depends on how much vertical space dialog boxes need)
- Nearest-neighbor scaling for pixel-perfect art
- Layer order: floor tiles → ground scatter → shadows → entities (y-sorted) → projectiles → UI overlay

## Technical Risks

| Risk | Mitigation |
|------|------------|
| Dialog system complexity | Start with linear dialog, add branching after it works |
| Area transitions feeling janky | Simple fade-to-black first, polish later |
| Companion AI being annoying | Keep it simple: follow + stay close + attack nearest. Personality comes from dialog, not AI |
| Scope creep | The game is 5 areas. If an idea doesn't serve those 5 areas, it waits |
| Save system bugs | Serialize everything to one JSON blob. Test early, test often |
