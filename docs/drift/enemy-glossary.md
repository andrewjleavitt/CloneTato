# Drift — Enemy Glossary

## Attack Types

All enemies currently deal bump/contact damage on collision. From wave 4+, random enemies can spawn armed with ranged weapons. Only bosses have dedicated melee attacks. The sprite art supports distinct attack patterns per enemy — this doc tracks the intended design for wiring them up.

| Attack Type | Description |
|-------------|-------------|
| **Ranged** | Maintains distance, fires projectiles at the player |
| **Melee** | Closes to attack range, plays attack animation, deals damage in an arc/cone |
| **AOE** | Area-of-effect damage — energy pulses, explosions, ground hazards |
| **Bump** | Contact damage only, no distinct attack animation |

## Factions & Enemies

### Tribe

| # | Code Name | Sprite | HP | Spd | Dmg | Behavior | Attack Anims | Attack Type |
|---|-----------|--------|---:|----:|----:|----------|-------------|-------------|
| 0 | Scorpion | Tribe Hunter | 25 | 45 | 8 | Chase | shoot (3-dir, 7f) | **Ranged** |
| 3 | Beetle | Tribe Warrior | 60 | 30 | 15 | Tank | attack (3-dir, 11f) | **Melee** — wide weapon sweep |
| 9 | Relic Guardian | Tamed Beast | 70 | 32 | 16 | Tank | attack (3-dir, 16f) | **Melee** — charge/lunge bite |

The Tribe Hunter is the first enemy the player encounters (wave 1). Its ranged attack should establish the "dodge projectiles" dynamic early.

### Insect

| # | Code Name | Sprite | HP | Spd | Dmg | Behavior | Attack Anims | Attack Type |
|---|-----------|--------|---:|----:|----:|----------|-------------|-------------|
| 1 | Snake | Small Bug | 15 | 70 | 5 | FastChase | attack (6f) | **Melee** — quick bite |
| 2 | Bat | Medium Insect | 18 | 55 | 6 | Erratic | attack (10f) | **Bump** — no distinct attack needed |
| 7 | Big Bug | Big Bug | 80 | 25 | 18 | Tank | attack (6f) | **Melee** — ground slam |
| 8 | Spiny Beetle | Spiny Beetle | 30 | 60 | 10 | Erratic | attack (8f) | **Ranged** — spike projectile |

Insects are non-directional sprites (same art all directions). Small Bugs swarm in numbers; Big Bug is a slow heavy hitter.

### Humanoid (Starter Pack)

| # | Code Name | Sprite | HP | Spd | Dmg | Behavior | Attack Anims | Attack Type |
|---|-----------|--------|---:|----:|----:|----------|-------------|-------------|
| 4 | Archer | Starter Archer | 20 | 40 | 10 | Erratic | shoot (7f) | **Ranged** — bow attack |
| 5 | Guard | Starter Guard | 45 | 35 | 12 | Chase | attack (5f) | **Melee** — shield bash |
| 6 | Warrior | Starter Warrior | 55 | 38 | 14 | Chase | attack (5f) | **Melee** — overhead swing |

Humanoids use 32x32 grid sheets. Guard is the tankiest non-boss enemy in this faction.

### Robot

| # | Code Name | Sprite | HP | Spd | Dmg | Behavior | Attack Anims | Attack Type |
|---|-----------|--------|---:|----:|----:|----------|-------------|-------------|
| 10 | Rusty Robot | Rusty Robot | 20 | 65 | 7 | FastChase | none | **Bump** — kamikaze rusher |
| 11 | Guard Robot | Guard Robot | 50 | 30 | 14 | Tank | attack (10f) | **Melee** — extending arms |
| 12 | Circle Bot | Circle Bot | 35 | 45 | 10 | Erratic | attack (8f) | **AOE** — energy pulse |
| 13 | Delivery Bot | Delivery Bot | 12 | 80 | 4 | FastChase | attack (6f) | **Ranged** — small projectile |
| 17 | Planter Bot | Planter Bot | 28 | 35 | 6 | Chase | attack (24f!) | **AOE** — plants ground hazard |

Rusty Robot has no attack animation (death = walk). Planter Bot has the longest attack animation in the game (24 frames).

### Minion

