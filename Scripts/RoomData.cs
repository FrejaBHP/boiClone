using System;
using System.Collections.Generic;
using Godot;

[Flags]
public enum Exits {
    NONE = 0,
    North = 1 << 0,
    East = 1 << 1,
    South = 1 << 2,
    West = 1 << 3,
    Flex = 1 << 4
}

[Flags]
public enum RoomFlags {
    NONE = 0,
    Visited = 1 << 0,
    HasCombat = 1 << 1
}

public enum RoomType {
    Normal,
    Starting,
    Treasure,
    Boss,
    Secret,
    Shop
}

public partial class RoomData {
    public int ID { get; }
    public PackedScene Scene { get; }
    public Exits BaseExits { get; }
    public string Name { get; }
    public RoomType Type { get; }
    public RoomFlags Flags { get; set; }

    public RoomData(int id, PackedScene scene, Exits exits, string name, RoomType type) {
        ID = id;
        Scene = scene;
        BaseExits = exits;
        Name = name;
        Type = type;
    }
}
