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

### Tribe Hunter
**Source:** `enemies/tribe/Tribe Hunter/`
**Frame size:** 34x37, horizontal strips

| Animation | Frames | FPS | Status |
|-----------|--------|-----|--------|
| idle (3 dirs) | 6 | 6 | [x] Wired |
| walk (3 dirs) | 8 | 8 | [x] Wired |
| death | 10 | 10 | [x] Wired |
| shoot right | ? | — | [ ] Not wired (`Tribe Hunter-Shoot.png`) |
| shoot up | ? | — | [ ] Not wired (`Tribe Hunter-Shoot Up.png`) |
| shoot down | ? | — | [ ] Not wired (`Tribe Hunter-Shoot Down.png`) |

- [ ] Wire shoot animations for ranged attack behavior
- [ ] Verify hitbox radius 10, pivot offset Y=-4

### Tribe Warrior
**Source:** `enemies/tribe/Tribe Warrior/`
**Frame size:** 62x69, horizontal strips

| Animation | Frames | FPS | Status |
|-----------|--------|-----|--------|
| idle (3 dirs) | 6 | 6 | [x] Wired |
| walk (3 dirs) | 8 | 8 | [x] Wired |
| death | 10 | 10 | [x] Wired |
| attack right | ? | — | [ ] Not wired (`Tribe Warrior-attack.png`) |
| attack up | ? | — | [ ] Not wired (`Tribe Warrior-Attack Up.png`) |
| attack down | ? | — | [ ] Not wired (`Tribe Warrior-attack down.png`) |

- [ ] Wire attack animations for melee behavior
- [ ] Verify hitbox radius 12, pivot offset Y=-10

### Tamed Beast (Relic Guardian)
**Source:** `enemies/tribe/Tribe Tamed Beast/`
**Frame size:** 76x67, horizontal strips

| Animation | Frames | FPS | Status |
|-----------|--------|-----|--------|
| idle (3 dirs) | 6 | 6 | [x] Wired |
| walk/move (3 dirs) | 6 | 6 | [x] Wired |
| death | 17 | — | [x] Wired |
| attack LR | 16 | — | [ ] Not wired |
| attack up | 16 | — | [ ] Not wired |
| attack down | 16 | — | [ ] Not wired |

- [ ] Wire attack animations (16 frames each — charge/bite?)
- [ ] Verify hitbox radius 14, pivot offset Y=-10

---

## ENEMIES — INSECTS

### Small Bug
**Source:** `enemies/insects/Small Bug/`
**Frame size:** 20x28, horizontal strips

| Animation | Frames | FPS | Status |
|-----------|--------|-----|--------|
| idle/move | 6 | 6 | [x] Wired (reused for all dirs) |
| death | 10 | 10 | [x] Wired |
| attack | 6 | — | [ ] Not wired (`Small insect-Attack.png`) |

- [ ] Verify hitbox radius 8, pivot offset Y=-3

### Medium Insect
**Source:** `enemies/insects/Medium Bug/`
**Frame size:** 34x37, horizontal strips

| Animation | Frames | FPS | Status |
|-----------|--------|-----|--------|
| idle/move | 6-8 | 6-8 | [x] Wired |
| death | 10 | 10 | [x] Wired |
| attack | ? | — | [ ] Not wired (`Medium Insect-Attack.png`) |

- [ ] Verify hitbox radius 9, pivot offset Y=-4

### Big Bug
**Source:** `enemies/insects/Big Bug/`
**Frame size:** 72x44, horizontal strips

| Animation | Frames | FPS | Status |
|-----------|--------|-----|--------|
| idle/move | auto | 8 | [x] Wired |
| death | auto | 13 | [x] Wired |
| attack | 6 (72x44) | — | [ ] Not wired (`Big Insect-Attack.png`) |

- [ ] Verify hitbox radius 14, pivot offset Y=-4

