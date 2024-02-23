using System.Collections.Generic;
using Godot;

public class HeartBlack : HeartBase {
    private int halves;
    public override int Halves {
		get => halves;
		set {
            halves = value;
			ChangeSprite();
		} 
    }

    public override Texture2D Sprite { get; set; }

    public HeartBlack(int h) {
        Halves = h;
    }

    public void OnPickup() {

    }

    public void OnBroken() {
        Main.DamageAllEnemies(40);
    }

    public override void ChangeSprite() {
        switch (halves) {
            case 1:
                Sprite = HeartSprites.HUDBlackHeartHalf;
                break;

            case 2:
                Sprite = HeartSprites.HUDBlackHeartFull;
                break;
        }
    }
}
