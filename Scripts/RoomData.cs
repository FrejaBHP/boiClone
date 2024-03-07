using System;
using System.Collections.Generic;
using Godot;

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
