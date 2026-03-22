# Dead Meridian — Game Design Document

*Working document. Everything here is open to revision.*

## Vision

A top-down turn-based RPG set on a lawless frontier planet at the edge of settled space. Tone sits between Firefly's scrappy warmth, Andor's grounded tension, and The Mandalorian's lone-gunslinger mystique. The player is a bounty hunter — arrived to collect a target — who gets pulled into the web of a world that's more complicated than it looks.

Pixel art. Tactical combat. Characters you remember. Secrets worth finding.

A completely different pace from Drift. Where Drift is adrenaline and reflexes, Dead Meridian is choices and consequences. You think, then you act.

## Pillars

1. **Story first** — Every quest, every NPC, every area should serve the narrative. Combat is a verb in the story, not the story itself.
2. **Earned mystery** — The world has deep secrets, but they're revealed through exploration and relationships, not exposition dumps. Players should feel like detectives, not tourists.
3. **Space cowboy fantasy** — You're resourceful, outnumbered, and operating in morally gray territory. The power fantasy is cleverness and grit, not overwhelming force.
4. **Small world, dense world** — Better to have 5 areas that feel alive than 20 that feel empty.

---

## Setting

### The Planet

A remote desert world on the fringe of civilized space. Officially: a decommissioned mining colony, abandoned when the ore dried up. The landscape is harsh — cracked desert, dust storms, and ancient rock formations — but not dead. Life has adapted. People have adapted.

The official story is a lie.

Dead Meridian was never a mining colony. A military contractor — **[NAME TBD]** — used the planet as a black site. The ruins of an ancient race were discovered here, and the corp came to study (or weaponize) what they found. The mining operation was a cover story. The settlers were window dressing — warm bodies to make the colony look civilian on paper.

The planet has layers:
- **Surface**: What everyone sees and believes. Frontier towns, tribal territories, merchant routes, wildlife. A forgotten mining world.
- **Underneath**: Ancient ruins that predate human presence by millennia. Technology that doesn't match anything known. The corp didn't stumble on these — **the ruins are why they came.**
- **The Secret**: What the corp found in the deepest ruins, what they did with it, and why they "left" (did they actually leave, or just go deeper?). This is the central mystery.

### Factions

| Faction | Role | Analog |
|---------|------|--------|
| **Settlers** | Frontier townspeople. Farmers, mechanics, bartenders. Just trying to get by. | Firefly's border moon settlers |
| **The Tribes** | Indigenous groups who were here before the corp. Watched the corp dig in the wrong places and said nothing. Know far more about the ruins than they let on. | Tusken Raiders meets Fremen |
| **Remnant Corp** | Skeleton crew of military contractor personnel still running ops. The black site was never fully decommissioned. They're still on mission. | The Empire's quiet reach |
| **Scavengers** | Raiders and opportunists picking over what the corps left behind. | Jawas meets Reavers (less horrifying) |
| **The Order** | Scholars/monks studying the ruins. Neutral but secretive. May not be what they seem. Are they independent — or the corp's research division with the logos scraped off? | Jedi archives crossed with a frontier monastery |

### Key Locations (mapped to biomes)

| Location | Biome Asset | Description |
|----------|-------------|-------------|
| **Dustport** | Town Builder | The main settlement. Hub town with merchants, jobs, and rumors. |
| **The Barrens** | Blood Desert | Open desert. Hostile fauna, scavenger camps, surface ruins. |
| **Overgrown Vaults** | Temple | Ancient structures being reclaimed by alien plant life. Deeper mystery here. |
| **The Greenhollow** | Farm | A sheltered valley where settlers farm. Peaceful — until it isn't. |
| **The Murk** | Swamps | Toxic wetlands at the edge of the map. Something lives in there. |

---

## Player Character

### Identity
Player-named bounty hunter. They have a voice — opinions, dry humor, reactions in dialog — but the player chooses their name and makes the key decisions. Not a blank slate, not a fixed protagonist. Somewhere in between.

**Arrival hook**: You're here for a bounty. Someone on Dead Meridian has a price on their head, and you've been hired to collect. Simple job. Except your target is tangled up in something much bigger, and the deeper you dig, the less the bounty matters.

### Combat Style
Dual-mode, matching the STRANDED hero variants:
- **Gun** — Ranged combat. Pistols, rifles, scavenged energy weapons.
- **Blade** — Melee combat. Knives, machetes, salvaged tech-blades.

The player can switch between loadouts (or blend them). Not a class system — more of a playstyle spectrum. You find weapons, you use what you like.

### Companion
A small droid that's been with you for as long as you can remember. It was there before the bounty hunting, before everything. You trust it completely.

- Has always been with you — not a tutorial unlock, not earned. Just *there*.
- Its own personality: curious, mouthy, loyal, occasionally scared
- Opinions about your choices (and not shy about sharing them)
- A personal quest line — **the droid has secrets**. Things it hasn't told you. Things about where it came from, who made it, and why it's with *you* specifically.
- Possible connection to the ancient race / ruins (TBD — but the implication is rich)

Not a party member you manage — a character who's with you. Think R2-D2's loyalty crossed with HK-47's commentary. They act in combat (action points of their own?) but you don't micromanage them.

### Progression
- **No XP/leveling** — progression comes from gear, story access, and player skill
- **Weapons** found, bought, or crafted — each feels distinct
- **Upgrades** from The Order or Remnant Corp tech (faction-gated)
- **Reputation** with factions opens doors (and closes others)

