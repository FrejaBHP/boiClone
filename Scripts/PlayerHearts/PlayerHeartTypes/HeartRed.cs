using Godot;

public class HeartRed : HeartBase {
    private int halves;
    public override int Halves {
		get => halves;
		set {
            halves = value;
			ChangeSprite();
		} 
    }

    public override Texture2D Sprite { get; set; }

    public HeartRed() {
        
    }

    public override void ChangeSprite() {
        switch (halves) {
            case 0:
                Sprite = HeartSprites.HUDRedHeartEmpty;
                break;

            case 1:
                Sprite = HeartSprites.HUDRedHeartHalf;
                break;

            case 2:
                Sprite = HeartSprites.HUDRedHeartFull;
                break;
        }
    }
}
