using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class Room : Node2D {
	public int RoomID { get; set; }
	public RoomFlags Flags { get; set; }

	[Export]
	public string RoomName { get; private set; }
	[Export]
	public Exits Exits { get; private set; }
	[Export]
	public RoomType Type { get; private set; }

	public TileMap MapNode { get; private set; }
	public Node2D MarkersNode { get; private set; }
	public Node2D EntitiesNode { get; private set; }
	public Node2D ProjectilesNode { get; private set; }
	public Node2D PedestalsNode { get; private set; }
	public Node2D DoorsNode { get; private set; }
	public Node2D EnemiesNode { get; private set; }

	public List<Marker2D> EnemyMarkers { get; private set; }
	public List<Marker2D> ItemMarkers { get; private set; }

	private Area2D[] doors;

	public bool Visited { get; set; }
	public bool Seen { get; set; }

    public override void _Ready() {
		Visible = false;

		MapNode = GetNode<TileMap>("TileMap");
		MarkersNode = GetNode<Node2D>("Markers");
		EntitiesNode = GetNode<Node2D>("Entities");
		ProjectilesNode = GetNode<Node2D>("Projectiles");
		PedestalsNode = GetNode<Node2D>("Pedestals");
		DoorsNode = GetNode<Node2D>("Doors");
		EnemiesNode = GetNode<Node2D>("Enemies");

		doors = DoorsNode.GetChildren().Cast<Area2D>().ToArray();

		EnemyMarkers = new();
		ItemMarkers = new();
		foreach (Marker2D marker in MarkersNode.GetChildren().Cast<Marker2D>()) {
			switch ((int)marker.GetMeta("markerType")) {
				case 0:
					EnemyMarkers.Add(marker);
					break;
				
				case 1:
					ItemMarkers.Add(marker);
					break;

				case 2:
					break;

				default:
					GD.PushError($"Invalid markerType value of {(int)marker.GetMeta("markerType")}.");
					break;
			}
		}

		if (EnemyMarkers.Count > 0) {
			Flags |= RoomFlags.HasCombat;
		}
    }

	public void RemoveDoor(int dir) { // Still needs implementation for big rooms when they're added
		foreach (Area2D door in doors) {
			if ((int)door.GetMeta("direction") == dir) {
				Vector2I doorCoords = MapNode.LocalToMap(ToLocal(door.GlobalPosition));
				Vector2I tileCoords = MapNode.GetCellAtlasCoords(1, doorCoords);

				ReplaceDoorTileWithWall(doorCoords, tileCoords);
				
				door.Free();
				doors = DoorsNode.GetChildren().Cast<Area2D>().ToArray();
				break;
			}
		}
	}

	private void ReplaceDoorTileWithWall(Vector2I coords, Vector2I tile) { // Only uses default doors and wall textures. TODO: act-dependent switch?
		if (tile[0] == 0) {		// If horizontal door -
			Vector2I newTile = new(1, 6);
			MapNode.SetCell(1, coords, 4, newTile);
		}
		else {					// If vertical door |
			Vector2I newTile = new(0, 7);
			MapNode.SetCell(1, coords, 4, newTile);
		}
	}

	public void ReplaceDoorWithSecretDoor(int dir) { // Needs implementation for big rooms when they're added
		foreach (Area2D door in doors) {
			if ((int)door.GetMeta("direction") == dir) {
				Vector2I doorCoords = MapNode.LocalToMap(ToLocal(door.GlobalPosition));
				Vector2I tileCoords = MapNode.GetCellAtlasCoords(1, doorCoords);

				if (tileCoords[0] == 0) {	// If horizontal door -
					Vector2I newTile = new(1, 6);
					MapNode.EraseCell(1, doorCoords);
					MapNode.SetCell(2, doorCoords, 4, newTile);
				}
				else {						// If vertical door |
					Vector2I newTile = new(0, 7);
					MapNode.EraseCell(1, doorCoords);
					MapNode.SetCell(2, doorCoords, 4, newTile);
				}
				
				break;
			}
		}
	}

	public void OpenSecretDoor(int dir) {
		foreach (Area2D door in doors) {
			if ((int)door.GetMeta("direction") == dir) {
				Vector2I doorCoords = MapNode.LocalToMap(ToLocal(door.GlobalPosition));
				MapNode.EraseCell(2, doorCoords);
				
				break;
			}
		}
	}

	public void BlowUpObstacleTiles(Vector2 explosionScaledLocalPos, int explosionScaledRadius) {
        Vector2I[] allCells = MapNode.GetUsedCells(2).ToArray();
		foreach (Vector2I cell in allCells) {
            Vector2 tileLocalPos = MapNode.MapToLocal(cell);
			if (explosionScaledLocalPos.DistanceTo(tileLocalPos) <= explosionScaledRadius + 8) { // scaled explosion + quarter of a tile
				MapNode.EraseCell(2, cell);
				//GD.Print($"Destroyed cell at {tileLocalPos} (Map: {cell})");
			}
		}
	}

	public void CheckAndStartOpeningDoors() {
		Visited = true;

		foreach (Area2D door in DoorsNode.GetChildren().Cast<Area2D>()) {
			OpenDoor(door);
		}
	}

	private void OpenDoor(Area2D door) {
		int dir = (int)door.GetMeta("direction");

		door.BodyEntered += (body) => OnDoorEntered(dir, body);

		Vector2I doorCoords = MapNode.LocalToMap(ToLocal(door.GlobalPosition));
		Vector2I tileCoords = MapNode.GetCellAtlasCoords(1, doorCoords);

		//GD.Print($"DC: {Map.LocalToMap(ToLocal(door.GlobalPosition))}, TC: {Map.GetCellAtlasCoords(1, doorCoords)}");
		OpenDoorTile(doorCoords, tileCoords);
	}

	private void OpenDoorTile(Vector2I coords, Vector2I tile) {
		if (tile[0] == 0) {
			Vector2I newTile = new(6, tile[1]);
			MapNode.SetCell(1, coords, 0, newTile);
		}
		else {
			MapNode.SetCell(1, coords, -1);
		}
	}

	public void OnDoorEntered(int dir, Node2D body) {
		if (body.IsInGroup("Player")) {
			GetNode<World>("/root/Main/World").MoveRooms(dir);
		}
	}

	public void RemoveEmptyPedestals() {
		var pedestals = PedestalsNode.GetChildren();
		foreach (ItemPedestal pedestal in pedestals.Cast<ItemPedestal>()) {
			if (pedestal.ItemRemoved) {
				pedestal.QueueFree();
			}
		}
	}
}
