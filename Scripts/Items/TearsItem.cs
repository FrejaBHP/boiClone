using Godot;
using System;

public partial class TearsItem : Item {
    public override int ItemID { get => 1; }
    public override string ItemName { get => "Tears Item"; }

    private float delayBonus = 0.7f;

    public override void OnPickedUp() {
        Main.Player.AttackDelayBonus += delayBonus;
    }

    public override void OnRemoved() {
        Main.Player.AttackDelayBonus -= delayBonus;
    }
}
