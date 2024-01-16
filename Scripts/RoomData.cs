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
    HasCombat = 1 << 0
}

public enum RoomType {
    Normal,
    Starting,
    Item,
    Boss,
    Secret,
    Shop
}

public partial class RoomData {
    public int ID { get; set; }
    public PackedScene Scene { get; set; }
    public Exits BaseExits { get; set; }
    public string Name { get; set; }
    public RoomType Type { get; set; }
    public RoomFlags Flags { get; set; }

    public RoomData(int id, PackedScene scene, Exits exits, string name, RoomType type, RoomFlags flags) {
        ID = id;
        Scene = scene;
        BaseExits = exits;
        Name = name;
        Type = type;
        Flags = flags;
    }
}
