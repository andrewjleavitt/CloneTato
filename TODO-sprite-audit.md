# Sprite Audit Checklist — Drift

Go through every character in the Sprite Gallery. For each one:
- Verify animations play correctly (no empty/garbage frames)
- Verify hitbox radius fits the sprite visually
- Verify pivot offset centers the sprite on the entity position
- Identify and wire up unique attack/special animations we're not using yet
- Note: `LoadStrip` auto-derives frame count from image width. Numbers in code are FPS, not frame counts.

---

## HEROES

### Gunslinger (Gun Hero)
**Source:** `Stranded 04 - Hero sprite/Without Sword (for gun)/`
**Frame size:** 64x65, horizontal strips

| Animation | File | Frames | FPS | Status |
|-----------|------|--------|-----|--------|
| idle_right | `R.png` | 12 | 8 | [x] Wired |
| idle_up | `Idle Up.png` | 12 | 8 | [x] Wired |
| idle_down | `Idle Down.png` | 12 | 8 | [x] Wired |
| run_right | `R Run.png` | 8 | 10 | [x] Wired |
| run_up | `Run UP.png` | 8 | 10 | [x] Wired |
| run_down | `Run Down.png` | 8 | 10 | [x] Wired |
| roll_right | `R Roll.png` | 6 | 12 | [x] Wired |
| roll_up | `Roll Up.png` | 8 | 12 | [x] Wired |
| roll_down | `Roll Down.png` | 8 | 12 | [x] Wired |
| move_right | `R move.png` | 8 | — | [ ] Not wired (walk vs run?) |
| move_up | `Move Up.png` | 8 | — | [ ] Not wired |
| move_down | `Move Down.png` | 8 | — | [ ] Not wired |
| death | `Death.png` | 13 | 10 | [x] Wired |

- [ ] **Verify:** Pivot offset (14.5, -8) aligns feet with entity position
- [ ] **Verify:** Hitbox radius 10 fits the character visually
- [ ] **Decision:** Wire up "move" strips as walk (slower speed) vs "run" for sprinting?
- [ ] **Gun sprites:** 6 separate gun PNGs in `Guns/` folder — could show equipped weapon visually

### Blade Dancer (Sword Hero)
**Source:** `Stranded 04 - Hero sprite/With Sword/`
**Frame size:** 64x65, horizontal strips

| Animation | Dir | File | Frames | FPS | Status |
|-----------|-----|------|--------|-----|--------|
| idle | right | `Idle left right.png` | 12 | 8 | [x] Wired |
| idle | up | `Idle Up.png` | 12 | 8 | [x] Wired |
| idle | down | `Idle Down.png` | 12 | 8 | [x] Wired |
| run | right | `R Run.png` | 8 | 10 | [x] Wired |
| run | up | `Run UP.png` | 8 | 10 | [x] Wired |
| run | down | `Run Down.png` | 8 | 10 | [x] Wired |
| roll | right | `R Roll.png` | 6 | 12 | [x] Wired |
| roll | up | `Roll Up.png` | 8 | 12 | [x] Wired |
| roll | down | `Roll Down.png` | 8 | 12 | [x] Wired |
| slash | right | `R Slash.png` | 5 | 12 | [x] Wired |
| slash | up | `attack slash up.png` | 7 | 12 | [x] Wired |
| slash | down | `Slash Down.png` | 7 | 12 | [x] Wired |
| chop | right | `R CHop.png` | 4 | — | [ ] Not wired |
| chop | up | `Chop Up.png` | 4 | — | [ ] Not wired |
| chop | down | `Chop Down.png` | 4 | — | [ ] Not wired |
| move | right | `R move.png` | 8 | — | [ ] Not wired |
| move | up | `Move Up.png` | 8 | — | [ ] Not wired |
| move | down | `Move Down.png` | 8 | — | [ ] Not wired |
| death | — | `Death.png` | 13 | 10 | [x] Wired |

- [ ] **Wire chop:** 4-frame overhead chop — single-target melee. Slash = wide arc, Chop = focused hit?
- [ ] **Wire move:** Walk animation separate from run
- [ ] **Verify:** Slash frame counts (5/7/7 across directions) — are some frames empty?

