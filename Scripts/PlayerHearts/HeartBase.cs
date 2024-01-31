using Godot;

public abstract class HeartBase {
    private int halves;
    public virtual int Halves {
		get => halves;
		set {
            halves = value;
			ChangeSprite();
		} 
    }

    public abstract Texture2D Sprite { get; set; }


    public abstract void OnPickup();

    public abstract void OnBroken();

    public abstract void ChangeSprite();
}