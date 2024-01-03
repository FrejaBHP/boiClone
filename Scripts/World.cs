using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class World : Node {
	public static RoomData[,] RoomsA { get; set; }
	public static Vector2I CurrentCoords { get; set; }
	public static TileMap CurrentTileMap { get; set; }
	
	private static int enemiesLeft = 0;
	private static readonly int gridSize = 50;
	private static readonly int gridGapX = 608;
	private static readonly int gridGapY = 416;
	private static readonly int skip = 216;

	protected PackedScene Player = GD.Load<PackedScene>("Scenes/player.tscn");
	protected PackedScene ItemPedestal = GD.Load<PackedScene>("Scenes/worldItem.tscn");

	public World() {
		RoomsA = new RoomData[gridSize, gridSize];
		var c = CurrentCoords;
		c.X = gridSize / 2;
		c.Y = gridSize / 2;
		CurrentCoords = c;
	}

    public override void _Ready() {
		BuildLevel();
	}

	private void BuildLevel() {
		//Until coming across a better format, maps are kept track of in a 2D array for navigation
		//And since arrays don't like negative indeces, it's offset by 25 later (for a 50x50 total map size atm)
		//Coordinates here are however still written as starting at [0, 0] for ease of use

		List<RoomData> tempList = new() {
			new((int)RoomNames.UDLR_BaseRoom, 0, 0),
			new((int)RoomNames.UDLR_BaseRoom, 1, 0),
			new((int)RoomNames.R_Room, -1, 0),
			new((int)RoomNames.UD_LongRoom, 0, 1),
			new((int)RoomNames.U_TestItemRoom, 0, 2),
			new((int)RoomNames.UDLR_BaseRoom, 0, -1)
		};
		
		PlaceRooms(tempList);
	}

	private void PlaceRooms(List<RoomData> list) {
		foreach (RoomData data in list) {
			if (RoomCollection.RoomScenes[data.RoomID] != null) {
				TileMap newRoom = RoomCollection.RoomScenes[data.RoomID].Instantiate() as TileMap;
				data.RoomMap = newRoom;
				RoomsA[data.RoomCoords.X + (gridSize / 2), data.RoomCoords.Y + (gridSize / 2)] = data;
				AddChild(newRoom);
			}
			else {
				GD.PushError($"Room with ID {data.RoomID} not found.");
			}

			Vector2 grid = data.RoomMap.GlobalPosition;
			grid.X = data.RoomCoords.X * gridGapX;
			grid.Y = data.RoomCoords.Y * gridGapY;
			data.RoomMap.GlobalPosition = grid;
		}
		CurrentTileMap = RoomsA[gridSize / 2, gridSize / 2].RoomMap;
		//ProcessRoomMarkers(CurrentTileMap);
		AddPlayer();
		CheckIsRoomClear();
	}

	private void AddPlayer() {
		Player player = Player.Instantiate() as Player;
		AddChild(player);

		//Stats separated to allow for different kinds of characters later
		//Should probably be moved somewhere else, however
		player.MaxHealth = 3f;
		player.Health = player.MaxHealth;
		player.Speed = 1f;
		player.ShotSpeed = 1f;
		player.Range = 6.5f;
		player.Damage = 3.5f;
		player.AttackDelay = 0f;
		player.Team = 0;

		var pos = player.GlobalPosition;
        pos.X = 240;
		pos.Y = 144;
		player.GlobalPosition = pos; //Middle of the starting room
	}

	private void ProcessRoomMarkers(TileMap room) {
		Node markersNode = room.GetNode("Markers");
		foreach (Marker2D marker in markersNode.GetChildren().Cast<Marker2D>()) {
			switch ((int)marker.GetMeta("nodeType")) {
				case 0:
					break;
				
				case 1:
					CreateItem((int)marker.GetMeta("entityID"), marker.GlobalPosition);
					break;

				case 2:
					CreateEnemy((int)marker.GetMeta("entityID"), marker.GlobalPosition);
					enemiesLeft++;
					break;
				
				default:
					break;
			}
		}

		if (enemiesLeft == 0) {
			CheckIsRoomClear();
		}
	}

	private void CreateItem(int itemID, Vector2 markerPos) {
		WorldItem newPedestal = ItemPedestal.Instantiate() as WorldItem;
		Sprite2D itemSprite = newPedestal.GetNode<Sprite2D>("ItemSpriteItem");

		// If itemID is -1, it's interpreted as a random item spawn, otherwise it's a set spawn
		if (itemID == -1) {
			if (ItemCollection.ItemTypes.Count != 0) {
				Random random = new();
				int rnd = random.Next(0, ItemCollection.ItemTypes.Count);
				itemSprite.Texture = GD.Load<Texture2D>(ItemCollection.ItemSpritePaths[rnd]);
				newPedestal.SetMeta("itemID", rnd);
			}
			else {
				GD.PushError($"No items found.");
			}
		}
		else {
			if (ItemCollection.ItemTypes[itemID] != null) {
				itemSprite.Texture = GD.Load<Texture2D>(ItemCollection.ItemSpritePaths[itemID]);
				newPedestal.SetMeta("itemID", itemID);
			}
			else {
				GD.PushError($"Item with ID {itemID} not found.");
			}
		}
		
		AddChild(newPedestal);
		newPedestal.GlobalPosition = markerPos;
	}

	private void CreateEnemy(int enemyID, Vector2 markerPos) {
		if (EnemyCollection.EnemyScenes[enemyID] != null) {
			Enemy newEnemy = EnemyCollection.EnemyScenes[enemyID].Instantiate() as Enemy;
			AddChild(newEnemy);
			newEnemy.GlobalPosition = markerPos;
		}
		else {
			GD.PushError($"Enemy with ID {enemyID} not found.");
		}
	}

	public void DecreaseEnemyCount() {
		enemiesLeft--;
		CheckIsRoomClear();
	}

	private void CheckIsRoomClear() {
		if (enemiesLeft == 0) {
			(CurrentTileMap as Room).OpenDoors();
		}
	}

	public void MoveRooms(int dir) {
		if (GetNode<Timer>("RoomInitDelay").TimeLeft == 0) {
			Vector2 pos = Main.Player.GlobalPosition;
			Vector2 cpos = Main.Camera.GlobalPosition;
			var c = CurrentCoords;
			
			switch (dir) {
				case 0:
					pos.Y -= skip;
					cpos.Y -= gridGapY;
					c.Y--;
					break;
					
				case 1:
					pos.X += skip;
					cpos.X += gridGapX;
					c.X++;
					break;

				case 2:
					pos.Y += skip;
					cpos.Y += gridGapY;
					c.Y++;
					break;

				case 3:
					pos.X -= skip;
					cpos.X -= gridGapX;
					c.X--;
					break;

				default:
					break;
			}

			Main.Player.GlobalPosition = pos;
			Main.Camera.GlobalPosition = cpos;
			CurrentCoords = c;
			CurrentTileMap = RoomsA[CurrentCoords.X, CurrentCoords.Y].RoomMap;

			if (!(CurrentTileMap as Room).Visited) {
				GetNode<Timer>("RoomInitDelay").Start();
			}
		}
	}

	private void OnRoomDelayTimeout() {
		ProcessRoomMarkers(CurrentTileMap);
	}
}