---

## Core Mechanics

### Combat
Turn-based tactical combat with action points. A completely different register from Drift.

- **Action Points (AP)**: Each turn you have a pool of AP. Moving costs AP. Shooting costs AP. Using an item costs AP. Positioning matters.
- **Ranged**: Guns have range, accuracy, and ammo. Ammo is scarce — every shot should feel like a decision.
- **Melee**: Close-range, higher damage, but you have to spend AP getting there. Risk/reward.
- **Cover**: Use obstacles and terrain for defensive bonuses. Top-down tactical positioning.
- **The droid gets turns**: Companion acts on its own turn with limited AP. Can scan, distract, or attack. You don't control it directly — it makes its own choices (informed by its personality and your relationship).
- **Enemy variety**: Different enemies require different tactics. Bugs swarm, robots suppress, tribe warriors flank.
- **Lethality**: You can take a beating, but healing is scarce. Fights are won by smart positioning and resource management, not attrition.
- **Avoidable fights**: Some encounters can be talked out of, snuck around, or defused. Combat is a choice, not a default.

### Exploration
- Top-down movement through connected areas
- Each area has main paths and hidden zones
- Environmental storytelling: details in the world tell stories dialog doesn't
- Some areas gated by story progress, some by ability/gear

### Dialog & Interaction
- Walk up to NPC, press interact key
- Dialog box with typewriter text (9-patch UI panels)
- Branching choices on key conversations
- Choices affect faction reputation and story branches
- Not every conversation needs choices — some NPCs just talk

### Quests
- **Main quest**: The central mystery. 5-7 major beats across all areas.
- **Side quests**: Character-driven. Help a settler, investigate a disappearance, recover something from the ruins. Each should reveal something about the world.
- **No fetch quests**: If a quest involves retrieving something, the journey to get it should be interesting.
- **Quest log**: Simple list. Active quests, completed quests. No minimap markers — use directions and landmarks.

### Economy
- **Credits**: Universal currency. Earned from jobs, loot, selling scrap.
- **Merchants**: Each sells different goods. The Skull Merchant deals in forbidden tech. The Fruit Merchant is a front for something.
- **Crafting**: Light crafting at workbenches. Combine scrap + blueprint = upgrade. Not a deep system — more like "bring materials to the forge."

---

## Narrative Structure

### Act 1 — Arrival
- Player arrives at Dustport. Establish the town, key NPCs, and the surface-level status quo.
- Take odd jobs to earn trust and credits.
- Hear rumors about the ruins, the tribes, and why the corp never fully left.
- The droid is with you from the start — your only constant.
- First trip into The Barrens — tracking your bounty's trail. Encounter hostile fauna and scavengers.

### Act 2 — Descent
- Access to the Overgrown Vaults. First contact with pre-corp ruins.
- The Order takes interest in you (or tries to stop you).
- Faction tensions escalate — something is destabilizing the status quo.
- Discover that the thing you came here looking for is connected to the ruins.
- Choices start mattering: who do you trust? Who do you help?

### Act 3 — The Secret
- The truth about the ruins, the planet, and why the corps left.
- The factions converge — alliances shift based on your reputation.
- Final area opens (The Murk or a deeper ruin layer).
- Confrontation with the real antagonist (not who you expected).
- Resolution depends on faction relationships and key choices.

### Ending
Multiple endings based on:
- Faction reputation (who stands with you, who stands against you)
- Key moral choices (2-3 binary decisions that matter)
- Whether you completed the companion's personal quest

Not a "good/bad" binary. More like different truths about the world and your place in it.

---

## Tone & Writing Notes

- Dialog should feel natural, not fantasy-formal. People talk like people.
- Humor exists but it's character-driven, not quippy. More dry wit than Marvel.
- Violence has weight. Killing isn't casual — enemies are people (or creatures with implied ecology).
- The mystery should be genuinely surprising but feel inevitable in retrospect.
- Western genre tropes to lean into: the stranger in town, the standoff, the posse, the hidden past, the frontier code of honor.
- Sci-fi should be furniture, not the point. Spaceships exist but this isn't about space travel — it's about what happens on the ground.

---

## Open Questions

- [x] ~~What is the player looking for?~~ **A bounty.** Someone on Dead Meridian with a price on their head.
- [ ] Who is the bounty target? Likely someone who discovered the black site truth. The price on their head isn't justice — it's a cover-up.
- [ ] What is the central secret of the ruins? What did the corp find, and did they weaponize it?
- [ ] What's the corp's name? What do they look like from the outside? (Military contractor — think Weyland-Yutani, Cerberus, the ISB)
- [x] ~~What is the companion's deal?~~ **A droid that's always been with you. Has secrets — possibly connected to the ancient race.**
- [ ] What are the droid's secrets specifically? Who made it? Why is it with the player?
- [ ] Should there be a "wanted" system (corp bounty on you)?
- [x] ~~Who are "the corps"?~~ **A military contractor that used Dead Meridian as a black site.** Mining was the cover story. The ruins were the real objective. The "pullout" may not have been real — Remnant Corp is still on mission.
- [ ] How many weapons total? 8-12 feels right for meaningful variety.
- [ ] Is there a mount/vehicle for desert traversal, or is it all on foot?
- [x] ~~Name for the planet. Name for the game.~~ **Dead Meridian** — the planet and the title.
- [ ] Music and sound direction — the Kenney sounds are placeholder. What's the target?
