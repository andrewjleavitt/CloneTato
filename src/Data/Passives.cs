namespace CloneTato.Data;

/// <summary>
/// Mechanical passive upgrades that change gameplay behavior (not just stat numbers).
/// Stacks additively — picking Ricochet twice = 2 bounces.
/// </summary>
public class Passives
{
    public int Ricochet;           // projectiles bounce to N nearby enemies
    public int VampiricHeal;       // heal N HP on melee kill
    public int ThornsDamage;       // deal N damage to enemies that hit you
    public bool ExplosiveKills;    // kills trigger a small AOE blast
    public float OverclockMult;    // secondary cooldown reduction (0.25 = 25% faster)
    public float AdrenalineWindow; // kill-streak window in seconds (0 = disabled)
    public float AdrenalineBoost;  // attack speed bonus during adrenaline (0.4 = 40%)

    // Adrenaline runtime state (not an upgrade, but tracked here for simplicity)
    public int AdrenalineKills;
    public float AdrenalineTimer;    // time since last kill in streak
    public float AdrenalineActive;   // remaining duration of attack speed buff
}
