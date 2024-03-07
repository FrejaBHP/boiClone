using System;
using System.Collections.Generic;
using Godot;

[Flags]
public enum Exits {
    NONE = 0,
    North = 1 << 0,
    East = 1 << 1,
    South = 1 << 2,
    West = 1 << 3
}

[Flags]
public enum RoomFlags {
    NONE = 0,
    Visited = 1 << 0,
    HasCombat = 1 << 1
}

public enum RoomType {
    NONE,
    Default,
    Starting,
    Treasure,
    Boss,
    Secret,
    Shop
}

public partial class ExternalRoomData {
    public int ID { get; }
    public PackedScene Scene { get; }
    public Exits Exits { get; private set; }
    public RoomType Type { get; private set; }

    public ExternalRoomData(int id, PackedScene scene) {
        ID = id;
        Scene = scene;
    }

    public void SetMetaExits(Exits exits) {
        Exits = exits;
    }

    public void SetMetaType(RoomType type) {
        Type = type;
    }
}
