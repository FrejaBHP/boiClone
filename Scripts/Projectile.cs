using Godot;
using System;
using System.Collections;

public partial class Projectile : Area2D {
	public float Speed { get; set; }
	public float Range { get; set; }
	public float Damage { get; set; }
	public float Knockback { get; set; }
	public float Radius { get; set; }

	public bool Piercing { get; set; }
	public bool Spectral { get; set; }

	public float Lifetime { get; set; }

	public override void _PhysicsProcess(double delta) {
		// Destroys projectile after range limit
		if (Lifetime > 0) {
			GlobalPosition -= Transform.Y * (float)(Speed * delta);
			Lifetime -= (float)delta;
		}
		else {
			QueueFree();
		}
	}

	public void SetProjectileProperties(float speed, float damage, float range, int alignment) {
		Speed = 256 * speed;
		Damage = damage;
		Range = range;
		Lifetime = (Range * 32) / Speed;

		// Modifies projectile object scanning behaviour
		if (Spectral) {
			SetCollisionMaskValue((int)ECollisionLayer.Obstacle, false);
		}

		if (alignment == 0) {
			SetCollisionMaskValue((int)ECollisionLayer.Enemy, true);
		}
		if (alignment == 1) {
			SetCollisionMaskValue((int)ECollisionLayer.Player, true);
		}
	}

	public void OnBodyEntered(Node2D body) {
		if (body.IsInGroup("Player")) {
			//Main.ProcessPlayerDamage((Character)body, 1);
			QueueFree();
		}

		if (body.IsInGroup("Enemy")) {
			//Main.ProcessEnemyDamage(Main.Player, (Enemy)body, Damage);

			if (!Piercing) {
				QueueFree();
			}
		}

		else {
			QueueFree();
		}
	}

	public void OnAreaEntered(Area2D area) {
		if (area.IsInGroup("EnemyHurtbox") && !Piercing) {
			QueueFree();
		}
		
		// Prevents projectiles from exiting rooms through open doors
		if (area.HasMeta("door")) {
			QueueFree();
		}
	}
}