**Buff overlays** (64x65 area, 432x27 strips — 16 frames each at ~27px wide?):
- `Buff.png` — generic power-up glow
- `Defence.png` — shield effect
- `Flames.png` — fire damage overlay
- `Heal.png` — healing effect
- [ ] These are VFX overlays drawn on top of the hero — wire to active buffs/power-ups

**Grid sheet:** `Sci-fi hero 64x65.png` — combined grid with all animations. Can use strips instead.

### Drifter (Starter Pack Hero)
**Source:** `01 Stranded - Starter Pack v1/Hero/Hero/`
**Frame size:** 32x32, grid sheet 10 cols × 6 rows (320x192)
**Color variants:** Blue, Green, Pink, Yellow (currently using Blue)

| Row | Animation | Frames (est.) | Status |
|-----|-----------|---------------|--------|
| 0 | idle/walk down | ~7 | [x] Wired (idle + run) |
| 1 | idle/walk right | ~8 | [x] Wired (idle + run) |
| 2 | idle/walk up | ~8 | [x] Wired (idle + run) |
| 3 | hit/dodge | ~4 | [x] Wired (roll) |
| 4-5 | death (blue sparkle) | ~14 | [x] Wired |

- [ ] **Verify:** Frame counts per row — count visible frames, trim empties
- [ ] **Verify:** 32x32 grid alignment is correct (was previously broken as 20x32)
- [ ] **Verify:** Pivot offset (-4) and hitbox radius 8
- [ ] **Color variants:** Could offer as cosmetic skins (Green, Pink, Yellow)

### Companion
**Source:** `01 Stranded - Starter Pack v1/Hero/Companion/`
**Frame size:** 32x32, grid sheet 10 cols × 4 rows (320x128)
**Color variants:** Blue, Green, Pink, Yellow (matching hero)

| Row | Animation | Frames (est.) | Status |
|-----|-----------|---------------|--------|
| 0 | idle bob | ~4 | [x] Loaded |
| 1 | move | ~4 | [x] Loaded |
| 2 | gather/pulse | ~4 | [x] Loaded |
| 3 | attack/activate | ~10 | [x] Loaded |

- [ ] **Verify:** Frame counts per row
- [ ] **Not yet rendered in gameplay** — companion system not implemented yet
- [ ] Color should match hero variant

---

## ENEMIES — TRIBE

### Tribe Hunter — RANGED ATTACKER
**Source:** `enemies/tribe/Tribe Hunter/`
**Frame size:** 34x37, horizontal strips
**Behavior:** Kites at range, shoots projectiles at player

| Animation | Frames | FPS | Status |
|-----------|--------|-----|--------|
| idle (3 dirs) | 6 | 6 | [x] Wired |
| walk (3 dirs) | 8 | 8 | [x] Wired |
| shoot (3 dirs) | 7 | 12 | [x] Wired as attack_right/up/down |
| death | 10 | 10 | [x] Wired |

- [x] Shoot animations wired — ranged attack with projectile
- [x] Kiting behavior implemented (predictive aim, panic flight when close)

### Tribe Warrior — MELEE WITH WEAPON
**Source:** `enemies/tribe/Tribe Warrior/`
**Frame size:** 62x69, horizontal strips
**Behavior:** Chase player, big sweeping weapon swing when in range

| Animation | Frames | FPS | Status |
|-----------|--------|-----|--------|
| idle (3 dirs) | 6 | 6 | [x] Wired |
| walk (3 dirs) | 8 | 8 | [x] Wired |
| attack (3 dirs) | 11 | 12 | [x] Wired as attack_right/up/down |
| death | 10 | 10 | [x] Wired |

- [x] Attack animations wired — wide melee sweep with reach
- [x] Melee attack behavior implemented (chase, swing at range)

### Tamed Beast (Relic Guardian) — MELEE CHARGER
**Source:** `enemies/tribe/Tribe Tamed Beast/`
**Frame size:** 76x67, horizontal strips
**Behavior:** Slow approach, big charge/lunge attack. Enrages below 50% HP (+speed, +damage)

| Animation | Frames | FPS | Status |
|-----------|--------|-----|--------|
| idle (3 dirs) | 6 | 6 | [x] Wired |
| walk/move (3 dirs) | 6 | 6 | [x] Wired |
| attack (3 dirs) | 16 | 12 | [x] Wired as attack_right/up/down |
| death | 17 | 10 | [x] Wired |

