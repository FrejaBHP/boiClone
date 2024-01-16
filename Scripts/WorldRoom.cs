using Godot;

public partial class WorldRoom {
    public RoomData Data { get; set; }
    public Vector2I Coords { get; set; }
    public TileMap Map { get; set; }

    public WorldRoom(int id, int x, int y) {
        Data = RoomCollection.RoomDataSet[RoomCollection.RoomDataSet.FindIndex(r => r.ID == id)];
        Coords = Coords with { X = x, Y = y };
    }
}
