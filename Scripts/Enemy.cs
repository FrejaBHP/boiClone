using Godot;
using System;

public partial class Enemy : Character {
	public static Vector2 GetPlayerVector(Vector2 enemyPos, Vector2 playerPos) {
		return playerPos - enemyPos;
	}

    protected override void CheckBodyCollision() {
        for (int i = 0; i < GetSlideCollisionCount(); i++) {
			KinematicCollision2D col = GetSlideCollision(i);
			
			if (col.GetCollider().IsClass("CharacterBody2D")) {
				Character collider = (Character)col.GetCollider();
				
				if (collider.IsInGroup("Player")) {
					Main.ProcessPlayerDamage(this, 1);
				}
				/*
				else if (collider.IsInGroup("Enemy")) {
					Main.ProcessEnemyDamage(this, (Enemy)collider, Damage);
				}
				*/
			}
		}
    }

    public override void _Ready() {
        AddToGroup("Enemy");
    }

    public override void _PhysicsProcess(double delta) {
		if (Main.Player.IsAlive) {
			Vector2 direction = GetPlayerVector(Position, Main.PlayerPosition);
			direction = direction.Normalized();

			Move(direction);
		}
	}
}