- [x] Attack animations wired — long wind-up charge with dust cloud
- [x] Charge/melee behavior implemented
- [x] Enrage at 50% HP implemented (speed + damage buff)

---

## ENEMIES — INSECTS

### Small Bug — MELEE SWARM / BITE
**Source:** `enemies/insects/Small Bug/`
**Frame size:** 20x28, horizontal strips
**Behavior:** Fast swarm, quick bite on contact. Spawn in large numbers.

| Animation | Frames | FPS | Status |
|-----------|--------|-----|--------|
| idle/move | 6 | 6 | [x] Wired (reused all dirs) |
| attack | 6 | 12 | [x] Wired — lunging bite |
| death | 10 | 10 | [x] Wired |

- [x] Attack animation wired
- [x] Melee bite attack implemented

### Medium Insect — MELEE WITH ACID SPIT
**Source:** `enemies/insects/Medium Bug/`
**Frame size:** 34x37, horizontal strips
**Behavior:** Chase, close-range acid spit (short-range projectile?)

| Animation | Frames | FPS | Status |
|-----------|--------|-----|--------|
| idle/move | 6-8 | 6-8 | [x] Wired |
| attack | 10 | 12 | [x] Wired — lunge + green acid venom |
| death | 10 | 10 | [x] Wired |

- [x] Attack animation wired — later frames show green spit effect
- [x] Acid trail DOT pools implemented (drops ooze zones behind it)

### Big Bug — MELEE SLAM / SPAWNER
**Source:** `enemies/insects/Big Bug/`
**Frame size:** 72x44, horizontal strips
**Behavior:** Slow tank, slam attack when close. Acts as spawner for small bugs.

| Animation | Frames | FPS | Status |
|-----------|--------|-----|--------|
| idle/move | 8 | 8 | [x] Wired (reused all dirs) |
| attack | 6 | 12 | [x] Wired — rears up, slams with green slash |
| death | 13 | 10 | [x] Wired |

- [x] Attack animation wired
- [x] AOE pulse ground slam implemented (IsAOEPulse)
- [ ] **TODO:** Spawner behavior (periodically spawns small bugs)

### Spiny Beetle — RANGED SPIKE SHOOTER
**Source:** `enemies/insects/Medium bug 2/`
**Frame size:** 88x37, horizontal strips
**Behavior:** Erratic movement, fires green spike projectiles

| Animation | Frames | FPS | Status |
|-----------|--------|-----|--------|
| idle/move | 8 | 8 | [x] Wired (reused all dirs) |
| attack | 8 | 12 | [x] Wired — launches green spike projectile |
| death | 11 | 10 | [x] Wired |

- [x] Attack animation wired — clear projectile launch in sprite
- [x] Ranged spike shooter implemented (3 projectile spread)
- [x] Enhanced zigzag movement implemented

---

## ENEMIES — HUMANOIDS (Starter Pack)

### Archer — RANGED SHOOTER
**Source:** `enemies/starter_archer/archer.png`
**Frame size:** 32x32, grid 8 cols
**Behavior:** Kites at range, shoots arrows

| Animation | Start Frame | Count | Status |
|-----------|-------------|-------|--------|
| idle | 1 | 5 | [x] Wired |
| walk/run | 11 | 4 | [x] Wired |
| shoot | 16 | 7 | [x] Wired as "attack" (fixed offset) |
| hit | 33 | 2 | [ ] Not wired |
| death | 35 | 5 | [x] Wired |

- [x] Shoot animation wired (start frame fixed 19→16)
- [x] Kiting behavior implemented (predictive aim + wounded rapid-fire)
- [ ] Wire hit reaction animation

### Guard — MELEE TANK
**Source:** `enemies/starter_guard/guard.png`
**Frame size:** 32x32, grid 16 cols
**Behavior:** Slow chase, melee attack when close. Tanky.

| Animation | Start Frame | Count | Status |
|-----------|-------------|-------|--------|
| idle | 1 | 5 | [x] Wired |
| walk/run | 11 | 4 | [x] Wired |
| attack | 16 | 5 | [x] Wired as "attack" (fixed offset) |
| death | 29 | 16 | [x] Wired |

