# Drift — Biome & Wave Progression

## Structure

Three biomes, each a separate run. Completing a biome unlocks the next. Each run targets **~12-15 minutes** of gameplay. Each biome has **10 waves**, a distinct enemy roster of 6 types, and a boss encounter.

Players keep progression (hero unlocks, meta-upgrades if any) across runs. Each run starts fresh with wave 1 weapons/stats.

## Biome Overview

| Biome | Setting | Enemies | Identity | Boss |
|-------|---------|:-------:|----------|------|
| **The Waste** | Open desert, sand and rock | 6 | Simple creatures, learn the game | Dust Warrior |
| **Blood Desert** | Red sand, tribal structures | 6 | Organized foes, ranged/melee tactics | Blowfish |
| **The Temple** | Ancient interior, mechanical | 6 | Traps, AOE, assassins, controlled chaos | Tarnished Widow |

---

## Biome 1: The Waste

*Sunbaked desert crawling with wildlife and scrap. The player learns to fight, dodge, and prioritize.*

### Enemy Roster

| # | Enemy | Type | Role in Biome | Why here |
|---|-------|------|---------------|----------|
| 1 | Small Bug | Melee swarm | First enemy. Teaches "kill things that run at you" | Simple, fast, unthreatening alone |
| 2 | Medium Insect | Bump + acid | Background pressure. Acid trails restrict movement | Introduces area denial gently |
| 10 | Rusty Robot | Kamikaze bump | Urgency. Timed self-destruct forces reactions | Simple behavior, but demands attention |
| 13 | Delivery Bot | Drive-by ranged | First projectiles the player dodges | Low damage, teaches "not everything is melee" |
| 8 | Spiny Beetle | Ranged spread | Escalation. Spike volleys force wider dodges | Harder ranged threat than Delivery Bot |
| 7 | Big Bug | Melee slam + AOE | Miniboss-tier. Shockwaves claim space | The "oh shit" enemy of the biome |

### Wave Breakdown

**Wave 1 — "First Blood"** (40s)
- Small Bugs only. 8-10 total.
- Spawn 1-2 at a time from random edges. Gradual ramp.
- *Player learns:* movement, aiming, basic combat loop.

**Wave 2 — "Toxic Ground"** (45s)
- Small Bugs + Medium Insects. ~12 total.
- Medium Insects wander in from wave start. Small Bugs fill gaps.
- First acid trails appear when Medium Insects die.
- *Player learns:* not everything dies clean — watch where you step.

**Wave 3 — "Incoming"** (50s)
- Small Bugs + Medium Insects. ~15 total. Faster spawn rate.
- Larger packs of Small Bugs (3-4 at once). Pressure increases.
- *Player learns:* crowd management, weapon effectiveness against groups.

**Shop break.** Player has seen 2 enemy types, has a feel for the arena.

**Wave 4 — "Scrap Metal"** (55s)
- Add Rusty Robots. ~14 total.
- 2-3 Rusty Robots mixed with Small Bugs.
- Robots sprint from edges — first "fast threat" that demands snap targeting.
- *Player learns:* priority targeting. Robots die fast but punish if ignored (5s self-destruct).

**Wave 5 — "Drive-By"** (60s)
- Add Delivery Bots. ~16 total.
- 2 Delivery Bots strafe the arena while bugs and robots rush in.
- First time the player is shot at. Low damage, but novel.
- *Player learns:* dodging projectiles while fighting melee. Multitasking.

**Wave 6 — "Spike Storm"** (65s)
- Add Spiny Beetles. ~18 total.
- 2-3 Spiny Beetles zigzag in, firing spike spreads.
- Mixed with Rusty Robots and Small Bugs for melee pressure.
- *Player learns:* wide dodges (spike spread vs single shot). The arena starts feeling crowded.

**Shop break.** Player has full roster minus the boss-tier enemy.

