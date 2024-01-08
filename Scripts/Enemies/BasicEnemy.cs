using Godot;
using System;

public partial class BasicEnemy : Enemy {
	public BasicEnemy() {
		MaxHealth = 5; //20
		Health = MaxHealth;
		Speed = 0.5f;
		ShotSpeed = 0.5f;
		Range = 6.5f;
	}
}
