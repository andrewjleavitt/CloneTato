using System.Numerics;
using CloneTato.Core;
using CloneTato.Data;
using CloneTato.Entities;
using Raylib_cs;

namespace CloneTato.Systems;

public static class WeaponSystem
{
    public const float OrbitRadius = 18f;

    private static float GetAttackSpeedMultiplier(GameState state)
    {
        var player = state.Player;
        float mult = player.ComputedStats.AttackSpeedMultiplier;
        if (player.DashBuffTimer > 0 && player.ComputedStats.PostDashAttackSpeed > 0)
            mult *= (1f + player.ComputedStats.PostDashAttackSpeed);
        return mult;
    }

    public static void Update(float dt, GameState state)
    {
        var player = state.Player;
        Vector2 mouseWorld = state.MouseWorldPosition;

        // Aim direction from player to mouse
        Vector2 aimDir = mouseWorld - player.Position;
        float aimDist = aimDir.Length();
        if (aimDist > 1f) aimDir /= aimDist;
        else aimDir = new Vector2(1, 0);
        float aimAngle = MathF.Atan2(aimDir.Y, aimDir.X);

        // Update orbit angles — weapons fan out toward aim direction
        int weaponCount = state.EquippedWeapons.Count;
        for (int w = 0; w < weaponCount; w++)
        {
            float spread = weaponCount > 1 ? MathF.PI * 0.6f : 0f;
            float step = weaponCount > 1 ? spread / (weaponCount - 1) : 0f;
            float targetAngle = aimAngle - spread / 2f + step * w;

            float current = state.WeaponOrbitAngles[w];
            float diff = NormalizeAngle(targetAngle - current);
            state.WeaponOrbitAngles[w] = current + diff * Math.Min(1f, 12f * dt);
        }

        // Tick all cooldowns and reload timers
        for (int w = 0; w < weaponCount; w++)
        {
            if (state.WeaponCooldowns[w] > 0)
                state.WeaponCooldowns[w] -= dt;

            // Reload timer
            if (state.WeaponReloadTimers[w] > 0)
            {
                state.WeaponReloadTimers[w] -= dt * state.Player.ComputedStats.ReloadSpeedMultiplier;
                if (state.WeaponReloadTimers[w] <= 0)
                {
                    // Reload complete — refill clip
                    state.WeaponClipAmmo[w] = state.EquippedWeapons[w].ClipSize;
                }
            }
        }

        // Update mines
        UpdateMines(dt, state);

        // Update melee swipe visuals
        for (int i = 0; i < state.MeleeSwipes.Count; i++)
            if (state.MeleeSwipes[i].Active)
                state.MeleeSwipes[i].Update(dt);

        // Fire weapons based on type
        for (int w = 0; w < weaponCount; w++)
        {
            if (state.WeaponCooldowns[w] > 0) continue;
            if (state.WeaponReloadTimers[w] > 0) continue; // reloading

            var weapon = state.EquippedWeapons[w];

            // Check clip (0 = unlimited, e.g. melee/mines)
            if (weapon.ClipSize > 0 && state.WeaponClipAmmo[w] <= 0)
            {
                // Start reload
                state.WeaponReloadTimers[w] = weapon.ReloadTime;
                continue;
            }

            switch (weapon.Def.Type)
            {
                case WeaponType.Auto when weapon.Def.IsLockOn:
                    UpdateLockOnWeapon(state, w, weapon);
                    break;
                case WeaponType.Auto:
                    UpdateAutoWeapon(state, w, weapon, aimDir, aimAngle);
                    break;
                case WeaponType.Manual:
                    UpdateManualWeapon(state, w, weapon, aimDir, aimAngle);
                    break;
                case WeaponType.Melee:
                    UpdateMeleeWeapon(state, w, weapon, aimDir, aimAngle);
                    break;
            }
        }
    }

