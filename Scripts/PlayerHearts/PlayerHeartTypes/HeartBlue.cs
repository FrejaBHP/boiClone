using Godot;

public class HeartBlue : HeartBase {
    private int halves;
    public override int Halves {
		get => halves;
		set {
            halves = value;
			ChangeSprite();
		} 
    }

    public override Texture2D Sprite { get; set; }

    public HeartBlue(int h) {
        Halves = h;
    }

    public override void ChangeSprite() {
        switch (halves) {
            case 1:
                Sprite = HeartSprites.HUDBlueHeartHalf;
                break;

            case 2:
                Sprite = HeartSprites.HUDBlueHeartFull;
                break;
        }
    }
}