| # | Code Name | Sprite | HP | Spd | Dmg | Behavior | Attack Anims | Attack Type |
|---|-----------|--------|---:|----:|----:|----------|-------------|-------------|
| 14 | Hooded Minion | Hooded Minion | 30 | 42 | 11 | Chase | attack (10f) | **Melee** — scythe swing |
| 15 | Bomb Minion | Bomb Minion | 10 | 75 | 3 | FastChase | prep explode (4f) + explosion (15f, 124x77) | **AOE** — suicide detonation |
| 16 | Ranged Minion | Ranged Minion | 22 | 38 | 8 | Erratic | attack (15f) | **Ranged** — sustained fire |

Bomb Minion has a unique two-part death: prep → large explosion sprite (124x77, much bigger than body at 13x15).

## Bosses

| Type | Sprite | Attack Anims | Attack Type | Notes |
|------|--------|-------------|-------------|-------|
| Dust Warrior | 67x45 strips | attack (8f) | **Melee** | Default boss, waves 3-5 |
| Blowfish | 94x47 strips | burrow attack (13f) | **Melee/AOE** | Unwired: small form, grow/shrink, spike trail |
| Tarnished Widow | 188x90 grid | attack w/ blood (10f) | **Melee + phases** | Unwired rows 2/3/5/6 suggest multi-phase |

Boss cycle: Dust Warrior → alternating with Blowfish → all three in rotation from wave 10+.

## Summary by Attack Type

| Attack Type | Enemies | Count |
|-------------|---------|------:|
| **Ranged** | Tribe Hunter, Archer, Spiny Beetle, Delivery Bot, Ranged Minion | 5 |
| **Melee** | Tribe Warrior, Relic Guardian, Small Bug, Big Bug, Guard, Warrior, Guard Robot, Hooded Minion | 8 |
| **AOE** | Circle Bot (pulse), Bomb Minion (suicide), Planter Bot (ground trap) | 3 |
| **Bump only** | Rusty Robot, Medium Insect | 2 |

## Wave Unlock Progression

| Wave | New Enemies | Running Total |
|------|------------|:-------------:|
| 1 | Scorpion (Tribe Hunter), Snake (Small Bug) | 2 |
| 2 | Rusty Robot | 3 |
| 3 | Bat (Medium Insect) | 4 |
| 4 | Archer | 5 |
| 5 | Spiny Beetle, Delivery Bot | 7 |
| 6 | Beetle (Tribe Warrior) | 8 |
| 7 | Big Bug | 9 |
| 8 | Circle Bot | 10 |
| 9 | Bomb Minion | 11 |
| 10 | Guard Robot, Hooded Minion | 13 |
| 11 | Guard | 14 |
| 12 | Warrior, Ranged Minion | 16 |
| 13 | Relic Guardian | 17 |
| 14+ | Planter Bot | 18 |

Boss waves: 3, 5, 6, 9, 10, 12, 15, 18 (every 3rd-5th wave from wave 3).

## Behavior Types

| Behavior | Movement Pattern | Used By |
|----------|-----------------|---------|
| **Chase** | Direct path toward player | Tribe Hunter, Guard, Warrior, Hooded Minion, Planter Bot, Tribe Warrior |
| **FastChase** | High-speed direct pursuit | Small Bug, Rusty Robot, Delivery Bot, Bomb Minion |
| **Tank** | Slow, heavy, direct | Tribe Warrior, Big Bug, Relic Guardian, Guard Robot |
| **Erratic** | Sinusoidal zigzag toward player | Medium Insect, Archer, Spiny Beetle, Circle Bot, Ranged Minion |

Note: Armed enemies (wave 4+) override movement to maintain preferred range and strafe.

---

## Attack Patterns

Each enemy should have a distinct behavior loop that creates a readable rhythm the player can learn and exploit. The goal is variety — every new wave introduction should change how the player moves through the arena.

### Design Principles

1. **Telegraphs before damage.** Every attack has a wind-up the player can read. Faster enemies get shorter telegraphs; slower enemies get longer ones with bigger payoff.
2. **Enemies create space problems.** The interesting decision is never "can I dodge this one thing" — it's "where do I go when three things are happening at once." Each enemy type should claim space differently.
3. **Escalation through combination.** A Tribe Hunter alone is simple. A Tribe Hunter behind a Tribe Warrior who's blocking your escape route is a problem. Patterns should create emergent pressure when mixed.
4. **Recovery windows.** Every enemy has a moment of vulnerability after attacking. Aggressive players are rewarded for learning the timing.

---

### Tribe Faction

