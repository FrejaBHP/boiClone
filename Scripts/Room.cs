using Godot;
using System;
using System.Linq;
using System.Numerics;

public partial class Room : TileMap {
	public bool Visited { get; set; }

	public void OpenDoors() {
		Visited = true;
		if (GetNodeOrNull<Area2D>("Doors/DoorNorth") != null) {
			GetNode<Area2D>("Doors/DoorNorth").BodyEntered += OnNorthEntered;
			Vector2I doorCoords = LocalToMap(ToLocal(GetNode<Area2D>("Doors/DoorNorth").GlobalPosition));
			Vector2I tileCoords = GetCellAtlasCoords(1, doorCoords);
			OpenDoorTile(doorCoords, tileCoords);
		}
		if (GetNodeOrNull<Area2D>("Doors/DoorEast") != null) {
			GetNode<Area2D>("Doors/DoorEast").BodyEntered += OnEastEntered;
			Vector2I doorCoords = LocalToMap(ToLocal(GetNode<Area2D>("Doors/DoorEast").GlobalPosition));
			Vector2I tileCoords = GetCellAtlasCoords(1, doorCoords);
			OpenDoorTile(doorCoords, tileCoords);
		}
		if (GetNodeOrNull<Area2D>("Doors/DoorSouth") != null) {
			GetNode<Area2D>("Doors/DoorSouth").BodyEntered += OnSouthEntered;
			Vector2I doorCoords = LocalToMap(ToLocal(GetNode<Area2D>("Doors/DoorSouth").GlobalPosition));
			Vector2I tileCoords = GetCellAtlasCoords(1, doorCoords);
			OpenDoorTile(doorCoords, tileCoords);
		}
		if (GetNodeOrNull<Area2D>("Doors/DoorWest") != null) {
			GetNode<Area2D>("Doors/DoorWest").BodyEntered += OnWestEntered;
			Vector2I doorCoords = LocalToMap(ToLocal(GetNode<Area2D>("Doors/DoorWest").GlobalPosition));
			Vector2I tileCoords = GetCellAtlasCoords(1, doorCoords);
			OpenDoorTile(doorCoords, tileCoords);
		}
	}

	private void OpenDoorTile(Vector2I coords, Vector2I tile) {
		if (tile[1] == 0) {
			if (tile[0] == 0) {
				Vector2I newTile = new(6, tile[1]);
				SetCell(1, coords, 0, newTile);
			}
			else if (tile[0] == 1) {
				SetCell(1, coords, -1);
			}
		}
	}

	public void OnNorthEntered(Node2D body) {
		GetNode<World>("/root/Main/World").MoveRooms(0);
	}

	public void OnEastEntered(Node2D body) {
		GetNode<World>("/root/Main/World").MoveRooms(1);
	}

	public void OnSouthEntered(Node2D body) {
		GetNode<World>("/root/Main/World").MoveRooms(2);
	}

	public void OnWestEntered(Node2D body) {
		GetNode<World>("/root/Main/World").MoveRooms(3);
	}
}
