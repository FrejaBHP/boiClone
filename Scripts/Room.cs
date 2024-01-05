using Godot;
using System;
using System.Linq;
using System.Numerics;

public partial class Room : TileMap {
	public bool Visited { get; set; }

	private string GetDoorPath(int dir) {
		string nodePath = "";
		switch (dir) {
			case 0:
				nodePath = "Doors/DoorNorth";
				break;
			
			case 1:
				nodePath = "Doors/DoorEast";
				break;

			case 2:
				nodePath = "Doors/DoorSouth";
				break;

			case 3:
				nodePath = "Doors/DoorWest";
				break;
		}
		return nodePath;
	}

	public void RemoveDoor(int dir) {
		string nodePath = GetDoorPath(dir);
		
		Vector2I doorCoords = LocalToMap(ToLocal(GetNode<Area2D>(nodePath).GlobalPosition));
		Vector2I tileCoords = GetCellAtlasCoords(1, doorCoords);
		ReplaceDoorTile(doorCoords, tileCoords);

		GetNode<Area2D>(nodePath).Free();
	}

	private void ReplaceDoorTile(Vector2I coords, Vector2I tile) {
		if (tile[0] == 0) {		// If horizontal door -
			Vector2I newTile = new(1, 6);
			SetCell(1, coords, 4, newTile);
		}
		else {					// If vertical door |
			Vector2I newTile = new(0, 7);
			SetCell(1, coords, 4, newTile);
		}
	}

	public void CheckAndStartOpeningDoors() {
		Visited = true;
		if (GetNodeOrNull<Area2D>(GetDoorPath(0)) != null) {
			OpenDoor(GetDoorPath(0));
		}
		if (GetNodeOrNull<Area2D>(GetDoorPath(1)) != null) {
			OpenDoor(GetDoorPath(1));
		}
		if (GetNodeOrNull<Area2D>(GetDoorPath(2)) != null) {
			OpenDoor(GetDoorPath(2));
		}
		if (GetNodeOrNull<Area2D>(GetDoorPath(3)) != null) {
			OpenDoor(GetDoorPath(3));
		}
	}

	private void OpenDoor(string nodePath) {
		Area2D area = GetNode<Area2D>(nodePath);
		uint dir = (uint)area.GetMeta("direction");

		area.BodyEntered += (body) => OnDoorEntered(dir, body);

		Vector2I doorCoords = LocalToMap(ToLocal(GetNode<Area2D>(nodePath).GlobalPosition));
		Vector2I tileCoords = GetCellAtlasCoords(1, doorCoords);
		OpenDoorTile(doorCoords, tileCoords);
	}

	private void OpenDoorTile(Vector2I coords, Vector2I tile) {
		if (tile[0] == 0) {
			Vector2I newTile = new(6, tile[1]);
			SetCell(1, coords, 0, newTile);
		}
		else {
			SetCell(1, coords, -1);
		}
	}

	public void OnDoorEntered(uint dir, Node2D body) {
		GetNode<World>("/root/Main/World").MoveRooms(dir);
	}
}
