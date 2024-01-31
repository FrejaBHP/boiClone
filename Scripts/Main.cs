using Godot;
using System;
using System.Linq;
using System.Reflection.Emit;

public partial class Main : Node {
	// Player Z-index = 100
	// Projectile Z-index = 50
	// Enemy Z-index = 20
	// Pickup Z-index = 10

	public static int BasePlayerDamageTaken { get; set; }
	public static Player Player { get; set; }
	public static Vector2 PlayerPosition { get; set; }
	public static Camera2D Camera { get; set; }
	public static CanvasLayer ItemShowcase { get; set; }
	
	// Called when the node enters the scene tree for the first time.
	public Main() {
		BasePlayerDamageTaken = 1;
	}

    public override void _Ready() {

    }

    public override void _EnterTree() {
        RoomCollection.CompileRoomList();
		EnemyCollection.CompileEnemyList();
		ItemCollection.CompileItemList();
		PickupCollection.CompilePickupList();
    }

	public static void ProcessEnemyDamage(Character source, Enemy target, float damage) {
		if (!target.Invulnerable) {
			GD.Print($"Health: {target.Health}, Damage taken: {damage}");
			target.ModifyHealth(-damage);
		}
	}

	public static void ProcessPlayerDamage(Character source) {
		if (!Player.Invulnerable) {
			//GD.Print($"Health: {Player.Health}, Damage taken: {BasePlayerDamageTaken * 1}");
			Player.TakeDamage(BasePlayerDamageTaken * 1, 0);
		}
	}

	public static void GivePickup(int pickupType, int amount) {
		switch (pickupType) {
			case 0:
				if ((Player.Coins + amount) < 100) {
					Player.Coins += amount;
				}
				else {
					Player.Coins = 99;
				}
				break;

			case 1:
				if ((Player.Bombs + amount) < 100) {
					Player.Bombs += amount;
				}
				else {
					Player.Bombs = 99;
				}
				break;

			case 2:
				if ((Player.Keys + amount) < 100) {
					Player.Keys += amount;
				}
				else {
					Player.Keys = 99;
				}
				break;

			case 3:
				//if (Player.HeartContainers.Count * 2 > Player.GetRedHearts()) {
					Player.GiveHeart(amount, HeartEType.RedHeart);
				//}
				break;

			case 4:
				//if ((Player.HeartContainers.Count + Player.LooseHearts.Count) < 13) {
					Player.GiveHeart(amount, HeartEType.BlueHeart);
				//}
				break;
		}
	}

	public static void GiveItem(int itemID) {
		if (ItemCollection.ItemDataSet[itemID] != null) {
			(ItemShowcase as NewItemShowcase).ShowNewItem(ItemCollection.ItemDataSet[itemID]);

			Item item = (Item)Activator.CreateInstance(ItemCollection.ItemDataSet[itemID].Type);
			Player.Inventory.Add(item);
			item.OnPickedUp();

			//GD.Print($"Picked up {ItemCollection.ItemDataSet[itemID].Name}! {ItemCollection.ItemDataSet[itemID].Description}");
		}
	}
}
