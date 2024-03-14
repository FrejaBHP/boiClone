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

	public Vector2 PlayerNormVector { get; private set; }

	public static Vector2 GetPlayerVector(Vector2 enemyPos, Vector2 playerPos) {
		return playerPos - enemyPos;
	}

	public static Vector2 GetPlayerVectorNormalised(Enemy enemy) {
		Vector2 direction = GetPlayerVector(enemy.GlobalPosition, Main.PlayerPosition);
		direction = direction.Normalized();

		return direction;
	}

	public void Move(Enemy enemy, Vector2 dir) {
		if (dir != Vector2.Zero) {
			enemy.Velocity = dir * (baseSpeed * SpeedMult);
			enemy.MoveAndSlide();
		}
	}

	public void MoveTowardsPlayer(Enemy enemy) {
		if (Main.Player.IsAlive) {
			PlayerNormVector = GetPlayerVectorNormalised(enemy);
			Move(enemy, PlayerNormVector);
		}
	}
}
