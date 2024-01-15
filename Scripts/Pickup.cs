using Godot;
using System;

public partial class Pickup : Area2D {
	protected int pType;
	protected int amount;
	
	public override void _Ready() {
		pType = (int)GetMeta("pickupType");
		amount = (int)GetMeta("amount");
	}

	private void OnPlayerEntered(Node2D body) {
		if (body.IsInGroup("Player")) {
			Main.GivePickup(pType, amount);
			QueueFree();
		}
	}
}
