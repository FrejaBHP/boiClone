using Godot;
using System;
using System.Linq;
using System.Reflection.Emit;

public partial class Main : Node {
	public static float BasePlayerDamageTaken { get; set; }
	public static Player Player { get; set; }
	public static Vector2 PlayerPosition { get; set; }
	public static Camera2D Camera { get; set; }
	
	// Called when the node enters the scene tree for the first time.
	public Main() {
		BasePlayerDamageTaken = 0.5f;
	}

    public override void _Ready() {
        //HUD.HUDUpdateDamage((float)(Player.Damage * Math.Sqrt(Player.DamageBonus * 1.2 + 1) + Player.FlatDamageBonus));
		//HUD.HUDUpdateRate(Player.CalculateAttackRate());
    }

    public override void _EnterTree() {
        RoomCollection.CompileRoomList();
		EnemyCollection.CompileEnemyList();
		ItemCollection.CompileItemList();
		PickupCollection.CompilePickupList();
    }

	public static void ProcessDamage(Character source, Character target, float damage) {
		if (!target.Invulnerable) {
			GD.Print($"Health: {target.Health}, Damage taken: {damage}");
			target.ModifyHealth(-damage);
		}
	}

	public static void GivePickup(int pickupID, int amount) {
		switch (pickupID) {
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
		}
	}

	public static void GiveItem(int itemID) {
		Item item = (Item)Activator.CreateInstance(ItemCollection.ItemTypes[itemID]);
		Player.Inventory.Add(item);
		AddItemStats(item);

		GD.Print($"Picked up {item.ItemName}!");
	}

	public static void AddItemStats(Item item) {
		for (int i = 0; i < item.ItemBonuses.Count; i++) {
			if (item.ItemBonuses[i] != 0) {
				switch (i) {
					case 0:
						Player.DamageBonus += item.ItemBonuses[i];
						break;

					case 1:
						Player.HealthBonus += item.ItemBonuses[i];
						break;

					case 2:
						Player.RangeBonus += item.ItemBonuses[i];
						break;

					case 3:
						Player.ShotSpeedBonus += item.ItemBonuses[i];
						break;

					case 4:
						Player.SpeedBonus += item.ItemBonuses[i];
						break;

					case 5:
						Player.AttackDelayBonus += item.ItemBonuses[i];
						break;

					case 6:
						Player.FlatDamageBonus += item.ItemBonuses[i];
						break;

					case 7:
						Player.FlatAttackRateBonus += item.ItemBonuses[i];
						break;

					default:
						break;
				}
			}
		}
	}

	public static void RemoveItemStats(Item item) {
		for (int i = 0; i < item.ItemBonuses.Count; i++) {
			if (item.ItemBonuses[i] != 0) {
				switch (i) {
					case 0:
						Player.DamageBonus -= item.ItemBonuses[i];
						break;

					case 1:
						Player.HealthBonus -= item.ItemBonuses[i];
						break;

					case 2:
						Player.RangeBonus -= item.ItemBonuses[i];
						break;

					case 3:
						Player.ShotSpeedBonus -= item.ItemBonuses[i];
						break;

					case 4:
						Player.SpeedBonus -= item.ItemBonuses[i];
						break;

					case 5:
						Player.AttackDelayBonus -= item.ItemBonuses[i];
						break;

					case 6:
						Player.FlatDamageBonus -= item.ItemBonuses[i];
						break;

					case 7:
						Player.FlatAttackRateBonus -= item.ItemBonuses[i];
						break;
						
					default:
						break;
				}
			}
		}
	}
}
