using Godot;
using System;

public partial class BasicItem : Item {
    private float damageBonus = 1;

    public override void OnPickedUp() {
        Main.Player.DamageBonus += damageBonus;
    }

    public override void OnRemoved() {
        Main.Player.DamageBonus -= damageBonus;
    }
}
