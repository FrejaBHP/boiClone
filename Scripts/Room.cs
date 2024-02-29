using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public partial class Room : Node2D {
	public RoomData Data { get; set; }
	public TileMap Map { get; set; }

	
	public Node2D Pedestals { get; private set; }
	private Node2D doors;

	public List<Marker2D> EnemyMarkers { get; private set; }
	public List<Marker2D> ItemMarkers { get; private set; }

	public bool Visited { get; set; }
	public bool Seen { get; set; }

    public override void _Ready() {
		Visible = false;

		Pedestals = GetNode<Node2D>("Pedestals");
		doors = Map.GetNode<Node2D>("Doors");

		EnemyMarkers = new();
		ItemMarkers = new();
		Node markersNode = Map.GetNode("Markers");
		foreach (Marker2D marker in markersNode.GetChildren().Cast<Marker2D>()) {
			switch ((int)marker.GetMeta("nodeType")) {
				case 0:
					EnemyMarkers.Add(marker);
					break;
				
				case 1:
					ItemMarkers.Add(marker);
					break;

				case 2:
					break;
			}
		}

		if (EnemyMarkers.Count > 0) {
			Data.Flags |= RoomFlags.HasCombat;
		}
		//GD.Print($"Enemies: {EnemyMarkers.Count}, Items: {ItemMarkers.Count}");
    }

	public void RemoveDoor(int dir) { // Legacy function, obsolete!! Fix for new rooms structure and big rooms
	/*
		string nodePath = GetDoorPath(dir);
		
		Vector2I doorCoords = Map.LocalToMap(ToLocal(GetNode<Area2D>(nodePath).GlobalPosition));
		Vector2I tileCoords = Map.GetCellAtlasCoords(1, doorCoords);
		ReplaceDoorTile(doorCoords, tileCoords);

		GetNode<Area2D>(nodePath).Free();
		*/
	}

	private void ReplaceDoorTile(Vector2I coords, Vector2I tile) {
		if (tile[0] == 0) {		// If horizontal door -
			Vector2I newTile = new(1, 6);
			Map.SetCell(1, coords, 4, newTile);
		}
		else {					// If vertical door |
			Vector2I newTile = new(0, 7);
			Map.SetCell(1, coords, 4, newTile);
		}
	}

	public void CheckAndStartOpeningDoors() {
		Visited = true;

		foreach (Area2D door in doors.GetChildren().Cast<Area2D>()) {
			OpenDoor(door);
		}
	}

	private void OpenDoor(Area2D door) {
		int dir = (int)door.GetMeta("direction");

		door.BodyEntered += (body) => OnDoorEntered(dir, body);

		Vector2I doorCoords = Map.LocalToMap(ToLocal(door.GlobalPosition));
		Vector2I tileCoords = Map.GetCellAtlasCoords(1, doorCoords);

		//GD.Print($"DC: {Map.LocalToMap(ToLocal(door.GlobalPosition))}, TC: {Map.GetCellAtlasCoords(1, doorCoords)}");

		OpenDoorTile(doorCoords, tileCoords);
	}

	private void OpenDoorTile(Vector2I coords, Vector2I tile) {
		if (tile[0] == 0) {
			Vector2I newTile = new(6, tile[1]);
			Map.SetCell(1, coords, 0, newTile);
		}
		else {
			Map.SetCell(1, coords, -1);
		}
	}

	public void OnDoorEntered(int dir, Node2D body) {
		if (body.IsInGroup("Player"))
			GetNode<World>("/root/Main/World").MoveRooms(dir);
	}

	public void RemoveEmptyPedestals() {
		var pedestals = Pedestals.GetChildren();
		foreach (WorldItem pedestal in pedestals.Cast<WorldItem>()) {
			if (pedestal.ItemRemoved) {
				pedestal.QueueFree();
			}
		}
	}
}
