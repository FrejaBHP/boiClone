using System.Collections.Generic;
using Godot;

public enum RoomNames {
    StartingRoom,
    R_Room,
    UD_LongRoom,
    U_TestItemRoom,
    UDLR_BaseRoom,
    S_Room
}

public static class RoomCollection {
    public static List<ExternalRoomData> RoomData { get; private set; }

    public static void CompileRoomList() {
        RoomData = new() {
            new(0, GD.Load<PackedScene>("Scenes/startingRoomTest.tscn")),
            new(1, GD.Load<PackedScene>("Scenes/treasureRoomFormatTest.tscn")),
            new(1, GD.Load<PackedScene>("Scenes/encounterRoomFormatTest.tscn"))
        };

        foreach (ExternalRoomData room in RoomData) {
            SceneState sceneState = room.Scene.GetState();

            for (int i = 0; i < sceneState.GetNodePropertyCount(0); i++) {
                switch (sceneState.GetNodePropertyName(0, i)) {
                    case "Exits":
                        room.SetMetaExits((Exits)sceneState.GetNodePropertyValue(0, i).AsInt32());
                        break;
                        
                    case "Type":
                        room.SetMetaType((RoomType)sceneState.GetNodePropertyValue(0, i).AsInt32());
                        break;
                    
                    default:
                        break;
                }
			}
            //GD.Print($"Exits: {room.Exits}. Type: {room.Type}");
        }
        GD.Print($"Loaded rooms: {RoomData.Count}");
    }
}
