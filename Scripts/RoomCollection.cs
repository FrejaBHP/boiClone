using System.Collections.Generic;
using Godot;

public static class RoomCollection {
    public static List<RoomData> RoomDataSet { get; set; }

    public static void CompileRoomList() {
        RoomDataSet = new() {
            new(0, GD.Load<PackedScene>("Scenes/Rooms/StartingRoom.tscn"), Exits.North | Exits.East | Exits.South | Exits.West, "Starting Room", RoomType.Normal),
            new(1, GD.Load<PackedScene>("Scenes/Rooms/RRoom.tscn"), Exits.West, "Right Room", RoomType.Normal),
            new(2, GD.Load<PackedScene>("Scenes/Rooms/UDLongRoom.tscn"), Exits.North | Exits.South, "Thin Room", RoomType.Normal),
            new(3, GD.Load<PackedScene>("Scenes/Rooms/UTestItemRoom.tscn"), Exits.North, "Test Item Room", RoomType.Treasure),
            new(4, GD.Load<PackedScene>("Scenes/Rooms/defaultRoom.tscn"), Exits.North | Exits.East | Exits.South | Exits.West, "Old Default Room", RoomType.Normal),
            new(5, GD.Load<PackedScene>("Scenes/Rooms/SRoom.tscn"), Exits.South, "Small Room", RoomType.Normal)
        };
        GD.Print($"Loaded rooms: {RoomDataSet.Count}");
    }
}
