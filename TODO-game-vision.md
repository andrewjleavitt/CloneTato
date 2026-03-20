# CloneTato — Game Vision & Feature Roadmap

## Identity Statement
CloneTato is a **twin-stick desert survivor** where the environment is as dangerous as the enemies. Skilled aiming, terrain tactics, and meaningful choices between waves make every run feel distinct. The Blood Desert isn't a backdrop — it's a character.

---

## Feature Pillars

### 1. THE DESERT IS ALIVE
The environment is a core mechanic, not just scenery.

- [ ] **Heat system** — exposure builds heat over time; shade from obstacles/terrain cools you down. Overheating debuffs speed/damage. Oases provide relief but attract enemies.
- [ ] **Sandstorms** — periodic events that reduce visibility, change enemy behavior (some flee, some get aggressive), and shift terrain zones. Player must adapt mid-wave.
- [ ] **Time of day** — palette shifts across waves (dawn→noon→dusk→night). Night waves have reduced visibility but better loot. Noon waves have max heat pressure.
- [ ] **Destructible terrain** — obstacles can be broken by explosions/bosses, changing the arena layout mid-wave. Cover is temporary.
- [ ] **Elevation** — high ground tiles give range bonus. Low ground (dry riverbeds) gives speed but you're exposed. Positioning matters.
- [ ] **Enemy nests** — destructible spawners that appear in waves. Kill them early to reduce pressure, or ignore them and deal with the flood.

### 2. BOSSES ARE EVENTS
Boss encounters change the rules, not just the numbers.

- [ ] **Tarnished Widow** — burrows underground, creates sinkholes (terrain hazards), emerges for attack phases. Multi-phase fight using her full sprite set (small form → big form → burrow → spike attacks).
- [ ] **Blowfish** — poisons terrain zones on contact, grows from small to large form mid-fight, launches spike projectiles. Arena becomes increasingly toxic.
- [ ] **Dust Warrior** — summons sand walls that funnel the player, has melee charge + ranged sand blast. Creates chokepoints you have to fight through.
- [ ] **Boss arenas** — bosses reshape the playing field. After a boss dies, the terrain changes persist into the next wave as a reward/challenge.

### 3. SKILL EXPRESSION THROUGH AIMING
Twin-stick aiming is the core differentiator from Brotato. Reward precision.

- [ ] **Piercing shots** — weapons that reward lining up enemies in a row.
- [ ] **Ricochet weapons** — bullets bounce off obstacles. Smart positioning = more damage.
- [ ] **Aimed AOE** — grenades/mines that punish bad placement and reward good reads.
- [ ] **Weak points** — some enemies (robots, bosses) have directional weak spots. Flanking matters.
- [ ] **Combo system** — rapid kills build a multiplier that boosts XP/gold. Skilled play is rewarded economically.

### 4. MEANINGFUL BETWEEN-WAVE CHOICES
Every decision between waves should create tension and run identity.

- [ ] **Wave mutators** — before each wave, choose between risk/reward modifiers:
  - "Double enemies, double gold"
  - "Boss bounty: bonus loot but boss is enraged"
  - "Sandstorm incoming: reduced visibility but rare drops"
  - "Oasis wave: healing zones appear but enemies are drawn to them"
- [ ] **Faction bounties** — bonus gold for killing a specific faction's leader unit.
- [ ] **Sacrifice mechanics** — trade HP for gold, trade a weapon slot for a powerful passive, etc.
- [ ] **Shop evolution** — shop offerings improve based on wave count and player choices. Early choices shape late-game options.

### 5. ENEMY FACTION IDENTITY
Factions aren't just visual categories — they have mechanical identity.

- [ ] **Tribe** — buff each other when nearby (pack tactics). Killing the leader demoralizes survivors (speed debuff). Tribe Hunter is the leader unit.
- [ ] **Insects** — ignore armor, swarm behavior, attack in clusters. Big Bug acts as a spawner for smaller bugs.
- [ ] **Robots** — have shields that regenerate. Weak to melee/explosive. Guard Robot projects a shield aura to nearby robots.
- [ ] **Minions** — swarm units that get stronger when a boss is alive. Bomb Minion explodes on death (AOE damage to player AND other enemies). Hooded Minion revives once if not killed with enough overkill damage.
- [ ] **Beasts** — Relic Guardians are slow but enrage (speed+damage buff) when below 50% HP. Charge attack that destroys obstacles in their path.
- [ ] **Faction waves** — some waves are faction-themed (all robots, all insects). These have unique mechanics and better rewards.

### 6. RUN IDENTITY & CHARACTER BUILDS
Every run should feel different from the last.

- [ ] **Starting archetypes** (character select):
  - Gunslinger — starts with pistol, bonus to bullet weapons, fast reload
  - Scavenger — starts with melee, bonus pickup range, finds more gold
  - Drone Commander — starts with an orbiting drone, bonus to summon/turret weapons
  - Glass Cannon — 50% HP, 50% more damage, faster but fragile
  - Nomad — bonus move speed, heat resistant, dash has longer range
- [ ] **Weapon synergies** — certain weapon combinations unlock passive bonuses (dual pistols = faster fire, shotgun + melee = close-range damage aura).
- [ ] **Relic system** — rare persistent items found in runs that carry over to future runs (meta-progression beyond stat upgrades). "The Widow's Fang" — crit kills spawn a sinkhole. "Desert Glass" — heat buildup increases fire damage.

---

## Priority Order
1. **Sprite audit** (get all animations correct — foundation for everything)
2. **Terrain polish** (Blood Desert floor, zones, scatter — the setting needs to look right)
3. **Enemy faction behaviors** (give each faction mechanical identity)
4. **Boss phase fights** (Tarnished Widow first — she has the best sprite set for it)
5. **Wave mutators** (low-code, high-impact on replayability)
6. **Heat/sandstorm system** (environment as threat)
7. **Starting archetypes** (run identity)
8. **Skill expression weapons** (piercing, ricochet, weak points)
9. **Relic system** (long-term meta progression)

---

## Design Principles
- **Depth over complexity** — each system should be simple to understand but create emergent interactions.
- **The desert is the third faction** — environment should threaten both player and enemies.
- **Reward skill, not just time** — twin-stick aiming means the player's skill ceiling is high. Honor that.
- **Every run tells a story** — choices, faction encounters, and terrain should make each run memorable.
- **Show don't tell** — visual feedback over UI text. Sandstorms you can see coming, heat shimmer on the screen, faction leaders that look distinct.