    private static void UpdateAutoWeapon(GameState state, int w, WeaponInstance weapon,
        Vector2 aimDir, float aimAngle)
    {
        var player = state.Player;

        // Only fire when enemies are in range (but aim toward mouse, not nearest enemy)
        bool enemyInRange = false;
        for (int e = 0; e < state.Enemies.Count; e++)
        {
            if (!state.Enemies[e].Active || state.Enemies[e].IsDying) continue;
            if (Vector2.Distance(player.Position, state.Enemies[e].Position) < weapon.Range * 1.2f)
            {
                enemyInRange = true;
                break;
            }
        }
        if (!enemyInRange) return;

        // Fire toward mouse cursor
        float orbitAngle = state.WeaponOrbitAngles[w];
        Vector2 weaponPos = player.Position + new Vector2(
            MathF.Cos(orbitAngle) * OrbitRadius,
            MathF.Sin(orbitAngle) * OrbitRadius);

        Vector2 fireDir = state.MouseWorldPosition - weaponPos;
        float fireDist = fireDir.Length();
        if (fireDist > 1f) fireDir /= fireDist;
        else fireDir = aimDir;

        float cooldown = 1f / (weapon.FireRate * GetAttackSpeedMultiplier(state));
        state.WeaponCooldowns[w] = cooldown;

        // Consume ammo
        if (weapon.ClipSize > 0)
            state.WeaponClipAmmo[w]--;

        int damage = (int)weapon.Damage;

        if (weapon.BurstCount > 1)
        {
            float baseAngle = MathF.Atan2(fireDir.Y, fireDir.X);
            for (int b = 0; b < weapon.BurstCount; b++)
            {
                float spreadAngle = baseAngle + (Random.Shared.NextSingle() - 0.5f) * weapon.Def.Spread * 2f;
                Vector2 spreadDir = new(MathF.Cos(spreadAngle), MathF.Sin(spreadAngle));
                FireProjectile(state, weaponPos, spreadDir, weapon);
            }
        }
        else
        {
            if (weapon.Def.Spread > 0)
            {
                float baseAngle = MathF.Atan2(fireDir.Y, fireDir.X);
                float spreadAngle = baseAngle + (Random.Shared.NextSingle() - 0.5f) * weapon.Def.Spread;
                fireDir = new Vector2(MathF.Cos(spreadAngle), MathF.Sin(spreadAngle));
            }
            FireProjectile(state, weaponPos, fireDir, weapon);
        }

        state.Assets.PlaySoundVariant("shoot", 0.25f);
    }

    private static void UpdateManualWeapon(GameState state, int w, WeaponInstance weapon,
        Vector2 aimDir, float aimAngle)
    {
        // Only fire on mouse click
        if (!state.IsFiring) return;

        var player = state.Player;
        float cooldown = 1f / (weapon.FireRate * GetAttackSpeedMultiplier(state));
        state.WeaponCooldowns[w] = cooldown;

        // Consume ammo
        if (weapon.ClipSize > 0)
            state.WeaponClipAmmo[w]--;

        if (weapon.Def.IsMine)
        {
            // Place mine at player's feet
            var mine = state.GetInactiveMine();
            if (mine != null)
            {
                mine.Init(player.Position, (int)weapon.Damage, weapon.ExplosionRadius,
                    weapon.Def.MineProximity, weapon.Def.SpriteIndex);
            }
            state.Assets.PlaySoundVariant("select", 0.3f);
        }
        else
        {
            // Fire grenade/bomb toward mouse
            float orbitAngle = state.WeaponOrbitAngles[w];
            Vector2 weaponPos = player.Position + new Vector2(
                MathF.Cos(orbitAngle) * OrbitRadius,
                MathF.Sin(orbitAngle) * OrbitRadius);

            Vector2 fireDir = state.MouseWorldPosition - weaponPos;
            float dist = fireDir.Length();
            if (dist > 1f) fireDir /= dist;
            else fireDir = aimDir;

            FireProjectile(state, weaponPos, fireDir, weapon);
            state.Assets.PlaySoundVariant("shoot", 0.4f);
        }
    }

