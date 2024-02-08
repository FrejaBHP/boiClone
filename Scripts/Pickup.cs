using Godot;
using System;
using System.Linq;

public partial class Pickup : RigidBody2D {
	protected int pType;
	protected int amount;
	
	public override void _Ready() {
		pType = (int)GetMeta("pickupType");
		amount = (int)GetMeta("amount");
	}

	// Pickups can be pushed through corners of the tilemap. This is not a pressing concern, but should be looked into eventually

	private void OnPlayerPickupRadiusEntered(Node2D body) {
		switch (pType) {
			case (int)PickupEType.RedHeart:
				if (Main.Player.HeartContainers.Count * 2 > Main.Player.GetRedHearts()) {
					Main.GivePickup(pType, amount);
					QueueFree();
				}
				break;
				
			case (int)PickupEType.BlueHeart:
				if ((Main.Player.HeartContainers.Count * 2) + Main.Player.GetOtherHearts() < 24) {
					Main.GivePickup(pType, amount);
					QueueFree();
				}
				break;
				
			case (int)PickupEType.BlackHeart:
				if ((Main.Player.HeartContainers.Count * 2) + Main.Player.GetOtherHearts() < 24 || Main.Player.LooseHearts.OfType<HeartBlue>().Any()) {
					Main.GivePickup(pType, amount);
					QueueFree();
				}
				break;
			
			default:
				Main.GivePickup(pType, amount);
				QueueFree();
				break;
		}
	}
}
