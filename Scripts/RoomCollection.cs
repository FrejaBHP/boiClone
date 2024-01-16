using System.Collections.Generic;
using Godot;

public static class RoomCollection {
    /*
    public static List<PackedScene> RoomScenes { get; set; }

    public static void CompileRoomList() {
        RoomScenes = new() {
            GD.Load<PackedScene>("Scenes/Rooms/StartingRoom.tscn"),
            GD.Load<PackedScene>("Scenes/Rooms/RRoom.tscn"),
            GD.Load<PackedScene>("Scenes/Rooms/UDLongRoom.tscn"),
            GD.Load<PackedScene>("Scenes/Rooms/UTestItemRoom.tscn"),
            GD.Load<PackedScene>("Scenes/Rooms/defaultRoom.tscn"),
            GD.Load<PackedScene>("Scenes/Rooms/SRoom.tscn")
        };
        GD.Print($"Loaded rooms: {RoomScenes.Count}");
    }
    */

    public static List<RoomData> RoomDataSet { get; set; }

    public static void CompileRoomList() {
        RoomDataSet = new() {
            new(0, GD.Load<PackedScene>("Scenes/Rooms/StartingRoom.tscn"), Exits.North | Exits.East | Exits.South | Exits.West, "Starting Room", RoomType.Normal, RoomFlags.NONE),
            new(1, GD.Load<PackedScene>("Scenes/Rooms/RRoom.tscn"), Exits.West, "Right Room", RoomType.Normal, RoomFlags.HasCombat),
            new(2, GD.Load<PackedScene>("Scenes/Rooms/UDLongRoom.tscn"), Exits.North | Exits.South, "Thin Room", RoomType.Normal, RoomFlags.HasCombat),
            new(3, GD.Load<PackedScene>("Scenes/Rooms/UTestItemRoom.tscn"), Exits.North, "Test Item Room", RoomType.Item, RoomFlags.NONE),
            new(4, GD.Load<PackedScene>("Scenes/Rooms/defaultRoom.tscn"), Exits.North | Exits.East | Exits.South | Exits.West, "Old Default Room", RoomType.Normal, RoomFlags.HasCombat),
            new(5, GD.Load<PackedScene>("Scenes/Rooms/SRoom.tscn"), Exits.South, "Small Room", RoomType.Normal, RoomFlags.HasCombat)
        };
        GD.Print($"Loaded rooms: {RoomDataSet.Count}");
    }

    /*
    private static void LoadRooms() {
        //int tC = 0;
        foreach (RoomData room in RoomDataSet) {
            GD.Load<PackedScene>(room.ScenePath);
            GD.Print($"Path: {room.ScenePath}, Exits: {room.BaseExits}, Name: {room.Name}");
            //tC++;
        }
        //GD.Print($"Loaded rooms: {tC}");
    }
    */
}