    private static void UpdateMeleeWeapon(GameState state, int w, WeaponInstance weapon,
        Vector2 aimDir, float aimAngle)
    {
        var player = state.Player;

        // Check if any enemy is in melee range
        bool enemyInRange = false;
        for (int e = 0; e < state.Enemies.Count; e++)
        {
            if (!state.Enemies[e].Active || state.Enemies[e].IsDying) continue;
            if (Vector2.Distance(player.Position, state.Enemies[e].Position) < weapon.Range + 10f)
            {
                enemyInRange = true;
                break;
            }
        }
        if (!enemyInRange) return;

        float cooldown = 1f / (weapon.FireRate * GetAttackSpeedMultiplier(state));
        state.WeaponCooldowns[w] = cooldown;

        float swingAngle = state.WeaponOrbitAngles[w];
        float halfArc = weapon.MeleeArc / 2f;
        int damage = (int)(weapon.Damage * player.ComputedStats.DamageMultiplier);

        // Damage all enemies in the arc
        int hitCount = 0;
        for (int e = 0; e < state.Enemies.Count; e++)
        {
            var enemy = state.Enemies[e];
            if (!enemy.Active || enemy.IsDying || enemy.IsBurrowed) continue;

            float dist = Vector2.Distance(player.Position, enemy.Position);
            if (dist > weapon.Range + enemy.Radius) continue;

            // Check if enemy is within the arc
            float angleToEnemy = MathF.Atan2(
                enemy.Position.Y - player.Position.Y,
                enemy.Position.X - player.Position.X);
            float angleDiff = MathF.Abs(NormalizeAngle(angleToEnemy - swingAngle));
            if (angleDiff > halfArc) continue;

            // Hit!
            bool crit = Random.Shared.NextSingle() < player.ComputedStats.CritChance;
            int finalDamage = crit ? (int)(damage * player.ComputedStats.CritDamage) : damage;
            finalDamage = enemy.ApplyFrontalReduction(finalDamage, player.Position);

            enemy.CurrentHP -= finalDamage;
            enemy.FlashTimer = 0.1f;
            state.TotalDamageDealt += finalDamage;
            hitCount++;

            // Knockback away from player
            Vector2 knockDir = dist > 1f
                ? Vector2.Normalize(enemy.Position - player.Position)
                : new Vector2(MathF.Cos(swingAngle), MathF.Sin(swingAngle));
            enemy.KnockbackVelocity = knockDir * Constants.KnockbackForce * 1.5f;
            enemy.KnockbackTimer = Constants.KnockbackDuration;

            var dmgNum = state.GetInactiveDamageNumber();
            dmgNum?.Init(enemy.Position, finalDamage.ToString(),
                crit ? Color.Yellow : Color.White);

            // Enemy death — hitstop on kill
            if (enemy.CurrentHP <= 0 && !enemy.IsDying)
            {
                state.HandleEnemyDeath(enemy);
                state.RequestHitstop(0.05f);  // ~3 frames freeze on melee kill
                state.RequestScreenShake(0.12f, 2.5f);
            }
        }

        // Trigger player melee attack animation
        player.MeleeAnimTimer = 0.35f;

        // Spawn swipe visual
        var swipe = state.GetInactiveMeleeSwipe();
        swipe?.Init(player.Position, swingAngle, weapon.MeleeArc, weapon.Range,
            hitCount > 0 ? Color.White : new Color((byte)200, (byte)200, (byte)200, (byte)150));

        if (hitCount > 0)
        {
            state.Assets.PlaySoundVariant("hurt", 0.2f);
            // Subtle screen shake on melee hit (not kill)
            state.RequestScreenShake(0.06f, 1.0f);
        }
        else
            state.Assets.PlaySoundVariant("move", 0.15f);
    }