#### Tribe Hunter — *Mounted Skirmisher*
**Role:** Early-game ranged pressure. Teaches the player to close distance or dodge projectiles.

- **Approach:** Advances toward the player until reaching preferred range (~120px). Moves laterally while at range, circling the player.
- **Attack — Aimed Shot:** Pauses movement for ~0.3s (wind-up), fires a single projectile at the player's current position. Projectile is moderately fast, narrow.
- **Recovery:** After firing, dashes ~40px away from the player (repositioning). Brief cooldown (1.5s) before next shot.
- **Personality:** Never wants to be close. If the player closes to melee range, the hunter breaks into a panicked sprint away before resuming its pattern. This rewards aggressive play — pressure them and they stop shooting.

#### Tribe Warrior — *Heavy Enforcer*
**Role:** Mid-game melee wall. Forces the player to deal with a slow, dangerous obstacle that can't be ignored.

- **Approach:** Walks steadily toward the player. Does not chase aggressively — maintains a deliberate march. Absorbs punishment on the way in.
- **Attack — Sweeping Strike:** When within range (~35px), plants feet (stops moving), raises weapon (~0.5s telegraph), then sweeps a wide 180-degree arc in front. High damage, moderate knockback.
- **Recovery:** After the swing, stands still for ~0.8s (weapon dragging on ground). Vulnerable window — player can circle behind for free hits.
- **Personality:** The immovable object. Doesn't flinch from knockback easily (50% knockback resistance). When multiple Warriors advance together, they create a closing wall the player has to find gaps in.

#### Relic Guardian (Tamed Beast) — *Charging Predator*
**Role:** Late-game pursuit threat. Punishes players who run in straight lines.

- **Approach:** Stalks the player at moderate speed, tracking their position. Maintains a ~100px distance while sizing up.
- **Attack — Bull Rush:** Lowers head (0.6s telegraph — sprite shifts down slightly), then charges in a straight line at 2.5x normal speed. Deals heavy damage on contact. Travels ~200px or until hitting arena edge.
- **Recovery:** After the charge, skids to a stop and is stunned for 1.2s (shaking head). Very vulnerable. If it hits the arena wall during a charge, stun extends to 1.8s.
- **Second Attack — Snap Bite:** If the player is within melee range (~30px) and the beast isn't in charge mode, quick bite with minimal telegraph (0.2s). Lower damage than the charge, but keeps close-range players honest.
- **Personality:** Terrifying in open space, exploitable near walls. The player learns to bait charges into arena edges for big punish windows.

---

### Insect Faction

#### Small Bug — *Swarm Fodder*
**Role:** Constant low-level pressure. Individually trivial, dangerous in numbers. The "popcorn" enemy.

- **Approach:** Beelines toward the player at high speed. Spawns in clusters of 3-5.
- **Attack — Lunge Bite:** When within ~20px, lunges forward a short distance with a quick snap. Low damage, very fast. No real telegraph — the approach IS the telegraph.
- **Recovery:** After biting, briefly pauses (~0.3s) before resuming chase. Almost no recovery window — these die to AOE and cleave, not to careful timing.
- **Personality:** Individually nothing. But they fill screen space and force the player to sweep weapons through them while dodging more dangerous enemies. They're the "tax" the player pays for not having AOE weapons.

#### Medium Insect — *Ambient Hazard*
**Role:** Passive area denial. Not aggressive, but punishes careless movement.

- **Approach:** Wanders erratically in the player's general direction. Not urgent about closing distance.
- **Attack — Contact Toxin:** Bump damage only, but leaves a small acid pool (~20px radius) on the ground where it dies. Pool persists for 3s, deals damage-over-time to the player standing in it.
- **Passive — Acid Trail:** While alive, periodically drips acid behind itself (every ~2s), leaving tiny hazard spots that last 1.5s. Creates lanes the player wants to avoid.
- **Personality:** The enemy you forget about until you dodge a Tribe Warrior's swing directly into the acid trail the Medium Insect left behind. Not threatening alone — dangerous in combination.

#### Big Bug — *Area Denier*
**Role:** Forces the player to reposition. Controls space with telegraphed but devastating attacks.

