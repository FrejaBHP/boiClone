using Godot;
using System;
using System.ComponentModel.DataAnnotations.Schema;

public partial class BasicEnemy : Enemy {
	[Export]
	private EnemyCollisionComponent CollisionComponent;
	public BasicEnemy() {
		Speed = 0.5f;
	}
}
