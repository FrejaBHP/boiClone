using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public partial class World : Node {
	public static Room PreviousRoom { get; private set; }
	public static Room CurrentRoom { get; private set; }

	private static WorldRoom[,] worldRooms;
	private static Vector2I currentCoords;
	private static Node2D roomsNode;
	
	private static int enemiesLeft = 0;

	private static readonly int roomLength = 480;
	private static readonly int roomHeight = 288;
	private static readonly int gridSize = 50;
	private static readonly int gridGapExtra = 0; // Until better solution is implemented, this is used to keep other rooms out of sight
	private static readonly int gridGapX = roomLength + gridGapExtra; // Each room is 32x15=480 units long
	private static readonly int gridGapY = roomHeight + gridGapExtra; // Each room is 32x9=288 units tall
	private static readonly int skip = 80 + gridGapExtra; // About 80 units seems like a good base value for jumping between rooms

	private static Timer EnemySpawnDelayTimer;
	
	protected PackedScene Player = GD.Load<PackedScene>("Scenes/player.tscn");
	protected PackedScene ItemPedestal = GD.Load<PackedScene>("Scenes/itemPedestal.tscn");

	public World() {
		worldRooms = new WorldRoom[gridSize, gridSize];
		currentCoords = currentCoords with { X = gridSize / 2, Y = gridSize / 2 };
	}

    public override void _Ready() {
		EnemySpawnDelayTimer = GetNode<Timer>("EnemySpawnDelay");
		roomsNode = GetNode<Node2D>("Rooms");
		GenerateRooms();
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
		pickupPos.X = CurrentRoom.GlobalPosition.X + 240;
		pickupPos.Y = CurrentRoom.GlobalPosition.Y + 144;

		CreatePickup(pickupRoll, pickupPos);
	}
	#endregion


	#region Initialisation
	private void GenerateRooms() {
		List<WorldRoom> roomList = new() {
			new(0, 0, 0),
			new(1, 0, 1),
			new(2, -1, 0),
			new(3, -1, -1),
			new(4, -2, -1),
			new(5, 1, 0)
		};

		PlaceRooms(roomList);
	}

	private void PlaceRooms(List<WorldRoom> roomList) {
		foreach (WorldRoom worldRoom in roomList) {
			if (RoomCollection.RoomData[worldRoom.ID] != null) {
				Room newRoom = RoomCollection.RoomData[worldRoom.ID].Scene.Instantiate() as Room;
				newRoom.RoomID = worldRoom.ID;

				roomsNode.AddChild(newRoom);

				worldRoom.Room = newRoom;
				worldRooms[worldRoom.Coords.X + (gridSize / 2), worldRoom.Coords.Y + (gridSize / 2)] = worldRoom;

				newRoom.GlobalPosition = newRoom.GlobalPosition with { X = worldRoom.Coords.X * gridGapX, Y = worldRoom.Coords.Y * gridGapY };
			}
			else {
				GD.PushError($"Room with ID {worldRoom.ID} not found.");
			}
		}
		CurrentRoom = worldRooms[gridSize / 2, gridSize / 2].Room;
		PreviousRoom = CurrentRoom; // To avoid possible future problems
		CurrentRoom.Visible = true;

        RoomPass();

		AddPlayer();
		HUD.ShowHUD();
		CurrentRoom.CheckAndStartOpeningDoors();
	}

	private static void RoomPass() {
		WorldRoom wrNorth;
		WorldRoom wrEast;
		WorldRoom wrSouth;
		WorldRoom wrWest;

		foreach (WorldRoom wr in worldRooms) {
			if (wr != null) {
				wrNorth = worldRooms[wr.Coords.X + (gridSize / 2), wr.Coords.Y + (gridSize / 2) - 1];
				wrEast = worldRooms[wr.Coords.X + (gridSize / 2) + 1, wr.Coords.Y + (gridSize / 2)];
				wrSouth = worldRooms[wr.Coords.X + (gridSize / 2), wr.Coords.Y + (gridSize / 2) + 1];
				wrWest = worldRooms[wr.Coords.X + (gridSize / 2) - 1, wr.Coords.Y + (gridSize / 2)];

				// Removes superfluous doors from Flex rooms
				if ((int)wr.Room.Exits == 15) {
					if (wr.Coords.Y == -(gridSize / 2) - 1 || wrNorth == null) {
						wr.Room.RemoveDoor(0);
					}
					if (wr.Coords.X == (gridSize / 2) || wrEast == null) {
						wr.Room.RemoveDoor(1);
					}
					if (wr.Coords.Y == (gridSize / 2) || wrSouth == null) {
						wr.Room.RemoveDoor(2);
					}
					if (wr.Coords.Y == -(gridSize / 2) - 1 || wrWest == null) {
						wr.Room.RemoveDoor(3);
					}
				}

				// Removes ordinary door tiles from exits towards secret rooms and replaces them with breakable walls
				if (wrNorth != null && wr.Room.Exits.HasFlag(Exits.North) && wrNorth.Room.Type == RoomType.Secret) {
					wr.Room.ReplaceDoorWithSecretDoor(0);
				}
				if (wrEast != null && wr.Room.Exits.HasFlag(Exits.East) && wrEast.Room.Type == RoomType.Secret) {
					wr.Room.ReplaceDoorWithSecretDoor(1);
				}
				if (wrSouth != null && wr.Room.Exits.HasFlag(Exits.South) && wrSouth.Room.Type == RoomType.Secret) {
					wr.Room.ReplaceDoorWithSecretDoor(2);
				}
				if (wrWest != null && wr.Room.Exits.HasFlag(Exits.West) && wrWest.Room.Type == RoomType.Secret) {
					wr.Room.ReplaceDoorWithSecretDoor(3);
				}
			}
		}
	}

	private void AddPlayer() {
		Player player = Player.Instantiate() as Player;
		AddChild(player);

		// Stats separated to allow for different kinds of characters later
		// Setting these values also sets the HUD, which is probably smart
		// Maybe should be moved somewhere else, however
		player.GiveHeartContainers(3, 6);

		player.TryOverrideAttackType(AttackType.Projectile);
		//player.AttackFlags |= AttackFlags.Spectral;
		//player.AttackFlags |= AttackFlags.Piercing;
		//player.AttackFlags |= AttackFlags.Ring;
		//player.AttackFlags |= AttackFlags.BeamFixed;
		player.AmountPerAttack = 1;
		player.AttackWidth = 24f;

		player.Speed = 1f;
		player.ShotSpeed = 1f;
		player.Range = 6.5f;
		player.Damage = 3.5f;
		player.AttackDelay = 0f;
		player.Luck = 0f;

		//player.DamageMultiplier *= 1.2f;
		//player.AttackRateMultiplier /= 3f;

		player.Coins = 0;
		player.Bombs = 5;
		player.Keys = 0;

		player.GlobalPosition = player.GlobalPosition with { X = roomLength / 2, Y = roomHeight / 2 }; // Middle of the starting room
	}
	#endregion

	#region Markers
	private void ReadRoomMarkers() {
		CallDeferred(MethodName.ProcessItemMarkers); // To avoid runtime error, this call is deferred
		//GD.Print($"Flags: {CurrentRoom.Data.Flags}");

		if (!CurrentRoom.Flags.HasFlag(RoomFlags.HasCombat)) {
			RoomCleared();
		}
		else {
			ProcessEnemyMarkers();
		}
	}

	private void ProcessItemMarkers() {
		foreach (Marker2D marker in CurrentRoom.ItemMarkers) {
			CreateItemPedestal((int)marker.GetMeta("entityID"), marker.GlobalPosition);
		}
	}

	private void OnEnemySpawnDelayTimeout() {
		//ProcessEnemyMarkers();
	}

	private async void ProcessEnemyMarkers() {
		if (CurrentRoom.EnemyMarkers.Count > 0) {
			EnemySpawnDelayTimer.Start();
			await ToSignal(EnemySpawnDelayTimer, "timeout");

			foreach (Marker2D marker in CurrentRoom.EnemyMarkers) {
				CreateEnemy((int)marker.GetMeta("entityID"), marker.GlobalPosition);
			}
		}
	}
	#endregion

	#region MarkerDataInstantiation
	private void CreateItemPedestal(int itemID, Vector2 pos) {
		ItemPedestal newPedestal = ItemPedestal.Instantiate() as ItemPedestal;
		Sprite2D itemSprite = newPedestal.GetNode<Sprite2D>("ItemSpriteItem");

		// If itemID is -1, it's interpreted as a random item spawn, otherwise it's a set spawn
		if (itemID == -1) {
			if (ItemCollection.ItemDataSet.Count != 0) {
				Random random = new();
				int rnd = random.Next(0, ItemCollection.ItemDataSet.Count);

				newPedestal.SetItemSprite(rnd);
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
		CurrentRoom.PedestalsNode.AddChild(newPedestal);
		newPedestal.GlobalPosition = pos;
	}

	private void CreateEnemy(int enemyID, Vector2 pos) {
		if (EnemyCollection.EnemyScenes[enemyID] != null) {
			Enemy newEnemy = EnemyCollection.EnemyScenes[enemyID].Instantiate() as Enemy;
			CurrentRoom.EnemiesNode.AddChild(newEnemy);
			newEnemy.GlobalPosition = pos;

			if (newEnemy.CountsTowardsEnemyCount) {
				enemiesLeft++;
			}
		}
		else {
			GD.PushError($"Enemy with ID {enemyID} not found.");
		}
	}
	#endregion

	#region RoomAndPickupLogic
	private void RoomCleared() {
		CurrentRoom.CheckAndStartOpeningDoors();
		
		if (CurrentRoom.Flags.HasFlag(RoomFlags.HasCombat)) {
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
			pickupPos.X = CurrentRoom.GlobalPosition.X + (roomLength / 2);
			pickupPos.Y = CurrentRoom.GlobalPosition.Y + (roomHeight / 2);

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

	public void MoveRooms(int dir) {
		CurrentRoom.RemoveEmptyPedestals();

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

		PreviousRoom = CurrentRoom;
		CurrentRoom = worldRooms[currentCoords.X, currentCoords.Y].Room;

		EnterRoom();

		if (!CurrentRoom.Visited) {
			EnterNewRoom(dir); // Entering a room for the first time
		}
	}

	private void EnterRoom() {
		PreviousRoom.Visible = false;
		CurrentRoom.Visible = true;

		switch (CurrentRoom.Type) {
			case RoomType.Default:
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
	}

	private void EnterNewRoom(int dir) {
		enemiesLeft = 0;

		switch (CurrentRoom.Type) {
			case RoomType.Default:
				break;

			case RoomType.Treasure:
				break;

			case RoomType.Boss:
				break;
				
			case RoomType.Secret:
				int dirFrom = 0;

				if (dir == 0) {
					dirFrom = 2;
				}
				else if (dir == 1) {
					dirFrom = 3;
				}
				else if (dir == 2) {
					dirFrom = 0;
				}
				else if (dir == 3) {
					dirFrom = 1;
				}

				CurrentRoom.OpenSecretDoor(dirFrom);
				break;
				
			case RoomType.Shop:
				break;
		}
		ReadRoomMarkers();
	}
	#endregion

	public Enemy[] SelectAllEnemies() {
		return GetTree().GetNodesInGroup("Enemy").Cast<Enemy>().ToArray();
	}

	// SIGNAL HANDLING
	public void DecreaseEnemyCount() {
		enemiesLeft--;
		
		if (enemiesLeft == 0) {
			RoomCleared();
		}
	}

	public static void RemoveAssociatedProjectiles(CharacterBody2D owner) {
		Node[] projectileNodes = CurrentRoom.ProjectilesNode.GetChildren().ToArray();
		for (int i = 0; i < projectileNodes.Length; i++) {
			if (projectileNodes[i].IsInGroup($"{owner.Name}Proj")) {
				projectileNodes[i].Free();
			}
		}
	}

	public static void RemoveAssociatedBeams(CharacterBody2D owner) {
		Node[] projectileNodes = CurrentRoom.ProjectilesNode.GetChildren().ToArray();
		for (int i = 0; i < projectileNodes.Length; i++) {
			if (projectileNodes[i].IsInGroup($"{owner.Name}Beam")) {
				projectileNodes[i].Free();
			}
		}
	}
}