- **Approach:** Lumbers toward the player. Slow but relentless. Does not deviate from its path easily.
- **Attack — Ground Slam:** When within ~40px, rears up (0.7s telegraph — body lifts visibly), then slams down. Creates a shockwave ring that expands outward (~60px radius). Damage falls off with distance. Shakes the screen.
- **Recovery:** After slamming, partially buried in the ground for 1.0s. Cannot move or attack. Clear punish window.
- **Personality:** You always know the slam is coming. The challenge is that while you're dodging the Big Bug's shockwave, the Small Bugs are closing in and the Tribe Hunter is lining up a shot. The Big Bug is the enemy that makes you move to a bad spot.

#### Spiny Beetle — *Mobile Artillery*
**Role:** Erratic ranged threat. Harder to pin down than the Tribe Hunter, but less accurate.

- **Approach:** Zigzags toward the player, never moving in a straight line. Pauses periodically at medium range.
- **Attack — Spike Volley:** Stops moving, curls up briefly (0.4s), then fires 3 spikes in a ~30-degree spread toward the player. Individual spikes are slower than Tribe Hunter projectiles but the spread is harder to dodge cleanly.
- **Recovery:** After firing, uncurls and skitters to a new position (~50px lateral movement). Cooldown 2.0s.
- **Personality:** The Tribe Hunter you can't just chase down. Its erratic movement makes it hard to hit, and the spike spread punishes players who dodge in a straight line. Forces wider dodge arcs.

---

### Humanoid Faction

#### Archer — *Disciplined Marksman*
**Role:** Precise ranged threat. More dangerous than the Tribe Hunter because it aims ahead of the player.

- **Approach:** Maintains ~130px distance. Sidesteps smoothly to maintain line of sight. Retreats if the player closes to ~70px.
- **Attack — Predictive Shot:** Brief stance shift (0.3s), fires a single arrow at where the player will be (leads the target based on player velocity). Faster projectile than Tribe Hunter, but single-target.
- **Second Attack — Rapid Volley (below 50% HP):** When wounded, fires 2 arrows in quick succession (0.15s apart) at slightly different lead angles. Panic fire — more dangerous but less accurate.
- **Recovery:** Minimal — 0.2s pause after firing, then immediately repositions. Very slippery. Cooldown 1.8s.
- **Personality:** The sniper. Punishes players who run in predictable patterns. The counter is to change direction unpredictably or rush it down — it's fragile (20 HP). The wounded rapid-fire makes it dangerous to leave alive at low health.

#### Guard — *Advancing Wall*
**Role:** Melee tank that creates positioning problems. Absorbs hits meant for squishier enemies behind it.

