using System.Collections.Generic;
using Godot;

public enum PickupEType {
    Coin,
    Bomb,
    Key,
    RedHeart,
    BlueHeart,
    AMOUNT
}

public enum PickupEName {
    Coin,
    Coin5,
    Coin10,
    Bomb,
    Key,
    RedHeartFull,
    RedHeartHalf,
    BlueHeartFull,
    BlueHeartHalf
}

public static class PickupCollection {
    public static List<PackedScene> PickupScenes { get; private set; }

    public static void CompilePickupList() {
        PickupScenes = new() {
            GD.Load<PackedScene>("Scenes/Pickups/coin.tscn"),
            GD.Load<PackedScene>("Scenes/Pickups/coin5.tscn"),
            GD.Load<PackedScene>("Scenes/Pickups/coin10.tscn"),
            GD.Load<PackedScene>("Scenes/Pickups/bomb.tscn"),
            GD.Load<PackedScene>("Scenes/Pickups/key.tscn"),
            GD.Load<PackedScene>("Scenes/Pickups/fullRedHeart.tscn"),
            GD.Load<PackedScene>("Scenes/Pickups/halfRedHeart.tscn"),
            GD.Load<PackedScene>("Scenes/Pickups/fullBlueHeart.tscn"),
            GD.Load<PackedScene>("Scenes/Pickups/halfBlueHeart.tscn")
        };
        GD.Print($"Loaded pickup types: {PickupScenes.Count}");
    }
}