- [x] Attack animation wired (start frame fixed 19→16)
- [x] Shield bash with knockback implemented
- [x] Frontal damage reduction (50%) implemented

### Warrior — MELEE FIGHTER
**Source:** `enemies/starter_warrior/warrior.png`
**Frame size:** 32x32, grid 8 cols
**Behavior:** Chase, melee attack. Balanced stats.

| Animation | Start Frame | Count | Status |
|-----------|-------------|-------|--------|
| idle | 1 | 5 | [x] Wired |
| walk/run | 11 | 4 | [x] Wired |
| attack | 16 | 5 | [x] Wired as "attack" (fixed offset) |
| hit | 29 | 2 | [ ] Not wired |
| death | 33 | 8 | [x] Wired |

- [x] Attack animation wired (start frame fixed 19→16)
- [x] Rush/lunge attack implemented
- [ ] Wire hit reaction animation

---

## ENEMIES — ROBOTS (grid sheets)

### Rusty Robot — KAMIKAZE RUSHER
**Source:** `enemies/robots/Rusty Robot/Rusty Robot 20x29 without Shadow.png`
**Frame size:** 20x29, grid 8 cols × 2 rows (160x58)
**Color variants:** Default, Grey, Purple
**Behavior:** Fast rusher, contact damage only. No attack animation — just wobbles into you.

| Row | Frames | Animation | Status |
|-----|--------|-----------|--------|
| 0 | 8 | idle/wobble | [x] Wired |
| 1 | 8 | run/wobble variant | [x] Wired as walk |

- No attack or death animation in sheet
- [ ] **TODO:** Flash/explode on death (use generic VFX?)

### Guard Robot — TANK WITH SHIELD
**Source:** `enemies/robots/Guard Robot/Robot 1 - Blue 26x34 without shadows.png`
**Frame size:** 26x34, grid 10 cols × 5 rows (260x170)
**Color variants:** Blue, Red, Bronze, Green
**Behavior:** Slow tank. Projects shield aura to nearby robots.

| Row | Frames (est.) | Animation | Status |
|-----|---------------|-----------|--------|
| 0 | 1-3 | deploy/static | [ ] Not wired |
| 1 | 8 | idle | [x] Wired |
| 2 | 8 | walk | [x] Wired |
| 3 | ~10 | attack (arms extend?) | [ ] Not wired — needs gallery review |
| 4 | ~10 | death (breaks apart) | [x] Wired |

- [x] Row 3 attack animation wired
- [x] Frontal damage reduction (40%) implemented

### Circle Bot — ENERGY PULSE ATTACKER
**Source:** `enemies/robots/Circle Bot/Circle Bot blue 29x35 without shadow.png`
**Frame size:** 29x35, grid 8 cols × 4 rows (232x140)
**Color variants:** Blue, Green, Purple
**Behavior:** Erratic movement, energy pulse attack (blue glow visible in row 2)

| Row | Frames | Animation | Status |
|-----|--------|-----------|--------|
| 0 | 8 | idle (antenna wobble) | [x] Wired |
| 1 | 8 | walk | [x] Wired |
| 2 | 8 | attack (blue energy pulse) | [ ] Not wired — needs gallery review |
| 3 | 8 | death | [x] Wired |

- [x] Row 2 attack animation wired
- [x] AOE energy pulse implemented (IsAOEPulse)

### Delivery Bot — FAST RUNNER
**Source:** `enemies/robots/Delivery Bot/Delivery Bot yellow without shadow 23x21.png`
**Frame size:** 23x21, grid 6 cols × 5 rows (138x105)
**Color variants:** Yellow, Blue-Grey, Red
**Behavior:** Fastest enemy. Could drop mines or just kamikaze contact.

| Row | Frames | Animation | Status |
|-----|--------|-----------|--------|
| 0 | 6 | idle | [x] Wired |
| 1 | 6 | walk | [x] Wired |
| 2 | 6 | unknown — needs review | [ ] Not wired |
| 3 | 6 | unknown — needs review | [ ] Not wired |
| 4 | 6 | death? | [x] Wired (row 3 used) |

- [x] Row 2 attack animation wired
- [x] Loot enemy (flee behavior, no contact damage, big drops)

