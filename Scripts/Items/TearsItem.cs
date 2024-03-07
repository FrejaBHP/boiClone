using Godot;
using System;

public partial class TearsItem : Item, IInstantEffect {
    private float delayBonus = 0.7f;

    public TearsItem() {
        ItemDataID = 1;
    }

    public void OnPickedUp() {
        Main.Player.AttackDelayBonus += delayBonus;
    }

    public void OnRemoved() {
        Main.Player.AttackDelayBonus -= delayBonus;
    }
}
