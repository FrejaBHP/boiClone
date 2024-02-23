using Godot;
using System;

public partial class BasicItem : Item, IInstantEffect {
    public override int ItemDataID => base.ItemDataID;
    private float damageBonus = 1;

    public BasicItem() {
        ItemDataID = 0;
    }

    public void OnPickedUp() {
        Main.Player.DamageBonus += damageBonus;
    }

    public void OnRemoved() {
        Main.Player.DamageBonus -= damageBonus;
    }
}