**Wave 7 — "Tremor"** (75s)
- Add Big Bug. ~18 total.
- 1 Big Bug enters mid-wave. Ground slam shockwave is a dramatic moment.
- Supporting cast of bugs, robots, and beetles keep pressure on.
- *Player learns:* reading telegraphs, punishing long recovery windows (Big Bug's 1s buried state).

**Wave 8 — "Swarm Surge"** (80s)
- All enemy types. ~22 total.
- Heavy Small Bug spawns (5-6 at once) plus everything else.
- Big Bug present from wave start.
- *Player learns:* full biome vocabulary at once. Managing multiple threat types simultaneously.

**Wave 9 — "The Gauntlet"** (90s)
- All enemy types. ~25 total. Intense spawn rate.
- 2 Big Bugs on the field. Spiny Beetles and Delivery Bots provide ranged pressure.
- Spawn pacing: opens moderate, peaks at 60%, brief dip at 80%, then final surge.
- *Player learns:* sustained performance. This is the stress test before the boss.

**Wave 10 — "Dust Warrior"** (Boss wave, ~90s)
- Dust Warrior boss spawns after a brief 5s calm.
- Small Bugs spawn in trickles throughout (1 every 3s) to prevent pure boss focus.
- Boss enters Phase 2 at 50% HP (speed increase, whirlwind attack).
- *Player learns:* boss patterns, punish windows, fighting a boss while managing adds.

### Biome 1 Timing
| Phase | Waves | Time | Cumulative |
|-------|-------|------|-----------|
| Learning | 1-3 | ~2:15 | 2:15 |
| Shop | — | 0:20 | 2:35 |
| Escalation | 4-6 | ~3:00 | 5:35 |
| Shop | — | 0:20 | 5:55 |
| Peak | 7-9 | ~4:05 | 10:00 |
| Boss | 10 | ~1:30 | 11:30 |
| **Total** | | | **~12 min** |

---

## Biome 2: Blood Desert

*Red-stained sand and bone totems. The tribes have organized. Enemies fight with intent — they flank, kite, and hold ground.*

### Enemy Roster

| # | Enemy | Type | Role in Biome | Why here |
|---|-------|------|---------------|----------|
| 0 | Tribe Hunter | Ranged skirmisher | Mobile shooter, repositions after firing | The signature threat of this biome |
| 4 | Archer | Ranged predictive | Leads shots, punishes predictable movement | Precision ranged escalation |
| 5 | Guard | Melee tank | Frontal damage reduction, shield bash | Creates walls, protects ranged allies |
| 3 | Tribe Warrior | Heavy melee | Wide sweeping strikes, knockback resist | The enforcer — can't be ignored |
| 6 | Warrior | Berserker melee | Anti-kite rush + overhead slam | Punishes passive play |
| 9 | Relic Guardian | Charge predator | Bull rush, wall stun, snap bite | Late-biome terror |

### Wave Breakdown

**Wave 1 — "Outriders"** (45s)
- Tribe Hunters only. 6-8 total.
- Spawn at arena edges, advance to range, begin shooting.
- *Player learns:* closing distance on ranged enemies. Hunters flee when pressured — chase them down.

**Wave 2 — "Marksmen"** (50s)
- Tribe Hunters + Archers. ~12 total.
- Archers hang back further than Hunters, lead their shots.
- *Player learns:* unpredictable movement defeats predictive aiming. Two flavors of ranged require different dodging.

**Wave 3 — "Skirmish"** (55s)
- Tribe Hunters + Archers. ~14 total. Higher density.
- Archers begin using rapid volley (wounded behavior) when low HP.
- *Player learns:* finish wounded enemies fast or they become more dangerous.

**Shop break.**

**Wave 4 — "Shield Wall"** (60s)
- Add Guards. ~14 total.
- 2-3 Guards advance toward player. Hunters and Archers position behind them.
- First time the player faces an enemy that absorbs damage (frontal reduction).
- *Player learns:* flanking. Circle behind the Guard to deal full damage. The Guard + Archer combo is the biome's signature puzzle.

**Wave 5 — "Warband"** (70s)
- Add Tribe Warriors. ~16 total.
- 1-2 Tribe Warriors march in alongside Guards.
- Warriors' wide sweep attacks create danger zones. Guards block escape routes.
- *Player learns:* reading multiple telegraphs. The Warrior's 0.8s recovery is a window, but the Hunter behind you is still shooting.

**Wave 6 — "Blood and Iron"** (75s)
- All current types. ~18 total.
- Guards and Warriors form a front line. Hunters and Archers provide covering fire.
- *Player learns:* dealing with organized formations. Find the weak point — kill the ranged, flank the tanks.

**Shop break.**

**Wave 7 — "Berserkers"** (80s)
- Add Warriors (berserker variant). ~18 total.
- Warriors use rushing slash to close on kiting players.
- Mixed with Tribe Warriors for two different melee threats (wide sweep vs focused rush).
- *Player learns:* you can't just run. The Warrior closes gaps. Stand, fight, dodge through.

**Wave 8 — "Siege"** (85s)
- All types. ~22 total. Heavy Guard + Archer composition.
- 3-4 Guards creating a moving wall with Archers behind.
- Warriors and Tribe Warriors flank from sides.
- *Player learns:* the full tactical challenge. Break the formation or get cornered.

**Wave 9 — "The Hunt"** (90s)
- Add Relic Guardian. ~22 total.
- 1 Relic Guardian enters mid-wave. Bull rush across the arena.
- Full supporting cast. The Guardian charges through the chaos.
- *Player learns:* baiting charges into walls for 1.8s stun. Positioning near arena edges becomes a tool.

**Wave 10 — "Blowfish"** (Boss wave, ~100s)
- Blowfish boss spawns after 5s calm.
- Tribe Hunters spawn in trickles (1 every 4s) as adds.
- Phase 1: Burrow + spike fan. Phase 2: Inflation + spike ring. Phase 3: Spike trails shrink the arena.
- *Player learns:* spatial awareness. The arena itself becomes the enemy as spike trails accumulate.

### Biome 2 Timing
| Phase | Waves | Time | Cumulative |
|-------|-------|------|-----------|
| Learning | 1-3 | ~2:30 | 2:30 |
| Shop | — | 0:20 | 2:50 |
| Escalation | 4-6 | ~3:25 | 6:15 |
| Shop | — | 0:20 | 6:35 |
| Peak | 7-9 | ~4:15 | 10:50 |
| Boss | 10 | ~1:40 | 12:30 |
| **Total** | | | **~13 min** |

---

## Biome 3: The Temple

*Crumbling corridors and humming machinery. The enemies here don't just fight — they control space, set traps, and ambush. Nothing is safe.*

### Enemy Roster

| # | Enemy | Type | Role in Biome | Why here |
|---|-------|------|---------------|----------|
| 14 | Hooded Minion | Melee assassin | Dash-strikes from blind spots, punishes distraction | Opens the biome with tension |
| 12 | Circle Bot | AOE pulse | Expanding shockwave claims space | Forces constant repositioning |
| 16 | Ranged Minion | Suppressor | 3-shot bursts, staggered with allies | Pins the player down |
| 11 | Guard Robot | Juggernaut | Piston punch, stomp shockwave, unkillable feel | The wall that keeps coming |
| 15 | Bomb Minion | Suicide AOE | Chain reactions, chaos in crowds | Risk/reward — detonate near enemies? |
| 17 | Planter Bot | Trapper | Mines reshape the arena over time | The long-game threat |

### Wave Breakdown

**Wave 1 — "Shadows"** (45s)
- Hooded Minions only. 6-8 total.
- They stalk at range, then dash-strike. A dramatic tonal shift from Biome 2.
- *Player learns:* tracking enemies that wait. The Minion's dash is fast — learn the tell.

**Wave 2 — "Pulse"** (50s)
- Hooded Minions + Circle Bots. ~12 total.
- Circle Bots park at mid-range and begin pulsing. Expanding shockwaves force movement.
- Hooded Minions dash-strike while the player is dodging pulses.
- *Player learns:* you can't stand still (pulses) and you can't tunnel vision (assassins). The biome's core tension.

**Wave 3 — "Crossfire"** (55s)
- Add more Circle Bots. ~14 total.
- 2-3 Circle Bots creating overlapping pulse zones.
- Hooded Minions exploit the chaos.
- *Player learns:* reading multiple AOE zones, finding safe lanes between overlapping pulses.

**Shop break.**

**Wave 4 — "Suppression"** (65s)
- Add Ranged Minions. ~16 total.
- 2-3 Ranged Minions fire burst volleys while Circle Bots control space.
- Hooded Minions still lurking.
- *Player learns:* projectile weaving under sustained fire. Ranged Minions' reload window (1s) is the attack window.

**Wave 5 — "Iron Wall"** (70s)
- Add Guard Robot. ~16 total.
- 1 Guard Robot enters. Cannot be knocked back. Piston punch has serious reach.
- Ranged Minions fire from behind it. Circle Bots pulse on the flanks.
- *Player learns:* patience. The Guard Robot demands sustained damage while everything else demands evasion. Learn the punch-punch-stomp pattern.

**Wave 6 — "Temple Guard"** (75s)
- All current types. ~18 total.
- Guard Robot + Ranged Minion backline + Circle Bot zones + Hooded Minion flanks.
- The full "organized defense" of the temple.
- *Player learns:* target priority in the biome's ecosystem. Kill the Ranged Minions first? The Circle Bots? Or focus the unkillable Guard Robot?

**Shop break.**

**Wave 7 — "Volatile"** (80s)
- Add Bomb Minions. ~20 total.
- 3-4 Bomb Minions rush in among other enemies.
- Tiny sprites (13x15) are hard to spot in the crowd. Chain reactions possible.
- *Player learns:* scanning for Bomb Minions. Detonating one near a cluster of enemies is a reward. Getting caught in a chain is devastating.

**Wave 8 — "Minefield"** (85s)
- Add Planter Bot. ~20 total.
- 1-2 Planter Bots begin mining the arena. Mines cut off escape routes.
- Bomb Minions rushing through mine fields can trigger them (clearing mines but creating explosions).
- *Player learns:* kill Planter Bots immediately or the arena shrinks. Mines + Circle Bot pulses = nowhere to stand.

**Wave 9 — "The Descent"** (95s)
- All types. ~24 total. Maximum intensity.
- 2 Guard Robots. Multiple Planter Bots mining the arena.
- Bomb Minions, Ranged Minions, Circle Bots, Hooded Minions — everything.
- Spawn pacing: relentless for first 60s, brief 5s lull, then final surge.
- *Player learns:* everything they know at maximum pressure. Survival mode.

**Wave 10 — "Tarnished Widow"** (Boss wave, ~110s)
- Tarnished Widow spawns after 5s calm.
- Hooded Minions spawn as adds (1 every 5s). Widow also summons spiderlings (Small Bugs) in Phase 2.
- Phase 1: Web spit + leg sweep combo. Phase 2: Pounce + spiderling summons. Phase 3: Blood frenzy — speed increase, web on every pounce landing, rapid leg barrage.
- *Player learns:* the final exam. Dodge webs, avoid sweeps, manage adds, survive the frenzy.

### Biome 3 Timing
| Phase | Waves | Time | Cumulative |
|-------|-------|------|-----------|
| Learning | 1-3 | ~2:30 | 2:30 |
| Shop | — | 0:20 | 2:50 |
| Escalation | 4-6 | ~3:30 | 6:20 |
| Shop | — | 0:20 | 6:40 |
| Peak | 7-9 | ~4:20 | 11:00 |
| Boss | 10 | ~1:50 | 12:50 |
| **Total** | | | **~13 min** |

---

## Wave Pacing (Within a Wave)

Each wave has internal pacing rather than dumping all enemies at once:

```
Wave Timeline:
|---RAMP---|---BUILD---|---PEAK---|---SURGE---|
0%        25%        55%        80%        100%

RAMP:   1-2 enemies at a time. Let the player orient.
BUILD:  2-3 at a time. New enemy type may appear here.
PEAK:   3-5 at a time. Max concurrent enemies on screen.
SURGE:  Final burst. Short, intense, then wave ends.
```

### Spawn Rate Curve (enemies per second)

| Wave Phase | Early Waves (1-3) | Mid Waves (4-6) | Late Waves (7-9) | Boss Wave |
|-----------|:-----------------:|:----------------:|:-----------------:|:---------:|
| Ramp | 0.3 | 0.4 | 0.5 | 0.2 (adds only) |
| Build | 0.5 | 0.7 | 0.9 | 0.3 |
| Peak | 0.8 | 1.0 | 1.3 | 0.3 |
| Surge | 1.0 | 1.3 | 1.8 | 0.0 (focus boss) |

### Enemy Density Cap
Maximum concurrent enemies on screen at once (prevents overwhelming the renderer and the player):

| Biome | Early Waves | Mid Waves | Late Waves | Boss Wave |
|-------|:-----------:|:---------:|:----------:|:---------:|
| The Waste | 10 | 14 | 18 | 8 + boss |
| Blood Desert | 10 | 14 | 18 | 8 + boss |
| The Temple | 8 | 12 | 16 | 6 + boss |

Temple has lower caps because its enemies are individually more complex and dangerous.

---

## Difficulty Scaling

### Within a Biome (wave-over-wave)
- **HP scaling:** +8% per wave (not +12% as current). Flatter curve so early waves don't feel trivial.
- **Damage scaling:** +5% per wave. Damage ramps slower than HP.
- **Mini-bosses:** Waves 7 and 9 can spawn 1 "elite" enemy (1.5x scale, slight glow). Not a full boss, just a tougher version of a regular enemy.

### Across Biomes
- **Biome 2 baseline:** Enemies start at ~1.3x Biome 1's base stats.
- **Biome 3 baseline:** Enemies start at ~1.6x Biome 1's base stats.
- This is baked into the enemy definitions per biome, not a global multiplier.

---

## Unlock Flow

```
[The Waste] --complete--> [Blood Desert] --complete--> [The Temple]
                                                            |
                                                       [Endless?]
```

- Completing a biome = defeating the boss on wave 10.
- Dying restarts the current biome from wave 1.
- Hero unlocks and any meta-progression persist across deaths.
- Optional: "Endless" mode after completing all three biomes — all 18 enemy types, escalating waves, how long can you survive?

---

## Enemy Migration Notes

### Code name cleanup
Several enemies still use legacy Kenney names in EnemyDef. Biome implementation is a good time to rename:

| Current Code Name | Should Be | Biome |
|-------------------|-----------|-------|
| Scorpion | Tribe Hunter | Blood Desert |
| Snake | Small Bug | The Waste |
| Bat | Medium Insect | The Waste |
| Beetle | Tribe Warrior | Blood Desert |

### From current → proposed

Current system: All 18 enemies in one pool, unlocked progressively over 20 waves.
Proposed system: 6 enemies per biome × 3 biomes = 18 enemies across 30 waves total.

This means each enemy gets more time to shine — introduced in wave N, mastered by wave N+3, combined with everything else by wave N+5. Currently an enemy introduced in wave 12 only has 8 waves of relevance. In the new system, every enemy is relevant for most of its biome.
