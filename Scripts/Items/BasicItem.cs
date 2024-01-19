using Godot;
using System;

public partial class BasicItem : Item {
    public override int ItemID { get => 0; }
    public override string ItemName { get => "Basic Item"; }
    public override ItemCategories Categories { get => ItemCategories.NONE; }
    public override ItemPools Pools { get => ItemPools.Treasure; }

    private float damageBonus = 1;

    public override void OnPickedUp() {
        Main.Player.DamageBonus += damageBonus;
    }

    public override void OnRemoved() {
        Main.Player.DamageBonus -= damageBonus;
    }
}
