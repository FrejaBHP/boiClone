using Godot;

public partial class WorldRoom {
    public int ID { get; set; }
    public Vector2I Coords { get; set; }
    public Room Room { get; set; }

    public WorldRoom(int id, int x, int y) {
        ID = id;
        //Data = RoomCollection.RoomDataSet[RoomCollection.RoomDataSet.FindIndex(r => r.ID == id)];
        Coords = Coords with { X = x, Y = y };
    }
}
