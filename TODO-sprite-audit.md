# Sprite Audit Checklist

Go through every character in the Sprite Gallery. For each one:
- Verify animations play correctly (no empty/garbage frames)
- Verify hitbox radius fits the sprite visually
- Verify pivot offset centers the sprite on the entity position
- Identify and wire up unique attack/special animations we're not using yet

## HERO
- [ ] Hero (Gun) — idle, run, roll, death all 3 directions

## TRIBE
- [ ] Tribe Hunter — has shoot animations (Shoot, Shoot Up, Shoot Down) not wired up
- [ ] Tribe Warrior — has attack animations (attack, Attack Up, attack down) not wired up

## INSECTS
- [ ] Small Bug — has Attack strip (6f) not wired up
- [ ] Medium Insect — has Attack strip not wired up
- [ ] Big Bug — has Attack strip (6f, 72x44) not wired up; verify idle 8f, death 13f
- [ ] Spiny Beetle (Medium Bug 2) — has Attack strip (8f, 88x37) not wired up; verify idle 8f, death 11f

## BEASTS
- [ ] Relic Guardian (Tribe Tamed Beast) — has Attack LR/Up/Down strips (16f each, 76x67) not wired up; verify idle/walk 6f, death 17f

## HUMANOIDS
- [ ] Archer — verify grid layout (32x32, 8 cols); has shoot frames?
- [ ] Guard — verify grid layout (32x32, 16 cols); has attack frames (19-23) not wired up
- [ ] Warrior — verify grid layout (32x32, 8 cols); has attack frames (19-23) not wired up

## ROBOTS (grid sheets — frame ranges are ESTIMATED, likely need fixing)
- [ ] Rusty Robot — 20x29, 8 cols, 2 rows. Currently: idle row1, walk row2, death=walk (no real death). Check for empty frames
- [ ] Guard Robot — 26x34, 10 cols, 5 rows. Currently: idle start 10, walk start 20, death start 40. Row 1 has 1 frame — verify all row boundaries
- [ ] Circle Bot — 29x35, 8 cols, 4 rows. Currently: idle row1, walk row2, death row4. Verify row 3 (attack?) not wired up
- [ ] Delivery Bot — 23x21, 6 cols, 5 rows. Currently: idle row1, walk row2, death row4. Verify — later rows may have empty frames

## MINIONS
- [ ] Hooded Minion (Minion 1) — has Attack strip (10f, 33x36) not wired up; verify idle/walk/death 8f
- [ ] Bomb Minion (Minion 2) — has Prep Explode as death (4f); also has Explosion effect (124x71, 15f) not wired up — could be on-death AOE visual
- [ ] Ranged Minion (Minion 3) — has Range Attack strip (15f, 25x15) not wired up; verify idle/walk 8f, death 7f

## BOSSES
- [ ] Dust Warrior — verify idle/walk/attack/death
- [ ] Blowfish — verify idle/walk/attack/death. Has many unused animations:
  - Small Idle/Move (12f) — small form
  - Go Big (5f) / Go Small (4f) — transformation
  - Into Ground (11f) — burrow
  - Attack Out of Ground Up/Down (13f each) — burrow attack
  - Spike 1/2/3, Spike Trail — projectile sprites
  - Could support phase-based boss fight (small→big, burrow attacks)
- [ ] Tarnished Widow — grid sheet 188x90, 18 cols, 8 rows. Verify frame ranges:
  - Row 1 (0-7): idle 8f
  - Row 2 (18-27): walk 10f
  - Row 3 (36-53): unknown — dissolve? alternate death?
  - Row 4 (54-61): unknown — idle variant?
  - Row 5 (72-81): attack with blood 10f
  - Row 6 (90-97): more blood attack?
  - Row 7 (108-119): unknown — charge/lunge?
  - Row 8 (126-143): death explosion 18f
  - Several rows have unknown animations — need to review in gallery

## Notes
- Robot grid sheets need the most work — frame ranges were guessed from visual inspection
- Many enemies have attack animations we should wire into gameplay (melee swings, ranged shots, etc.)
- Blowfish has the richest animation set — potential for a multi-phase boss fight
- Bomb Minion explosion effect could be a visual AOE on death