### Planter Bot — SUPPORT / HAZARD PLANTER (NEW — not in game)
**Source:** `enemies/robots/Planter Robot/Planter Bot Blue no shadow.png`
**Frame size:** 29x37, grid 24 cols × 3 rows (696x111)
**Color variants:** Blue, Red
**Behavior:** Plants hazards/turrets on the ground? Long row 2 animation suggests planting action.

| Row | Frames (est.) | Animation | Status |
|-----|---------------|-----------|--------|
| 0 | ~8 | idle | [ ] Not loaded |
| 1 | ~6 | walk | [ ] Not loaded |
| 2 | ~24 | plant/attack/death (long sequence) | [ ] Not loaded |

- [x] Loaded into game (DefIndex 17)
- [x] Attack + death animations wired
- [x] Mine-laying behavior implemented

---

## ENEMIES — MINIONS

### Hooded Minion (Minion 1) — MELEE SCYTHE
**Source:** `enemies/minions/Minion 1/Sprites Without Shadows/`
**Frame size:** 33x36, horizontal strips
**Behavior:** Chase, melee scythe slash. Revives once if not overkilled (per vision doc).

| Animation | File | Frames | FPS | Status |
|-----------|------|--------|-----|--------|
| idle | `Minion 1-idle.png` | 8 | 8 | [x] Wired |
| run | `Minion 1-Run.png` | 8 | 8 | [x] Wired |
| attack | `Minion 1-Attack.png` | 10 | 12 | [x] Wired — purple scythe arc |
| death | `Minion 1-Death.png` | 8 | 8 | [x] Wired |

- [x] Attack animation wired
- [x] Opportunist dash-strike implemented (3x rush cooldown reduction)

### Bomb Minion (Minion 2) — SUICIDE BOMBER
**Source:** `enemies/minions/Minion 2/Sprites Without Shadows/`
**Frame size:** 13x15 (body), 124x77 (explosion VFX)
**Behavior:** Rushes player, explodes on contact or death. AOE damages player AND other enemies.

| Animation | File | Frames | FPS | Status |
|-----------|------|--------|-----|--------|
| idle | `Minion 2-Idle.png` | 8 | 8 | [x] Wired |
| run | `Minion 2-Run.png` | 8 | 8 | [x] Wired |
| prep explode | `Minion 2-Prep Explode.png` | 4 | 8 | [x] Wired as death |
| explosion VFX | `Minion 2 Explosion - 124x71.png` | 15 | 12 | [ ] Not loaded |

- [ ] **TODO:** Load explosion VFX (1860x77, 15 frames at 124x77)
- [x] Explosion-on-death AOE implemented (damages nearby enemies, chain reactions)
- [x] Fuse proximity trigger implemented (arms within 100px of player)

### Ranged Minion (Minion 3) — RANGED SHOOTER
**Source:** `enemies/minions/Minion 3/Sprites without Shadow/`
**Frame size:** 25x15, horizontal strips
**Behavior:** Stay at range, shoot rapidly. Squishy (low HP).

| Animation | File | Frames | FPS | Status |
|-----------|------|--------|-----|--------|
| idle | `Minion 3-Idle.png` | 8 | 8 | [x] Wired |
| run | `Minion 3-Run.png` | 8 | 8 | [x] Wired |
| range attack | `Minion 3-Range Attack.png` | 15 | 12 | [x] Wired as "attack" |
| death | `Minion 3-Death.png` | 7 | 7 | [x] Wired |

- [x] Range attack animation wired
- [x] Ranged behavior implemented (3-projectile spread)

---

## BOSSES — IMPLEMENTED

### Dust Warrior
**Source:** `bosses/dust_warrior/`
**Frame size:** 67x45, horizontal strips

| Animation | File | Frames | FPS | Status |
|-----------|------|--------|-----|--------|
| idle | `Warrior-Idle.png` | auto | 8 | [x] Wired |
| walk | `Warrior-Run .png` | auto | 8 | [x] Wired |
| attack | `Warrior-Attack.png` | auto | 10 | [x] Wired |
| death | `Warrior-Death.png` | auto | 8 | [x] Wired |

- [ ] Verify pivot offset Y=-6, hitbox

### Blowfish
**Source:** `enemies/blowfish/` (used as boss)
**Frame size:** 94x47, horizontal strips

