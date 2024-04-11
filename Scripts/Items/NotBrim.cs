using Godot;
using System;

public partial class NotBrim : Item, IInstantEffect {
    private readonly float fireRatePenalty = 1f/3f;
    private readonly float damageMult = 1.2f;

    public NotBrim() {
        ItemDataID = 5;
    }

    public void OnPickedUp() {
        Main.Player.TryOverrideAttackType(AttackType.Beam);
        Main.Player.AttackRateMultiplier *= fireRatePenalty;
        Main.Player.DamageMultiplier *= damageMult;
    }

    public void OnRemoved() {
        Main.Player.TryOverrideAttackType(AttackType.Projectile);
        Main.Player.AttackRateMultiplier /= fireRatePenalty;
        Main.Player.DamageMultiplier /= damageMult;
    }
}
