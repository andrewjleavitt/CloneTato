# Untitled Sci-Fi Western Action RPG — Game Design Document

*Working document. Everything here is open to revision.*

## Vision

A top-down action RPG set on a lawless frontier planet at the edge of settled space. Tone sits between Firefly's scrappy warmth, Andor's grounded tension, and The Mandalorian's lone-gunslinger mystique. The player is a newcomer — arrived for their own reasons — who gets pulled into the web of a world that's more complicated than it looks.

Pixel art. Tight combat. Characters you remember. Secrets worth finding.

## Pillars

1. **Story first** — Every quest, every NPC, every area should serve the narrative. Combat is a verb in the story, not the story itself.
2. **Earned mystery** — The world has deep secrets, but they're revealed through exploration and relationships, not exposition dumps. Players should feel like detectives, not tourists.
3. **Space cowboy fantasy** — You're resourceful, outnumbered, and operating in morally gray territory. The power fantasy is cleverness and grit, not overwhelming force.
4. **Small world, dense world** — Better to have 5 areas that feel alive than 20 that feel empty.

---

## Setting

### The Planet

A remote desert world on the fringe of civilized space. Once a corporate mining colony, now largely abandoned by the corps and left to whoever stayed behind. The landscape is harsh — cracked desert, dust storms, and ancient rock formations — but not dead. Life has adapted. People have adapted.

The planet has layers:
- **Surface**: What everyone sees. Frontier towns, tribal territories, merchant routes, wildlife.
- **Underneath**: Pre-corporate ruins. Something was here before the mining corps arrived. Ancient structures buried under sand. Technology that doesn't match anything known.
- **The Secret**: What the ruins actually are, who built them, and why the corps really left. This is the central mystery.

### Factions

| Faction | Role | Analog |
|---------|------|--------|
| **Settlers** | Frontier townspeople. Farmers, mechanics, bartenders. Just trying to get by. | Firefly's border moon settlers |
| **The Tribes** | Indigenous groups who were here before the corps. Know more about the ruins than they let on. | Tusken Raiders meets Fremen |
| **Remnant Corp** | Skeleton crew of corporate security still running ops. What are they still doing here? | The Empire's quiet reach |
| **Scavengers** | Raiders and opportunists picking over what the corps left behind. | Jawas meets Reavers (less horrifying) |
| **The Order** | Scholars/monks studying the ruins. Neutral but secretive. May not be what they seem. | Jedi archives crossed with a frontier monastery |

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
The player character has a past but it's lightly sketched — enough to motivate arrival on the planet, vague enough for player projection. They're not a blank slate (they have opinions, a voice in dialog) but they're not a fully predetermined character either.

**Arrival hook**: You came here looking for someone. Or something. The opening establishes your reason, and the main quest gradually reveals that your personal search is tangled up with the planet's deeper secrets.

### Combat Style
Dual-mode, matching the STRANDED hero variants:
- **Gun** — Ranged combat. Pistols, rifles, scavenged energy weapons.
- **Blade** — Melee combat. Knives, machetes, salvaged tech-blades.

The player can switch between loadouts (or blend them). Not a class system — more of a playstyle spectrum. You find weapons, you use what you like.

### Companion
One companion travels with you. They have:
- Their own personality and dialog
- Idle, move, gather, and attack animations (already in assets)
- Opinions about your choices
- A personal quest line

The companion isn't a party member you manage — they're a character who's with you. Think Grogu, Cassian's contacts, Kaylee. They react, they help, they sometimes disagree.

### Progression
- **No XP/leveling** — progression comes from gear, story access, and player skill
- **Weapons** found, bought, or crafted — each feels distinct
- **Upgrades** from The Order or Remnant Corp tech (faction-gated)
- **Reputation** with factions opens doors (and closes others)

---

## Core Mechanics

### Combat
Action RPG combat — real-time, skill-based, moderately punishing.

- **Ranged**: Aim with mouse, fire with click. Ammo is scarce enough to matter.
- **Melee**: Close-range attacks with timing windows. Dodge roll for defense.
- **Dodge roll**: Invincibility frames. Already implemented in NukeDesert skeleton.
- **Enemy variety**: Different enemies require different tactics. Bugs swarm, robots shoot, tribe warriors flank.
- **No bullet sponges**: Enemies die in reasonable hits. So does the player. Fights are fast and lethal.

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
- Companion joins you (or you find them).
- First trip into The Barrens — encounter hostile fauna and scavengers.

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

- [ ] What is the player looking for? (A person? A ship? A signal? Their past?)
- [ ] What is the central secret of the ruins?
- [ ] What is the companion's deal? Why are they alone?
- [ ] Should there be a "wanted" system (Remnant Corp bounty on you)?
- [ ] How many weapons total? 8-12 feels right for meaningful variety.
- [ ] Is there a mount/vehicle for desert traversal, or is it all on foot?
- [ ] Name for the planet. Name for the game.
- [ ] Music and sound direction — the Kenney sounds are placeholder. What's the target?
