# Sci-Fi Western Action RPG — STRANDED Asset Inventory

*What we have, what it becomes, and what's missing.*

## Characters

### Player Characters

| Asset | Specs | RPG Role | Notes |
|-------|-------|----------|-------|
| Hero (Gun variant) | 64x65px, directional idle/run/roll/death | Primary player — ranged loadout | Full 4-direction animation set |
| Hero (Blade variant) | 64x65px, directional idle/run/roll/slash/death | Primary player — melee loadout | Includes slash attack animation |
| Starter Hero | 20x32px, idle/run/roll/death | Could be a younger version or flashback sprite | Simpler, smaller — good for prologue? |

**Gap**: No animation for "interact" or "talk" — player will just face NPC and stop.

### Companion

| Asset | Specs | RPG Role | Notes |
|-------|-------|----------|-------|
| Companion | 20x32px, idle/move/gather/attack | Player's companion character | Gather animation = perfect for "examining things" |

**Casting idea**: The companion is a small, resourceful creature or droid. Think Grogu meets R2-D2. The gather animation reads as "investigating" or "tinkering." The attack animation gives them combat utility without making them a full party member.

### NPCs (Merchants → Named Characters)

| Asset | Specs | RPG Role | Proposed Character |
|-------|-------|----------|--------------------|
| Traveler Merchant | With/without shadow | General goods trader, quest giver | **Kael** — first NPC you meet. Knows everyone's business. |
| Skull Merchant | With/without shadow | Black market / forbidden tech | **Morrow** — deals in pre-corp tech. Knows about the ruins. Dangerous. |
| Fruit Merchant | With/without shadow | Food/consumables, information front | **Senna** — cheerful exterior, runs an info network. |
| Bread Merchant | With/without shadow | Supplies, faction-connected | **Dak** — ex-corp supply officer. Still has connections. |

**Gap**: Only 4 merchant sprites. Need generic "townsperson" NPCs for background population. Could reuse enemy sprites (tribe members) as non-hostile NPCs with palette context.

### Enemies → RPG Encounters

#### Fauna (The Barrens — native wildlife)

| Asset | Specs | Encounter Type |
|-------|-------|----------------|
| Small Bug | 20x28px, idle/walk/death | Trash mob. Swarms near nests. |
| Medium Insect | 34x37px, idle/walk/death | Standard encounter. Territorial. |
| Big Bug | 72x44px, idle/walk/death | Mini-boss fauna. Blocks paths. |
| Spiny Beetle | 88x37px, idle/walk/death | Dangerous predator. Avoidable if smart. |

#### Tribe (The Tribes faction — not always hostile)

| Asset | Specs | Encounter Type |
|-------|-------|----------------|
| Tribe Hunter | 34x37px, idle/walk/death | Patrol units. May talk before fighting. |
| Tribe Warrior | 62x69px, idle/walk/death | Elite fighters. Reputation-gated hostility. |
| Tribe Tamed Beast | 76x67px, directional | Mounted or allied creature. Boss-tier if hostile. |

**Story note**: Tribe enemies should only be hostile based on reputation. If the player has good tribal rep, these become neutral or allied NPCs. The same sprites serve double duty.

#### Robots (Remnant Corp tech)

| Asset | Specs | Encounter Type |
|-------|-------|----------------|
| Rusty Robot | 20x29px | Derelict. Some still active in ruins. |
| Guard Robot | 26x34px | Corp security. Active near corp installations. |
| Circle Bot | 29x35px | Surveillance/patrol. Alerts other enemies. |
| Delivery Bot | 23x21px | Non-hostile? Could be a quest item carrier. |

**Story note**: Robots in the ruins raise the question: who's still maintaining them? Good mystery fodder.

#### Minions (Scavenger faction)

| Asset | Specs | Encounter Type |
|-------|-------|----------------|
| Hooded Minion | 33x36px | Scavenger foot soldier. Common in Barrens. |
| Bomb Minion | 13x15px | Suicide bomber type. Scavenger camps. |
| Ranged Minion | 25x15px | Scavenger sniper. Ruins and ambushes. |

#### Starter Pack Enemies

| Asset | Specs | Encounter Type |
|-------|-------|----------------|
| Starter Archer | 32x32px | Generic ranged enemy. Settlement defense? |
| Starter Guard | 32x32px | Generic melee. Town guards or corp troops. |
| Starter Warrior | 32x32px | Generic heavy. Boss bodyguards. |

### Bosses → Major Encounters

| Asset | Specs | RPG Role | When |
|-------|-------|----------|------|
| Dust Warrior | 67x45px, idle/run/attack/death + specials | Scavenger warlord or tribal champion | Mid-game confrontation |
| Tarnished Widow | 188x90px, idle/walk/attack/death | Ancient ruin guardian. Insectoid horror. | Overgrown Vaults boss |
| Blowfish | 94x47px, burrow/attack/death | Desert apex predator | Barrens boss, optional? |
| Electrocutioner | 157x107px, teleport/attack/range attack | Remnant Corp enforcer or ruin AI | Late-game boss |
| Dust Jumper | 42x91px | Ambush predator | Mini-boss, Barrens/Murk |
| Relic Guardians 1-3 | Complex modular sprites | Ancient protectors of the secret | Final area bosses |

---

## Environments

### Biome → Area Mapping

