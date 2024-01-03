using System.Collections.Generic;
using Godot;

public static class RoomCollection {
    public static List<PackedScene> RoomScenes { get; set; }
    static RoomCollection() {
        //CompileRoomList();
    }

    public static void CompileRoomList() {
        RoomScenes = new() {
            GD.Load<PackedScene>("Scenes/Rooms/defaultRoom.tscn"),
            GD.Load<PackedScene>("Scenes/Rooms/RRoom.tscn"),
            GD.Load<PackedScene>("Scenes/Rooms/UDLongRoom.tscn"),
            GD.Load<PackedScene>("Scenes/Rooms/UTestItemRoom.tscn")
        };
        GD.Print($"Loaded rooms: {RoomScenes.Count}");
    }
}
