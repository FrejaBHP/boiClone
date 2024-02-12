using Godot;
using System;

public partial class Orange : Item {
    private int healthBonus = 1;
    private int halvesToFill = 2;

    public override void OnPickedUp() {
        Main.Player.GiveHeartContainers(healthBonus, halvesToFill);
    }

    public override void OnRemoved() {
        Main.Player.RemoveHeartContainers(healthBonus);
    }
}
