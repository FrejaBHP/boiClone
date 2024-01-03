using Godot;
using System;

public partial class BasicItem : Item {
    //public override Texture2D ItemSprite { get; set; }
    public override float DamageBonus { get; set; } = 1;
    public BasicItem() {
        ItemID = 0;
        ItemName = "Basic Item";
        //ItemSprite = GD.Load<Texture2D>("../Images/TestWep.png");
    }

    protected override void OnPickedUp() {
        GD.Print("Nice!");
    }

    protected override void OnRemoved() {

    }
}
