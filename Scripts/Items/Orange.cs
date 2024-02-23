using Godot;
using System;

public partial class Orange : Item, IInstantEffect {
    public override int ItemDataID => base.ItemDataID;
    private int healthBonus = 1;
    private int halvesToFill = 2;

    public Orange() {
        ItemDataID = 2;
    }

    public void OnPickedUp() {
        Main.Player.GiveHeartContainers(healthBonus, halvesToFill);
    }

    public void OnRemoved() {
        Main.Player.RemoveHeartContainers(healthBonus);
    }
}
