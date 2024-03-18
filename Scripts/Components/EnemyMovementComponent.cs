using Godot;
using System;

[Tool]
public partial class EnemyMovementComponent : Node2D {
	private float baseSpeed = 208f;
	private float speedMult;
	[Export]
	public float SpeedMult { 
		get => speedMult; 
		set => speedMult = value; 
	}

	public void Move(Enemy enemy, Vector2 dir) {
		if (dir != Vector2.Zero) {
			enemy.Velocity = dir * (baseSpeed * SpeedMult);
			enemy.MoveAndSlide();
		}
	}

	public void MoveTowardsPlayer(Enemy enemy) {
		if (Main.Player.IsAlive) {
			Move(enemy, enemy.GetPlayerVectorNormalised());
		}
	}
}