| Biome Asset | Game Area | Mood |
|-------------|-----------|------|
| Town Builder | **Dustport** — hub town | Warm, busy, safe (mostly) |
| Blood Desert | **The Barrens** — open frontier | Harsh, exposed, dangerous |
| Temple (Overgrown) | **Overgrown Vaults** — ancient ruins | Eerie, verdant, mysterious |
| Farm | **The Greenhollow** — settler farmland | Peaceful, pastoral, vulnerable |
| Swamps | **The Murk** — toxic wetlands | Oppressive, hidden, wrong |

### Terrain Details

| Asset | Use |
|-------|-----|
| Blood Desert tileset (16x16) | Floor tiles for Barrens and general outdoor areas |
| Ground scatter (grass, pebbles, gravel) | Ambient decoration, all areas |
| Large scatter (rocks, swords, poles, hands) | Environmental storytelling props |
| Trees (5 variants) | Collidable obstacles, Greenhollow and Vaults |
| Big Rock | Collidable obstacle, Barrens |
| Skulls (plain, grassy) | Collidable, environmental storytelling |
| Statue | Collidable, interactable? Lore object? |
| Relics (3 sets) | Puzzle/interaction objects in Vaults |

### Terrain Tilesets Available but Unused

| Asset | Potential Use |
|-------|---------------|
| `terrain/farm/` | Greenhollow floor and structures |
| `terrain/temple/` | Overgrown Vaults interiors (has Tiled format!) |
| `terrain/swamps/` | The Murk floor and edges |
| `terrain/relics/` | Deep ruin decorations |
| `terrain/town_builder/` | Dustport buildings and streets |
| `terrain/trees/` | Additional tree variety |

---

## Weapons

| Asset | Type | RPG Fit |
|-------|------|---------|
| Fireball | Ranged VFX | Energy weapon projectile |
| Missile | Ranged VFX | Heavy weapon / launcher |
| Lightning Bolt | Ranged VFX | Tech weapon (relic-powered?) |
| Chain-able Lightning | Ranged VFX | Upgraded tech weapon |
| Ice Spikes | Ranged VFX | Cryo weapon (Murk-themed?) |
| Techno Blade | Melee VFX | High-tier melee weapon |
| Axe Boomerang | Thrown VFX | Thrown weapon option |
| Flame Bottle | Thrown VFX | Molotov/area denial |
| Poison Bottle | Thrown VFX | DoT weapon (Murk-sourced?) |
| AOE Rune Spell | Area VFX | Relic-powered ability |

**Weapon count**: 10 VFX types. With gun/blade base animations, this gives us a solid 12+ weapon feel.

---

## UI

| Asset | Use |
|-------|-----|
| 9-Patch panels (10 variants) | Dialog boxes, inventory panels, quest log, menus |
| Patch Side Deco (8 variants) | Panel decorations, section dividers |
| HP Bars (51x9 + 33x3) | Player health, enemy health, companion health |
| 16x16 Icons (14 variants) | Heart, coin, tools — HUD and inventory icons |
| Gun Icons (12 variants, 32x16) | Weapon slots in HUD and inventory |
| Reticles (10 designs) | Aiming cursor variants |
| Interact Icons (2 variants) | "Press to interact" prompts over NPCs/objects |
| Loading Icons (3 variants) | Area transition loading indicator |
| Icon Sprite Sheet | Consolidated icon atlas |

**Coverage**: UI assets are strong. Dialog boxes, HUD, inventory, and interaction prompts are all covered by existing art.

---

## Interactive Props

| Asset | RPG Use |
|-------|---------|
| Chests | Loot containers. Locked/unlocked variants? |
| Crafting Tables | Weapon/item upgrade stations |
| Forges | Weapon crafting (Dustport) |
| Ores | Crafting materials (Barrens, Vaults) |
| Quarries | Resource nodes |
| Camp objects | Save points? Rest areas? |

## Animals (ambient life)

| Asset | RPG Use |
|-------|---------|
| Chicken + Baby Chick | Dustport/Greenhollow ambient life |
| Sheep | Greenhollow livestock |
| Sci-fi Boar | Barrens wildlife (huntable?) |
| Sci-fi Rabbit | Ambient, all outdoor areas |

---

## What's Missing

### Critical Gaps
- [ ] **Townspeople sprites** — Only 4 merchants. Need generic NPCs for a living town. Options: repurpose tribe/starter sprites, or accept a sparse population.
- [ ] **Interior tilesets** — Buildings in Dustport need insides (bar, workshop, player quarters). Town builder tiles may cover this — needs investigation.
- [ ] **Door/entrance sprites** — Visual indicators for area transitions.
- [ ] **Portrait art** — For dialog boxes. Could skip (many pixel RPGs do) or use small sprite close-ups.

### Nice to Have
- [ ] **Weather/particle effects** — Dust storms, rain in the Murk, embers. Would need to be code-generated.
- [ ] **Mount/vehicle sprite** — If we want desert traversal beyond walking.
- [ ] **Emotion indicators** — Exclamation marks, question marks over NPC heads for quest state.
- [ ] **More weapon sprites** — The VFX are projectiles/effects, but visible held-weapon sprites beyond gun/blade.
- [ ] **Cutscene art** — Key story moments. Could be handled with in-engine camera work instead.

### Audio (Entirely TBD)
- Kenney sounds are placeholder. Need:
  - [ ] Ambient music per area (5 tracks minimum)
  - [ ] Combat music (1-2 tracks)
  - [ ] Dialog blips / typewriter sound
  - [ ] UI sounds (menu navigate, confirm, cancel)
  - [ ] Ambient SFX (wind, insects, machinery)
  - [ ] Weapon-specific sound effects
