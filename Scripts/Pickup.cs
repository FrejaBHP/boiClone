using Godot;
using System;

public partial class Pickup : Area2D {
	protected int pType;
	protected int amount;
	
	public override void _Ready() {
		pType = (int)GetMeta("pickupType");
		amount = (int)GetMeta("amount");
	}

	private void OnPlayerEntered(Node2D body) { // Add rules for pickups that can't yet be picked up, like red hearts at full health
		if (body.IsInGroup("Player")) {
			if (pType != (int)PickupEType.RedHeart) {
				Main.GivePickup(pType, amount);
				QueueFree();
			}
			else if (Main.Player.HeartContainers.Count * 2 > Main.Player.GetRedHearts()) {
				Main.GivePickup(pType, amount);
				QueueFree();
			}
		}
	}
}
