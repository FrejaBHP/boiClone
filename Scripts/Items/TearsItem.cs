using Godot;
using System;

public partial class TearsItem : Item {
    public override float AttackDelayBonus { get; set; } = 0.7f;
    public TearsItem() {
        ItemID = 1;
        ItemName = "Tears Item";
    }
}
