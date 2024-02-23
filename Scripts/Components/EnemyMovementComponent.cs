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

	public static Vector2 GetPlayerVector(Vector2 enemyPos, Vector2 playerPos) {
		return playerPos - enemyPos;
	}

	public void MoveTowardsPlayer(Enemy enemy) {
		if (Main.Player.IsAlive) {
			Vector2 direction = GetPlayerVector(enemy.GlobalPosition, Main.PlayerPosition);
			direction = direction.Normalized();

			Move(enemy, direction);
		}
	}

	public virtual void Move(Enemy enemy, Vector2 dir) {
		if (dir != Vector2.Zero) {
			enemy.Velocity = dir * (baseSpeed * SpeedMult);
			enemy.MoveAndSlide();
			//CheckTile();
		}
	}
}
