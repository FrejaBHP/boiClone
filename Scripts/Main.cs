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
	private static World world;

	public static PackedScene entityExplosion = GD.Load<PackedScene>("Scenes/entityExplosion.tscn");
	
	public Main() {
		BasePlayerDamageTaken = 1;
	}

    public override void _Ready() {
		world = GetNode<World>("/root/Main/World");
    }

    public override void _EnterTree() {
		RoomCollection.CompileRoomList();
		EnemyCollection.CompileEnemyList();
		ItemCollection.CompileItemList();
		PickupCollection.CompilePickupList();
    }

	public static void ProcessPlayerDamage(int multiplier) {
		if (!Player.Invulnerable) {
			Player.TakeDamage(BasePlayerDamageTaken * multiplier, 0);
		}
	}

	public static void ProcessPlayerDamage(Enemy source, int multiplier) {
		if (!Player.Invulnerable) {
			Player.TakeDamage(BasePlayerDamageTaken * multiplier, 0);
		}
	}

	public static void DamageAllEnemies(float damage) {
        Enemy[] enemies = world.SelectAllEnemies();
		for (int i = 0; i < enemies.Length; i++) {
			if (enemies[i].HasNode("EnemyHealthComponent")) {
				EnemyHealthComponent enemyHealth = enemies[i].GetNode<EnemyHealthComponent>("EnemyHealthComponent");
				enemyHealth.TakeDamage(damage);
			}
		}
	}

	public static void GivePickup(int pickupType, int amount) {
		switch (pickupType) {
			case (int)PickupEType.Coin:
				if ((Player.Coins + amount) < 100) {
					Player.Coins += amount;
				}
				else {
					Player.Coins = 99;
				}
				break;

			case (int)PickupEType.Bomb:
				if ((Player.Bombs + amount) < 100) {
					Player.Bombs += amount;
				}
				else {
					Player.Bombs = 99;
				}
				break;

			case (int)PickupEType.Key:
				if ((Player.Keys + amount) < 100) {
					Player.Keys += amount;
				}
				else {
					Player.Keys = 99;
				}
				break;

			case (int)PickupEType.RedHeart:
				Player.GiveHeart(amount, HeartEType.RedHeart);
				break;

			case (int)PickupEType.BlueHeart:
				Player.GiveHeart(amount, HeartEType.BlueHeart);
				break;

			case (int)PickupEType.BlackHeart:
				Player.GiveHeart(amount, HeartEType.BlackHeart);
				break;
		}
	}

	public static void GiveItem(int itemID) {
		ItemData itemData = ItemCollection.ItemDataSet[itemID];
		if (itemData != null) {
			(ItemShowcase as NewItemShowcase).ShowNewItem(itemData);

			Item item = (Item)Activator.CreateInstance(itemData.Type);

            if (item is IActiveEffect a) {
                Player.ActiveItem = item;

				HUD.UpdateActiveItem(itemData.Sprite, a.Charge, a.MaxCharges);
            }
            else {
                Player.Inventory.Add(item);
            }

			if (item is IInstantEffect i) {
				i.OnPickedUp();
			}
		}
	}

	public static void RemoveItem() {
		/*
		IInstantEffect i = item as IInstantEffect;
		i?.OnRemoved();
		*/
	}
}
