using Godot;

public partial class HeartContainer {
    public HeartRed RedHeart { get; set; }

    public HeartContainer(int startingHalves) {
        RedHeart = new() {
            Halves = startingHalves
        };
    }
}