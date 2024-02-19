using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public partial class World : Node {
	private static WorldRoom[,] worldRooms;
	private static Vector2I currentCoords;
	private static Room currentRoom;
	
	private static int enemiesLeft = 0;
	private static readonly int roomLength = 480;
	private static readonly int roomHeight = 288;
	private static readonly int gridSize = 50;
	private static readonly int gridGapExtra = 0; // Until better solution is implemented, this is used to keep other rooms out of sight
	private static readonly int gridGapX = roomLength + gridGapExtra; // Each room is 32x15=480 units long
	private static readonly int gridGapY = roomHeight + gridGapExtra; // Each room is 32x9=288 units tall
	private static readonly int skip = 80 + gridGapExtra; // About 80 units seems like a good base value for jumping between rooms

	private Timer EnemySpawnDelayTimer;

	protected PackedScene Room = GD.Load<PackedScene>("Scenes/room.tscn");
	protected PackedScene Player = GD.Load<PackedScene>("Scenes/player.tscn");
	protected PackedScene ItemPedestal = GD.Load<PackedScene>("Scenes/worldItem.tscn");

	public World() {
		worldRooms = new WorldRoom[gridSize, gridSize];
		currentCoords = currentCoords with { X = gridSize / 2, Y = gridSize / 2 };
	}

    public override void _Ready() {
		EnemySpawnDelayTimer = GetNode<Timer>("EnemySpawnDelay");
		BuildLevel();
	}


	#region Debug
	private static Random debugRND = new();

	public override void _Input(InputEvent @event) {
		if (@event.IsActionPressed("debugPickup")) {
			DEBUGRollForPickup();
		}
		else if (@event.IsActionPressed("debugChargeSingle")) {
			Main.Player.GainCharge(1);
		}
    }

	private void DEBUGRollForPickup() { // KP1
		int typeRoll = debugRND.Next(0, (int)PickupEType.AMOUNT);
		int pickupRoll = DeterminePickupRarity(typeRoll);

		Vector2 pickupPos;
		pickupPos.X = currentRoom.GlobalPosition.X + 240;
		pickupPos.Y = currentRoom.GlobalPosition.Y + 144;

		CreatePickup(pickupRoll, pickupPos);
	}
	#endregion


	#region Initialisation
	private void BuildLevel() {
		// Until coming across a better format, maps are kept track of in a 2D array for navigation
		// And since arrays don't like negative indeces, it's offset by 25 later (for a 50x50 total map size atm)
		// Coordinates here are however still written as starting at [0, 0] for ease of use

		List<WorldRoom> tempList = new() {
			new((int)RoomNames.StartingRoom, 0, 0),
			new((int)RoomNames.UDLR_BaseRoom, 1, 0),
			new((int)RoomNames.R_Room, -1, 0),
			new((int)RoomNames.UD_LongRoom, 0, 1),
			new((int)RoomNames.U_TestItemRoom, 0, 2),
			new((int)RoomNames.S_Room, 0, -1)
			//new((int)RoomNames.UDLR_BaseRoom, 0, -1)
		};
		
		PlaceRooms(tempList);
	}

	private void PlaceRooms(List<WorldRoom> worldRoomList) {
		foreach (WorldRoom worldRoom in worldRoomList) {
			if (RoomCollection.RoomDataSet[worldRoom.ID].Scene != null) {
				Room newRoom = Room.Instantiate() as Room;
				newRoom.Data = RoomCollection.RoomDataSet[worldRoom.ID];
				newRoom.Map = newRoom.Data.Scene.Instantiate() as TileMap;

				newRoom.AddChild(newRoom.Map);
				AddChild(newRoom);

				worldRoom.Room = newRoom;
				worldRooms[worldRoom.Coords.X + (gridSize / 2), worldRoom.Coords.Y + (gridSize / 2)] = worldRoom;

				newRoom.GlobalPosition = newRoom.GlobalPosition with { X = worldRoom.Coords.X * gridGapX, Y = worldRoom.Coords.Y * gridGapY };
			}
			else {
				GD.PushError($"Room with ID {worldRoom.ID} not found.");
			}
		}
		currentRoom = worldRooms[gridSize / 2, gridSize / 2].Room;
		currentRoom.Visible = true;

		CheckDirections();
		AddPlayer();
		currentRoom.CheckAndStartOpeningDoors();
	}

	private static void CheckDirections() {
		// Checks for empty spaces at adjacent coordinates, then removes and seals up extra doors to them. Only applies to starting room for the time being.
		if (worldRooms[currentCoords.X, currentCoords.Y - 1] == null) {
			currentRoom.RemoveDoor(0);
		}
		if (worldRooms[currentCoords.X + 1, currentCoords.Y] == null) {
			currentRoom.RemoveDoor(1);
		}
		if (worldRooms[currentCoords.X, currentCoords.Y + 1] == null) {
			currentRoom.RemoveDoor(2);
		}
		if (worldRooms[currentCoords.X - 1, currentCoords.Y] == null) {
			currentRoom.RemoveDoor(3);
		}
	}

	private void AddPlayer() {
		Player player = Player.Instantiate() as Player;
		AddChild(player);

		// Stats separated to allow for different kinds of characters later
		// Setting these values also sets the HUD, which is probably smart
		// Maybe should be moved somewhere else, however
		player.GiveHeartContainers(3, 6);

		player.Speed = 1f;
		player.ShotSpeed = 1f;
		player.Range = 6.5f;
		player.Damage = 3.5f;
		player.AttackDelay = 0f;
		player.Luck = 0f;

		player.Coins = 0;
		player.Bombs = 5;
		player.Keys = 0;

		player.GlobalPosition = player.GlobalPosition with { X = roomLength / 2, Y = roomHeight / 2 }; // Middle of the starting room
	}
	#endregion

	#region Markers
	private void ReadRoomMarkers() {
		CallDeferred(MethodName.ProcessItemMarkers); // To avoid runtime error, this call is deferred
		GD.Print($"Flags: {currentRoom.Data.Flags}");

		if (!currentRoom.Data.Flags.HasFlag(RoomFlags.HasCombat)) {
			RoomCleared(false);
		}
		else {
			ProcessEnemyMarkers();
		}
	}

	private void ProcessItemMarkers() {
		foreach (Marker2D marker in currentRoom.ItemMarkers) {
			CreateItemPedestal((int)marker.GetMeta("entityID"), marker.GlobalPosition);
		}
	}

	private void OnEnemySpawnDelayTimeout() {
		//ProcessEnemyMarkers();
	}

	private async void ProcessEnemyMarkers() {
		if (currentRoom.EnemyMarkers.Count > 0) {
			EnemySpawnDelayTimer.Start();
			await ToSignal(EnemySpawnDelayTimer, "timeout");

			foreach (Marker2D marker in currentRoom.EnemyMarkers) {
				CreateEnemy((int)marker.GetMeta("entityID"), marker.GlobalPosition);
				enemiesLeft++;
			}
		}
	}
	#endregion

	#region MarkerDataInstantiation
	private void CreateItemPedestal(int itemID, Vector2 pos) {
		WorldItem newPedestal = ItemPedestal.Instantiate() as WorldItem;
		Sprite2D itemSprite = newPedestal.GetNode<Sprite2D>("ItemSpriteItem");

		// If itemID is -1, it's interpreted as a random item spawn, otherwise it's a set spawn
		if (itemID == -1) {
			if (ItemCollection.ItemDataSet.Count != 0) {
				Random random = new();
				int rnd = random.Next(0, ItemCollection.ItemDataSet.Count);
				itemSprite.Texture = ItemCollection.ItemDataSet[rnd].Sprite;
				newPedestal.SetMeta("itemID", rnd);
			}
			else {
				GD.PushError($"No items found.");
			}
		}
		else {
			if (ItemCollection.ItemDataSet[itemID] != null) {
				itemSprite.Texture = ItemCollection.ItemDataSet[itemID].Sprite;
				newPedestal.SetMeta("itemID", itemID);
			}
			else {
				GD.PushError($"Item with ID {itemID} not found.");
			}
		}
		AddChild(newPedestal);
		newPedestal.GlobalPosition = pos;
	}

	private void CreateEnemy(int enemyID, Vector2 pos) {
		if (EnemyCollection.EnemyScenes[enemyID] != null) {
			Enemy newEnemy = EnemyCollection.EnemyScenes[enemyID].Instantiate() as Enemy;
			AddChild(newEnemy);
			newEnemy.GlobalPosition = pos;
		}
		else {
			GD.PushError($"Enemy with ID {enemyID} not found.");
		}
	}
	#endregion

	#region RoomAndPickupLogic
	private void RoomCleared(bool hadCombat) {
		currentRoom.CheckAndStartOpeningDoors();
		
		if (hadCombat) {
			CallDeferred(MethodName.RollForPickup); // To avoid runtime error, this call is deferred
			Main.Player.GainCharge(1);
		}
	}

	private void RollForPickup() {
		Random random = new();
		int dropRoll = random.Next(0, 4); // 25% chance

		if (dropRoll == 0) {
			int typeRoll = random.Next(0, (int)PickupEType.AMOUNT); // Coin, Bomb, Key, Red Heart
			int pickupRoll = DeterminePickupRarity(typeRoll);

			Vector2 pickupPos;
			pickupPos.X = currentRoom.GlobalPosition.X + (roomLength / 2);
			pickupPos.Y = currentRoom.GlobalPosition.Y + (roomHeight / 2);

			CreatePickup(pickupRoll, pickupPos);
		}
	}

	private int DeterminePickupRarity(int pType) {
		Random random = new(); // Placeholder luck
		int pID = 0;

		switch (pType) {
			case (int)PickupEType.Coin:
				int cRoll = random.Next(0, 20);
				if (cRoll == 19) {
					pID = (int)PickupEName.Coin10;
				}
				else if (cRoll >= 15) {
					pID = (int)PickupEName.Coin5;
				}
				else {
					pID = (int)PickupEName.Coin;
				}
				break;
			
			case (int)PickupEType.Bomb:
				pID = (int)PickupEName.Bomb;
				break;
			
			case (int)PickupEType.Key:
				pID = (int)PickupEName.Key;
				break;

			case (int)PickupEType.RedHeart:
				int hRoll = random.Next(0, 2);
				if (hRoll == 1) {
					pID = (int)PickupEName.RedHeartFull;
				}
				else if (hRoll == 0) {
					pID = (int)PickupEName.RedHeartHalf;
				}
				break;

			case (int)PickupEType.BlueHeart:
				int bRoll = random.Next(0, 2);
				if (bRoll == 1) {
					pID = (int)PickupEName.BlueHeartFull;
				}
				else if (bRoll == 0) {
					pID = (int)PickupEName.BlueHeartHalf;
				}
				break;
			
			case (int)PickupEType.BlackHeart:
				int blRoll = random.Next(0, 2);
				if (blRoll == 1) {
					pID = (int)PickupEName.BlackHeartFull;
				}
				else if (blRoll == 0) {
					pID = (int)PickupEName.BlackHeartHalf;
				}
				break;
		}
		return pID;
	}

	private void CreatePickup(int pickupID, Vector2 pos) {
		if (PickupCollection.PickupScenes[pickupID] != null) {
			Pickup newPickup = PickupCollection.PickupScenes[pickupID].Instantiate() as Pickup;
			AddChild(newPickup);
			newPickup.GlobalPosition = pos;
		}
		else {
			GD.PushError($"Pickup with ID {pickupID} not found.");
		}
	}

	public void MoveRooms(uint dir) {
		Vector2 pos = Main.Player.GlobalPosition;
		Vector2 cpos = Main.Camera.GlobalPosition;
		Vector2I c = currentCoords;

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
		}

		Main.Player.GlobalPosition = pos;
		Main.Camera.GlobalPosition = cpos;
		currentCoords = c;

		currentRoom.Visible = false;
		currentRoom = worldRooms[currentCoords.X, currentCoords.Y].Room;

		EnterNewRoom();
	}

	private void EnterNewRoom() {
		currentRoom.Visible = true;
		if (!currentRoom.Visited) {
			switch (currentRoom.Data.Type) {
				case RoomType.Normal:
					break;

				case RoomType.Treasure:
					break;

				case RoomType.Boss:
					break;
				
				case RoomType.Secret:
					break;
				
				case RoomType.Shop:
					break;
			}
			ReadRoomMarkers();
		}
	}
	#endregion

	public Enemy[] SelectAllEnemies() {
		return GetTree().GetNodesInGroup("Enemy").Cast<Enemy>().ToArray();
	}


	// SIGNAL HANDLING
	public void DecreaseEnemyCount() {
		//await ToSignal(typeof(EnemyHealthComponent), EnemyHealthComponent.SignalName.Died);
		enemiesLeft--;
		
		if (enemiesLeft == 0) {
			RoomCleared(true); // If combat occured, parameter is true, rolling for pickup
		}
	}
}