### Spiny Beetle (Medium Bug 2)
**Source:** `enemies/insects/Medium bug 2/`
**Frame size:** 88x37, horizontal strips

| Animation | Frames | FPS | Status |
|-----------|--------|-----|--------|
| idle/move | auto | 8 | [x] Wired |
| death | auto | 11 | [x] Wired |
| attack | 8 (88x37) | — | [ ] Not wired (`Medium2 bug-Attack.png`) |

- [ ] Verify hitbox radius 11, pivot offset Y=-4

---

## ENEMIES — HUMANOIDS (Starter Pack)

### Archer
**Source:** `enemies/starter_archer/archer.png`
**Frame size:** 32x32, grid 8 cols

| Animation | Start Frame | Count | Status |
|-----------|-------------|-------|--------|
| idle | 1 | 5 | [x] Wired |
| walk/run | 11 | 4 | [x] Wired |
| shoot | 19 | 7 | [ ] Not wired |
| hit | 33 | 2 | [ ] Not wired |
| death | 35 | 5 | [x] Wired |

- [ ] **Verify** grid layout — are frame start indices correct?
- [ ] Wire shoot animation for ranged behavior
- [ ] Verify hitbox radius 10, pivot offset Y=-4

### Guard
**Source:** `enemies/starter_guard/guard.png`
**Frame size:** 32x32, grid 16 cols

| Animation | Start Frame | Count | Status |
|-----------|-------------|-------|--------|
| idle | 1 | 5 | [x] Wired |
| walk/run | 11 | 4 | [x] Wired |
| attack | 19 | 5 | [ ] Not wired |
| death | 29 | 16 | [x] Wired |

- [ ] **Verify** grid layout with 16 cols — wider sheet, different row structure
- [ ] Wire attack animation for melee behavior
- [ ] Verify hitbox radius 12, pivot offset Y=-3

### Warrior
**Source:** `enemies/starter_warrior/warrior.png`
**Frame size:** 32x32, grid 8 cols

| Animation | Start Frame | Count | Status |
|-----------|-------------|-------|--------|
| idle | 1 | 5 | [x] Wired |
| walk/run | 11 | 4 | [x] Wired |
| attack | 19 | 5 | [ ] Not wired |
| hit | 29 | 2 | [ ] Not wired |
| death | 33 | 8 | [x] Wired |

- [ ] **Verify** grid layout
- [ ] Wire attack animation for melee behavior
- [ ] Verify hitbox radius 11, pivot offset Y=-4

---

## ENEMIES — ROBOTS (grid sheets — frame ranges ESTIMATED)

### Rusty Robot
**Source:** `enemies/robots/Rusty Robot/Rusty Robot 20x29 without Shadow.png`
**Frame size:** 20x29, grid (est. 8 cols)

| Animation | Start Frame | Count (est.) | Status |
|-----------|-------------|--------------|--------|
| idle | 0 | 8 | [x] Wired |
| walk | 8 | 8 | [x] Wired |
| death | 8 | 8 | [x] Wired (reuses walk — no real death anim?) |

- [ ] **NEEDS REVIEW:** Verify actual row/col layout. No confirmed death animation.
- [ ] Verify hitbox radius 8, pivot offset Y=-3

### Guard Robot
**Source:** `enemies/robots/Guard Robot/Robot 1 - Blue 26x34 without shadows.png`
**Frame size:** 26x34, grid (est. 10 cols)

| Animation | Start Frame | Count (est.) | Status |
|-----------|-------------|--------------|--------|
| idle | 10 | 8 | [x] Wired |
| walk | 20 | 8 | [x] Wired |
| death | 40 | 10 | [x] Wired |

- [ ] **NEEDS REVIEW:** 5 rows estimated. Row 0 may be 1-3 frames. Rows 3-4 unknown.
- [ ] Check for shield/attack animations in unused rows
- [ ] Verify hitbox radius 11, pivot offset Y=-4

