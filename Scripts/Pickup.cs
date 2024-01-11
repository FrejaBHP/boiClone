using Godot;
using System;

public partial class Pickup : Area2D {
	protected int pickupID;
	protected int amount;
	
	public override void _Ready() {
		pickupID = (int)GetMeta("pickupID");
		amount = (int)GetMeta("amount");
	}

	private void OnPlayerEntered(Node2D body) {
		if (body.IsInGroup("Player")) {
			Main.GivePickup(pickupID, amount);
			QueueFree();
		}
	}
}