| Animation | File | Frames | Status |
|-----------|------|--------|--------|
| idle | `Blowfish-Big Idle.png` | auto | [x] Wired |
| walk right | `move Left & Right.png` | 12 | [x] Wired |
| walk up | `Move Up.png` | 18 | [x] Wired |
| walk down | `Move Down.png` | 18 | [x] Wired |
| attack | `Attack out of Ground Down.png` | 13 | [x] Wired |
| death | `Blowfish-Death.png` | 14 | [x] Wired |
| small idle | `Blowfish-Small Idle.png` | 12 | [ ] Not wired |
| small move | `Blowfish-Small Move.png` | ? | [ ] Not wired |
| go big | `Blowfish-Grow into Big Form.png` | 5 | [ ] Not wired |
| go small | `Blowfish-Small shrink.png` | 4 | [ ] Not wired |
| into ground | `Blowfish-Move into ground.png` | 11 | [ ] Not wired |
| attack up | `Attack out of Ground Up.png` | 13 | [ ] Not wired |
| spike 1/2/3 | `Spike*.png` | various | [ ] Not wired |
| spike trail | `Spike trail.png` | ? | [ ] Not wired |

- [ ] **Phase boss potential:** small form → grow → big form → burrow → emerge attack
- [ ] Spike sprites could be projectiles

### Tarnished Widow
**Source:** `bosses/tarnished_widow/The Tarnished Widow 188x90.png`
**Frame size:** 188x90, grid 18 cols × 8 rows

| Row | Start | Frames (est.) | Animation (est.) | Status |
|-----|-------|---------------|------------------|--------|
| 0 | 0 | 8 | idle | [x] Wired |
| 1 | 18 | 10 | walk | [x] Wired |
| 2 | 36 | 18? | unknown — dissolve/alternate? | [ ] Not wired |
| 3 | 54 | 8? | unknown — idle variant? | [ ] Not wired |
| 4 | 72 | 10 | attack (with blood) | [x] Wired |
| 5 | 90 | 8? | unknown — more blood attack? | [ ] Not wired |
| 6 | 108 | 12? | unknown — charge/lunge? | [ ] Not wired |
| 7 | 126 | 18 | death explosion | [x] Wired |

- [ ] **NEEDS REVIEW:** Rows 2, 3, 5, 6 have unknown animations
- [ ] Multi-phase boss fight potential with the full 8-row sheet
- [ ] Verify pivot offset Y=-10

---

## BOSSES — NOT YET IMPLEMENTED

### Dust Jumper
**Source:** `06 Stranded - Dust Jumper/Dust Jumper/`
**Frame size:** 42x91, grid 14 cols × 9 rows (588x819)

| Row | Frames (est.) | Animation (est.) |
|-----|---------------|------------------|
| 0 | ~12 | idle/walk (large form, facing right) |
| 1 | ~6 | attack/transition |
| 2 | ~7 | dying/shrinking + debris |
| 3 | ~2 | tiny form / death |
| 4 | ~10 | emerging from ground (puff effect) |
| 5 | ~7 | idle/walk variant |
| 6 | ~12 | running/action |
| 7 | ~10 | attack/idle variant |
| 8 | ~8 | animation + death VFX |

**Additional sprites:**
- `Slam VFX 98x103.png` — 980px wide = 10 frames. Ground slam AOE effect.
- `Indicator Sprite Sheet 21x10.png` — 189px wide = 9 frames. Landing indicator.

- [ ] **Full row-by-row verification needed** — 9 rows of animation
- [ ] Slam VFX + indicator for ground-pound ability

### Speed Racer
**Source:** `STRANDED - Speed Racer/`
**Frame size:** 45x48, individual strip files per direction

| Animation | Directions | Frames/dir | Files |
|-----------|-----------|------------|-------|
| idle | 8 (N/NE/E/SE/S/SW/W/NW) | 1 | `Racer 1-Idle-{dir}.png` |
| run | 8 (N/NE/E/SE/S/SW/W/NW) | 4 | `Racer 1-{dir}.png` |

- [ ] **No death animation** in the pack — need to create one (flash/fade?) or use explosion VFX
- [ ] Full 8-directional coverage is unique — only sprite with diagonal variants
- [ ] Very small (45x48) — may need upscaling as a boss