- **Approach:** Walks toward the player with shield raised. While approaching, takes 40% reduced damage from the front (direction it's facing). Cannot be knocked back while advancing.
- **Attack — Shield Bash:** When within ~25px, lunges forward with shield. Moderate damage, strong knockback (pushes the player away). The bash is fast (0.3s telegraph).
- **Flank Vulnerability:** Damage from behind or sides deals full damage and staggers the Guard briefly (0.3s). Rewards circling around.
- **Recovery:** After bash, lowers shield for 0.6s (catches breath). Full damage from all directions during this window.
- **Personality:** The bodyguard. A Guard advancing in front of an Archer is a real problem — you can't easily reach the Archer, and the Guard keeps pushing you back. The player learns to circle or use AOE to deal with the pair.

#### Warrior — *Berserker*
**Role:** Aggressive melee that punishes passive play. Forces the player to fight rather than kite.

- **Approach:** Runs directly at the player. Slightly faster than other melee enemies. Accelerates the closer it gets (speed +20% within 80px).
- **Attack — Overhead Slam:** When within ~30px, raises weapon high (0.4s telegraph), brings it down in a focused strike. High damage, narrow hitbox (directly in front). Miss = weapon stuck in ground.
- **Attack — Rushing Slash:** If the player stays between 40-80px (kiting range), the Warrior lunges forward ~50px with a horizontal slash. Less damage than the overhead, but closes the gap and catches kiters.
- **Recovery:** Overhead has 0.7s recovery (pulling weapon from ground). Rushing slash has 0.4s recovery. Both are punishable.
- **Personality:** The anti-kiter. If you try to maintain distance, the rushing slash closes the gap. If you stand and fight, the overhead hits hard. The right play is to dodge through the attacks and hit during recovery — rewarding mechanical skill.

---

### Robot Faction

#### Rusty Robot — *Kamikaze Runner*
**Role:** Expendable fast threat. Forces snap reactions. The "oh no" enemy.

- **Approach:** Sprints directly at the player at high speed (65). No subtlety, no tactics.
- **Attack — Self-Destruct Contact:** On collision with the player, deals bump damage and dies. On death (from any cause), emits a small damaging burst (~15px radius). Not as dramatic as the Bomb Minion, but enough to punish melee players who don't step back after the kill.
- **Special — Countdown:** Begins sparking/flashing 2s after spawn. After 5s alive, self-destructs wherever it is (same small burst). This prevents them from being ignored — they're on a timer.
- **Personality:** The enemy that keeps you moving. When 3 Rusty Robots spawn from different edges, the player has to decide which one to deal with first. They force prioritization.

#### Guard Robot — *Mechanical Juggernaut*
**Role:** The heaviest non-boss enemy. Demands sustained attention.

- **Approach:** Slow, deliberate advance. Cannot be knocked back. Pauses periodically to scan (brief idle animation).
- **Attack — Piston Punch:** When within ~30px, extends both arms forward (0.5s telegraph — arms visibly retract first). Long reach (~40px), hits everything in a narrow cone. High damage.
- **Attack — Shockwave Stomp:** Every 3rd attack, stomps instead of punching. Creates a small circular shockwave (~35px). Less damage than the punch but harder to dodge.
- **Recovery:** Piston punch has 0.8s recovery (arms slowly retract). Stomp has 0.5s recovery. The punch recovery is the main punish window.
- **Personality:** A siege engine. Ignore it and it reaches you. Fight it and it takes forever to kill (50 HP + Tank behavior). The player learns its attack cadence — punch, punch, stomp — and times their aggression around the recovery windows.

#### Circle Bot — *Zone Controller*
**Role:** Area denial through AOE pulses. Forces the player out of comfortable positions.

- **Approach:** Erratic movement toward the player, settling at ~60px distance. Orbits the player loosely.
- **Attack — Energy Pulse:** Stops and charges up (0.5s — visible energy gathering effect), then releases a circular shockwave that expands outward from its position. Radius ~50px. Moderate damage, no knockback. The wave is visible and dodgeable by moving outside the radius during the charge.
- **Attack — Pulse Chain:** On later waves (10+), fires 2 pulses in succession (second 0.4s after first, slightly larger radius). Forces the player to keep moving rather than dodge-and-return.
- **Recovery:** After pulsing, energy depleted — moves at 50% speed for 1.0s. Good chase-down window.
- **Personality:** The enemy that says "you can't stand there." When a Circle Bot parks itself in the middle of the arena and starts pulsing, the player has to relocate. Combined with a Tribe Warrior blocking one exit, this creates real spatial puzzles.

#### Delivery Bot — *Strafing Runner*
**Role:** Fast drive-by threat. Hard to hit, easy to kill if you catch it.

- **Approach:** Runs in a wide arc around the player at high speed (80), maintaining ~90px distance. Constantly moving laterally.
- **Attack — Drive-By Shot:** While running, fires a single projectile at the player without stopping. Low damage, moderate accuracy. Fires every 1.5s.
- **Special — Speed Boost:** If hit but not killed, gains a 1.5s speed boost (speed +30%). Getting grazed makes it harder to finish off.
- **Recovery:** None — it never stops moving. The recovery window is positioning: cut off its arc by moving toward where it's heading, not where it is.
- **Personality:** The mosquito. Individually annoying, not dangerous. But while you're chasing the Delivery Bot, the Guard Robot is closing in behind you. Its role is distraction — it taxes the player's attention.

#### Planter Bot — *Trapper*
**Role:** Battlefield control through persistent hazards. Changes the geometry of the fight.

- **Approach:** Advances toward the player at moderate speed. Periodically stops to plant.
- **Attack — Deploy Mine:** Stops moving, plays long planting animation (24 frames, ~2s). Places a mine on the ground at its current position. Mine arms after 1s, then persists for 8s. Armed mines deal high damage + knockback to the player on proximity (~18px trigger radius). Visible as a small blinking object.
- **Limit:** Max 3 active mines per Planter Bot. Prioritizes planting mines between the player and arena exits / chokepoints.
- **Mine Placement AI:** Prefers to plant mines where the player has recently been (last 2s of player positions). This means the player's escape routes get gradually cut off.
- **Recovery:** Immobile during the entire planting animation. Very vulnerable — the 2s plant time is a long punish window. Smart players kill Planter Bots on sight before the arena fills with mines.
- **Personality:** The long-game enemy. A Planter Bot that lives for 15 seconds has reshaped the entire arena. The player learns to prioritize them immediately, which is itself a problem when other enemies need attention too. Creates interesting priority decisions.

---

### Minion Faction

#### Hooded Minion — *Assassin*
**Role:** Burst melee threat. Sneaks close, hits hard, disengages.

- **Approach:** Walks toward the player at moderate speed. When within ~70px, slows down and begins circling (stalking). Waits for the player to be occupied with another enemy.
- **Attack — Dash Strike:** From stalking range, dashes toward the player (~60px distance covered in 0.2s) and swings scythe. High damage, fast. The dash IS the telegraph — you see it coming but have very little time.
- **Recovery:** After the dash strike, continues past the player for ~30px (momentum carry), then pauses for 0.5s before resuming stalk.
- **Special — Opportunist:** Attack cooldown is shorter (1.2s) if the player is currently being hit by another enemy or in knockback. Punishes getting caught by other attacks.
- **Personality:** The enemy that hits you when you're not looking. You're dodging a Big Bug slam and suddenly take a scythe to the back. The player learns to track Hooded Minion positions even while handling other threats. Raises the skill ceiling.

#### Bomb Minion — *Suicide Bomber*
**Role:** Explosive area threat. Creates chaos and forces immediate reactions.

- **Approach:** Sprints toward the player at high speed (75). Small and hard to see among other enemies (13x15 sprite).
- **Attack — Proximity Detonation:** When within ~25px of the player, stops and plays prep-explode animation (4 frames, ~0.5s). Then explodes with a large blast (124x77 sprite, ~45px damage radius). Very high damage at center, moderate at edge. Damages other enemies too.
- **On-Death Explosion:** If killed before reaching the player, still explodes but with a smaller radius (~25px) and less damage. There is no safe way to kill a nearby Bomb Minion — only less dangerous ways.
- **Chain Reaction:** Explosion can trigger other nearby Bomb Minions' detonations. Multiple Bomb Minions can cascade.
- **Personality:** Pure chaos. The tiny sprite among a crowd of enemies creates an "oh no, where is it?" moment. Players learn to listen for and spot them early, or to kill them at a distance. The chain reaction mechanic means a cluster of them is either a huge threat or a crowd-clearing opportunity if you detonate them among other enemies.

#### Ranged Minion — *Suppressor*
**Role:** Sustained ranged pressure. Pins the player down with volume of fire.

- **Approach:** Maintains ~110px distance. Moves slowly, prioritizing firing position over safety.
- **Attack — Burst Fire:** Fires 3 projectiles in quick succession (0.2s apart) in a tight spread. Each projectile does low damage. Total burst takes ~0.6s. Projectiles are slow but numerous — harder to dodge all three.
- **Recovery:** After a burst, brief reload (1.0s) where it's stationary and vulnerable.
- **Special — Suppressing Fire:** If multiple Ranged Minions are alive, they stagger their bursts so the player faces near-continuous incoming fire. (This can be implicit from spawn timing rather than explicit coordination.)
- **Personality:** The bullet hell enemy. One is manageable. Three firing staggered bursts turns the arena into a projectile maze. Forces the player to weave through gaps while still dealing with melee threats. Combined with a Guard blocking the approach, this creates the "raid boss" small-enemy composition.

---

### Bosses

#### Dust Warrior — *The Proving Ground*
**Role:** First boss. Tests fundamentals: dodge timing, pattern recognition, punish windows. Fair and learnable.

**Phase 1 (100-50% HP):**
- **Advancing Slash:** Walks toward player, swings weapon in a horizontal arc at ~35px range. 0.4s telegraph (arm draws back). Moderate damage.
- **Lunging Thrust:** At ~60px distance, lunges forward with a stab. Covers ~40px. 0.5s telegraph (crouches). Higher damage than slash, narrower hitbox.
- **Pattern:** Alternates slash → slash → lunge. Predictable rhythm the player can learn.
- **Recovery:** 0.6s after each attack. Punishable.

**Phase 2 (below 50% HP):**
- **Speed increase:** Movement and attack speed +25%.
- **New Attack — Whirlwind:** Spins in place for 1s, damaging everything nearby (~40px). Used when surrounded or when player is directly on top. 0.3s telegraph (plants feet wide).
- **Pattern breaks:** No longer alternates predictably. Mixes attacks based on player position.
- **Recovery windows shorten** to 0.4s.

**Design intent:** The Dust Warrior is the "exam" for everything learned in waves 1-3. If the player can read telegraphs, dodge, and punish recovery windows, they pass.

#### Blowfish — *The Terrain Boss*
**Role:** Second boss. Introduces environmental awareness — attacks that change the arena.

**Phase 1 (100-60% HP):**
- **Burrow:** Digs underground (invulnerable, 1.5s), moves to a new position, erupts upward dealing damage in a ~35px radius. Dust cloud marks the emergence point 0.5s before surfacing. Teaches the player to watch the ground.
- **Spike Shot:** Fires 5 spikes in a fan pattern from its mouth. Wide spread, moderate speed. Standard ranged attack between burrows.
- **Cooldown:** 2s between attacks, slow and readable.

**Phase 2 (60-30% HP):**
- **Inflation:** Body swells to 1.5x size over 1s. While inflated, contact damage doubles and collision radius increases. Deflates after 3s or after being hit 5 times. Inflating is the telegraph for the next attack.
- **Spike Ring:** While inflated, releases spikes in all 8 cardinal/diagonal directions simultaneously. Forces the player to be between the gaps.
- **Faster burrows:** Underground time reduced to 1.0s, less warning.

**Phase 3 (below 30% HP):**
- **Spike Trail:** Leaves a trail of damaging spikes behind as it moves on the surface. Trail persists for 4s. Gradually fills the arena with hazards.
- **Frenzied Burrow:** Burrows and surfaces 3 times rapidly in succession, zigzagging across the arena.

**Design intent:** The Blowfish teaches spatial awareness and arena management. The fight gets harder not because the boss hits harder, but because the safe space shrinks.

#### Tarnished Widow — *The Final Exam*
**Role:** Third boss. Combines everything — projectiles, melee, area denial, and adds minion management.

**Phase 1 (100-70% HP):**
- **Leg Sweep:** Broad melee attack (~50px arc) when player is close. Fast telegraph (0.3s, legs shift). High damage, moderate knockback.
- **Web Spit:** Fires a glob of web at the player's position. On landing, creates a web zone (~30px radius) that slows the player by 50% for 3s. The web is the setup for the leg sweep — get webbed, get swept.
- **Pattern:** Web → advance → sweep. Readable and fair.

**Phase 2 (70-40% HP):**
- **Summon Spiderlings:** Spawns 4-6 Small Bugs from its position. Does this once every ~12s. The swarm adds pressure while the player deals with the boss.
- **Pounce:** Leaps to the player's position (0.6s telegraph — crouches), landing with AOE damage (~40px). Used at medium range. Closes distance instantly.
- **Web Trap Network:** Places 2 webs in quick succession, creating zones the player has to navigate around.

**Phase 3 (below 40% HP):**
- **Blood Frenzy:** Attack speed +40%, movement speed +20%. Web spit cooldown halved.
- **Leg Barrage:** Rapid series of 3 leg sweeps in succession, each covering a different arc. Nearly 360-degree coverage. Small gap between sweep 2 and 3 for dodge window.
- **Constant spiderling spawns** every 8s.
- **Desperation Pounce:** Pounce now leaves a web on the landing zone.

**Design intent:** The Widow tests everything: dodge timing (leg sweep), positioning (webs), priority management (spiderlings), and sustained pressure (blood frenzy). The player needs every skill they've developed over 20 waves.

---

## Wave Composition Philosophy

Waves should introduce enemies that complement each other, not just add more bodies:

| Wave | Composition Goal | Key Tension |
|------|-----------------|-------------|
| 1-2 | Learn the basics | Dodge projectiles (Hunter), deal with speed (Small Bug, Rusty Robot) |
| 3-4 | Introduce combos | Medium Insect acid + Hunter shots = movement restriction. First boss tests fundamentals |
| 5-6 | Ranged variety | Spiny Beetle spread vs Hunter single-shot. Delivery Bot distracts. Tribe Warrior blocks |
| 7-8 | Area denial | Big Bug shockwaves + Circle Bot pulses claim space. Player learns positioning |
| 9-10 | Chaos management | Bomb Minion chain reactions. Guard Robot + Hooded Minion = tank-and-flank |
| 11-13 | Full toolkit | Guard/Warrior melee wall + Ranged Minion suppression + Relic Guardian charges |
| 14+ | Everything at once | Planter Bot reshapes arena while all other threats continue. Survival mode |
