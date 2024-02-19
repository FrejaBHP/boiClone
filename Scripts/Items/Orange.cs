using Godot;
using System;

public partial class Orange : Item, IInstantEffect {
    private int healthBonus = 1;
    private int halvesToFill = 2;

    public void OnPickedUp() {
        Main.Player.GiveHeartContainers(healthBonus, halvesToFill);
    }

    public void OnRemoved() {
        Main.Player.RemoveHeartContainers(healthBonus);
    }
}
