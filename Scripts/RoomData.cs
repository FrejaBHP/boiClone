using Godot;

public partial class RoomData {
    public int RoomID { get; set; }
    public Vector2I RoomCoords { get; set; }
    public TileMap RoomMap { get; set; }

    public RoomData(int id, int x, int y) {
        RoomID = id;
        RoomCoords = RoomCoords with { X = x, Y = y };
    }
}
