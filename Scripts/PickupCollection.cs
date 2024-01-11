using System.Collections.Generic;
using Godot;

public static class PickupCollection {
    public static List<PackedScene> PickupScenes { get; set; }

    public static void CompilePickupList() {
        PickupScenes = new() {
            GD.Load<PackedScene>("Scenes/Pickups/coin.tscn"),
            GD.Load<PackedScene>("Scenes/Pickups/bomb.tscn"),
            GD.Load<PackedScene>("Scenes/Pickups/key.tscn")
        };
        GD.Print($"Loaded pickup types: {PickupScenes.Count}");
    }
}