    private static void UpdateLockOnWeapon(GameState state, int w, WeaponInstance weapon)
    {
        var player = state.Player;

        // Find all active, non-dying enemies
        var targets = new List<int>();
        for (int e = 0; e < state.Enemies.Count; e++)
        {
            if (!state.Enemies[e].Active || state.Enemies[e].IsDying) continue;
            if (Vector2.Distance(player.Position, state.Enemies[e].Position) < weapon.Range * 1.5f)
                targets.Add(e);
        }
        if (targets.Count == 0) return;

        float cooldown = 1f / (weapon.FireRate * GetAttackSpeedMultiplier(state));
        state.WeaponCooldowns[w] = cooldown;

        int missileCount = weapon.MissileCount;

        // Consume full clip
        if (weapon.ClipSize > 0)
            state.WeaponClipAmmo[w] = 0;

        // Distribute missiles among targets
        // If fewer targets than missiles, spread evenly
        int[] missilesPerTarget = new int[targets.Count];
        for (int i = 0; i < missileCount; i++)
            missilesPerTarget[i % targets.Count]++;

        float orbitAngle = state.WeaponOrbitAngles[w];
        Vector2 weaponPos = player.Position + new Vector2(
            MathF.Cos(orbitAngle) * OrbitRadius,
            MathF.Sin(orbitAngle) * OrbitRadius);

        for (int t = 0; t < targets.Count; t++)
        {
            for (int m = 0; m < missilesPerTarget[t]; m++)
            {
                var proj = state.GetInactiveProjectile();
                if (proj == null) break;

                // Launch in a spread pattern, missiles will home in
                float spreadAngle = orbitAngle + (Random.Shared.NextSingle() - 0.5f) * MathF.PI * 0.8f;
                Vector2 launchDir = new(MathF.Cos(spreadAngle), MathF.Sin(spreadAngle));

                float lifetime = weapon.Range / weapon.ProjectileSpeed * 2f; // extra lifetime for homing
                int damage = (int)weapon.Damage;
                proj.Init(weaponPos, launchDir * weapon.ProjectileSpeed, damage, lifetime,
                    0, weapon.Def.ProjectileColor, weapon.ExplosionRadius);
                proj.IsHoming = true;
                proj.TargetEnemyIndex = targets[t];
                proj.TurnRate = weapon.Def.MissileTurnRate;
            }
        }

        state.Assets.PlaySoundVariant("shoot", 0.4f);
    }

    public static void UpdateHomingProjectiles(float dt, GameState state)
    {
        for (int p = 0; p < state.Projectiles.Count; p++)
        {
            var proj = state.Projectiles[p];
            if (!proj.Active || !proj.IsHoming) continue;

            int ti = proj.TargetEnemyIndex;

            // Check if target is still valid
            if (ti < 0 || ti >= state.Enemies.Count ||
                !state.Enemies[ti].Active || state.Enemies[ti].IsDying)
            {
                // Find nearest alive enemy as new target
                float bestDist = float.MaxValue;
                int bestIdx = -1;
                for (int e = 0; e < state.Enemies.Count; e++)
                {
                    if (!state.Enemies[e].Active || state.Enemies[e].IsDying) continue;
                    float d = Vector2.Distance(proj.Position, state.Enemies[e].Position);
                    if (d < bestDist) { bestDist = d; bestIdx = e; }
                }
                proj.TargetEnemyIndex = bestIdx;
                ti = bestIdx;
            }

            if (ti < 0) continue; // no enemies left, fly straight

            // Steer toward target
            Vector2 toTarget = state.Enemies[ti].Position - proj.Position;
            float targetAngle = MathF.Atan2(toTarget.Y, toTarget.X);
            float currentAngle = MathF.Atan2(proj.Velocity.Y, proj.Velocity.X);
            float angleDiff = NormalizeAngle(targetAngle - currentAngle);

            float maxTurn = proj.TurnRate * dt;
            float turn = Math.Clamp(angleDiff, -maxTurn, maxTurn);
            float newAngle = currentAngle + turn;

            float speed = proj.Velocity.Length();
            // Accelerate slightly over time
            speed = Math.Min(speed + 80f * dt, speed * 1.5f);
            proj.Velocity = new Vector2(MathF.Cos(newAngle), MathF.Sin(newAngle)) * speed;
        }
    }