### Circle Bot
**Source:** `enemies/robots/Circle Bot/Circle Bot blue 29x35 without shadow.png`
**Frame size:** 29x35, grid (est. 8 cols)

| Animation | Start Frame | Count (est.) | Status |
|-----------|-------------|--------------|--------|
| idle | 0 | 8 | [x] Wired |
| walk | 8 | 8 | [x] Wired |
| death | 24 | 8 | [x] Wired |

- [ ] **NEEDS REVIEW:** Row 2 (frames 16-23) may be attack animation — not wired
- [ ] Verify hitbox radius 10, pivot offset Y=-4

### Delivery Bot
**Source:** `enemies/robots/Delivery Bot/Delivery Bot yellow without shadow 23x21.png`
**Frame size:** 23x21, grid (est. 6 cols)

| Animation | Start Frame | Count (est.) | Status |
|-----------|-------------|--------------|--------|
| idle | 0 | 6 | [x] Wired |
| walk | 6 | 6 | [x] Wired |
| death | 18 | 6 | [x] Wired |

- [ ] **NEEDS REVIEW:** Rows 2-3 (frames 12-17) unknown — may have empty frames
- [ ] Verify hitbox radius 7, pivot offset Y=-2

---

## ENEMIES — MINIONS

### Hooded Minion (Minion 1)
**Source:** `enemies/minions/Minion 1/Sprites Without Shadows/`
**Frame size:** 33x36, horizontal strips

| Animation | File | FPS | Status |
|-----------|------|-----|--------|
| idle | `Minion 1-idle.png` | 8 | [x] Wired |
| walk/run | `Minion 1-Run.png` | 8 | [x] Wired |
| death | `Minion 1-Death.png` | 8 | [x] Wired |
| attack | `Minion 1-Attack.png` (10f, 33x36) | — | [ ] Not wired |

- [ ] Wire attack animation
- [ ] Verify hitbox radius 10, pivot offset Y=-4

### Bomb Minion (Minion 2)
**Source:** `enemies/minions/Minion 2/Sprites Without Shadows/`
**Frame size:** 13x15, horizontal strips

| Animation | File | FPS | Status |
|-----------|------|-----|--------|
| idle | `Minion 2-Idle.png` | 8 | [x] Wired |
| walk/run | `Minion 2-Run.png` | 8 | [x] Wired |
| death (prep explode) | `Minion 2-Prep Explode.png` (4f) | 4 | [x] Wired |
| explosion VFX | `Explosion 124x71.png` (15f) | — | [ ] Not wired |

- [ ] Wire explosion VFX as on-death AOE visual
- [ ] Verify hitbox radius 5, pivot offset Y=-1

### Ranged Minion (Minion 3)
**Source:** `enemies/minions/Minion 3/Sprites without Shadow/`
**Frame size:** 25x15, horizontal strips

| Animation | File | FPS | Status |
|-----------|------|-----|--------|
| idle | `Minion 3-Idle.png` | 8 | [x] Wired |
| walk/run | `Minion 3-Run.png` | 8 | [x] Wired |
| death | `Minion 3-Death.png` (7f) | 7 | [x] Wired |
| range attack | `Minion 3-Range Attack.png` (15f, 25x15) | — | [ ] Not wired |

- [ ] Wire range attack animation
- [ ] Verify hitbox radius 8, pivot offset Y=-1

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

### Attack animations NOT wired: 14 enemies + 1 hero
- Tribe Hunter (shoot), Tribe Warrior (attack), Tamed Beast (attack)
- Small Bug, Medium Insect, Big Bug, Spiny Beetle (attack)
- Archer (shoot), Guard (attack), Warrior (attack)
- Hooded Minion (attack), Ranged Minion (range attack)
- Bomb Minion (explosion VFX)
- Blade Dancer (chop)

### Robot grid sheets needing verification: 4
- Rusty Robot, Guard Robot, Circle Bot, Delivery Bot

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