### Armored Mech
**Source:** `STRANDED - Armored Mech/`
**Frame size:** 148x109, grid 18 cols × 20 rows (2664x2180)
**Color variants:** Blue, Blue with markings, Copper, Green, Purple

| Row | Frames (est.) | Animation (est.) |
|-----|---------------|------------------|
| 0 | 3 | idle (few frames) |
| 1 | 8 | idle/walk |
| 2 | 10 | attack (energy blade + muzzle flash) |
| 3 | 10 | attack follow-through |
| 4 | 4 | idle variant |
| 5 | 4 | walk variant |
| 6 | 8 | walk |
| 7 | 10 | attack 2 (energy blade + muzzle flash) |
| 8 | 10 | attack 2 follow-through |
| 9 | 10 | idle/transition |
| 10 | 4 | stance change |
| 11 | 4 | stance |
| 12 | 4 | stance |
| 13 | 8 | walk/charge |
| 14 | 10 | attack 3 (energy blade) |
| 15 | 10 | attack 3 follow-through |
| 16 | 4 | hit/flinch |
| 17 | 4 | shield? |
| 18 | 7 | walk/shield |
| 19 | 18+ | death (explosion + dissolve, extends across row) |

- [ ] **Massive sheet — needs detailed row-by-row review in gallery**
- [ ] 20 rows × 18 cols = 360 possible frames
- [ ] Multiple attack variants with energy blade + muzzle flash
- [ ] Possible shield mechanics (rows 10-12, 17)
- [ ] 5 color variants for difficulty tiers or boss phases

### Electrocutioner
**Source:** `STRANDED - Electrocutioner/No shadow/`
**Frame size:** 157x107, horizontal strips

| Animation | File | Frames | Status |
|-----------|------|--------|--------|
| idle | `Electricuitioner-Idle.png` | 12 | [ ] Not yet loaded |
| attack | `Electricuitioner-Attack.png` | 39 | [ ] Not yet loaded |
| death | `Electricuitioner-Death.png` | 16 | [ ] Not yet loaded |
| appear | `Electricuitioner-Appear.png` | 6 | [ ] Not yet loaded |
| range attack | `Electricuitioner-Range Attack.png` | 16 | [ ] Not yet loaded |
| teleport | `Electricuitioner-Teleport.png` | 12 | [ ] Not yet loaded |
| full sheet | `Electricuitioner 157x107.png` | 102 | [ ] Not yet loaded |

**Additional sprites:**
- `Volley Projectile 157x122.png` — 2826px wide = 18 frames. Projectile effect.

- [ ] Richest animation set of any boss — 6 distinct animations
- [ ] 39-frame attack is the longest single animation in the game
- [ ] Appear + Teleport enable phase-based vanishing boss
- [ ] "With Shadow" variants also available (same animations, drop shadow baked in)

---

## SUMMARY

### Fully wired (idle/walk/death): 20 sprites
- 3 heroes (Gunslinger, Blade Dancer, Drifter) + Companion
- 17 enemies (all factions)
- 3 bosses (Dust Warrior, Blowfish, Tarnished Widow)

### Attack animations wired: ALL enemies + heroes
All 17 enemy types + 3 bosses now have attack animations wired.
Humanoid grid sheet attack frames fixed (startFrame 19→16).

### Robot grid sheets: VERIFIED
- Rusty Robot (no attack — kamikaze), Guard Robot ✓, Circle Bot ✓, Delivery Bot ✓, Planter Bot ✓

### Still not wired (low priority):
- Blade Dancer chop (separate from slash — could be alt melee)
- Hero "move" walk animations (separate from run)
- Bomb Minion explosion VFX (124x71 sprite, 15 frames)
- Blowfish small form + burrow animations (12 unused)
- Tarnished Widow rows 2, 3, 5, 6 (unknown animations)
- Archer/Warrior hit reaction animations

### Bosses not yet implemented: 4
- Dust Jumper (9 rows + slam VFX)
- Speed Racer (8-dir idle/run, no death)
- Armored Mech (20 rows, 5 colors)
- Electrocutioner (6 animations + volley projectile)

### Phase boss potential:
- Blowfish: small→grow→big→burrow→emerge (12 unused animations)
- Tarnished Widow: 4 unknown animation rows
- Electrocutioner: appear/teleport/attack/range (natural phase transitions)
- Armored Mech: shield/stance/multiple attack types