    private static void UpdateMines(float dt, GameState state)
    {
        for (int m = 0; m < state.Mines.Count; m++)
        {
            var mine = state.Mines[m];
            if (!mine.Active) continue;
            mine.Update(dt);
            if (mine.ArmTimer > 0) continue;

            // Check enemy proximity
            for (int e = 0; e < state.Enemies.Count; e++)
            {
                var enemy = state.Enemies[e];
                if (!enemy.Active || enemy.IsDying) continue;
                if (Vector2.Distance(mine.Position, enemy.Position) < mine.ProximityRadius + enemy.Radius)
                {
                    // Explode!
                    ExplodeAt(state, mine.Position, mine.Damage, mine.ExplosionRadius);
                    mine.Active = false;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// AOE explosion damage at a world position.
    /// </summary>
    public static void ExplodeAt(GameState state, Vector2 pos, int damage, float radius)
    {
        var player = state.Player;
        float dmgMult = player.ComputedStats.DamageMultiplier;

        for (int e = 0; e < state.Enemies.Count; e++)
        {
            var enemy = state.Enemies[e];
            if (!enemy.Active || enemy.IsDying || enemy.IsBurrowed) continue;
            float dist = Vector2.Distance(pos, enemy.Position);
            if (dist > radius + enemy.Radius) continue;

            // Damage falls off with distance
            float falloff = 1f - (dist / (radius + enemy.Radius)) * 0.5f;
            int finalDamage = (int)(damage * dmgMult * falloff);

            bool crit = Random.Shared.NextSingle() < player.ComputedStats.CritChance;
            if (crit) finalDamage = (int)(finalDamage * player.ComputedStats.CritDamage);
            finalDamage = enemy.ApplyFrontalReduction(finalDamage, pos);

            enemy.CurrentHP -= finalDamage;
            enemy.FlashTimer = 0.1f;
            state.TotalDamageDealt += finalDamage;

            // Knockback from explosion center
            Vector2 knockDir = dist > 1f
                ? Vector2.Normalize(enemy.Position - pos)
                : new Vector2(Random.Shared.NextSingle() - 0.5f, Random.Shared.NextSingle() - 0.5f);
            enemy.KnockbackVelocity = knockDir * Constants.KnockbackForce * 2f;
            enemy.KnockbackTimer = Constants.KnockbackDuration * 1.5f;

            var dmgNum = state.GetInactiveDamageNumber();
            dmgNum?.Init(enemy.Position, finalDamage.ToString(),
                crit ? Color.Yellow : Color.Orange);

            if (enemy.CurrentHP <= 0 && !enemy.IsDying)
                state.HandleEnemyDeath(enemy);
        }

        state.Assets.PlaySoundVariant("explosion", 0.5f);
    }

    public static Vector2 GetWeaponWorldPosition(GameState state, int weaponIndex)
    {
        float angle = state.WeaponOrbitAngles[weaponIndex];
        return state.Player.Position + new Vector2(
            MathF.Cos(angle) * OrbitRadius,
            MathF.Sin(angle) * OrbitRadius);
    }

    public static float GetWeaponDrawAngle(GameState state, int weaponIndex)
    {
        return state.WeaponOrbitAngles[weaponIndex];
    }

    private static void FireProjectile(GameState state, Vector2 origin, Vector2 dir, WeaponInstance weapon)
    {
        var proj = state.GetInactiveProjectile();
        if (proj == null) return;

        float lifetime = weapon.Range / weapon.ProjectileSpeed;
        int damage = (int)weapon.Damage;
        proj.Init(origin, dir * weapon.ProjectileSpeed, damage, lifetime,
            weapon.PierceCount, weapon.Def.ProjectileColor, weapon.ExplosionRadius);
    }

    private static float NormalizeAngle(float angle)
    {
        while (angle > MathF.PI) angle -= MathF.PI * 2f;
        while (angle < -MathF.PI) angle += MathF.PI * 2f;
        return angle;
    }
}
