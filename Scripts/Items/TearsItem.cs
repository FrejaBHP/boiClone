using Godot;
using System;

public partial class TearsItem : Item {
    public override int ItemID { get => 1; }
    public override string ItemName { get => "Tears Item"; }
    public override ItemCategories Categories { get => ItemCategories.NONE; }
    public override ItemPools Pools { get => ItemPools.Treasure; }

    private float delayBonus = 0.7f;

    public override void OnPickedUp() {
        Main.Player.AttackDelayBonus += delayBonus;
    }

    public override void OnRemoved() {
        Main.Player.AttackDelayBonus -= delayBonus;
    }
}
