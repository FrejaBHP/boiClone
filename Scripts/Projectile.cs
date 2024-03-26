using Godot;
using System;
using System.Collections;

public partial class Projectile : Area2D {
	public float Speed { get; set; }
	public float Range { get; set; }
	public float Damage { get; set; }
	public float Knockback { get; set; }
	public float Radius { get; set; }

	public bool Piercing { get; private set; }

	public float Lifetime { get; set; }

	public override void _PhysicsProcess(double delta) {
		// Destroys projectile after range limit
		if (Lifetime > 0) {
			GlobalPosition -= GlobalTransform.Y * (float)(Speed * delta);
			Lifetime -= (float)delta;
		}
		else {
			QueueFree();
		}
	}

	public void SetPiercing(bool piercing) {
		Piercing = piercing;
	}

	public void SetSpectral(bool spectral) {
		SetCollisionMaskValue((int)ECollisionLayer.Obstacle, spectral);
	}

	public void HurtsPlayer(bool value) {
		SetCollisionMaskValue((int)ECollisionLayer.Player, value);
	}

	public void HurtsEnemies(bool value) {
		SetCollisionMaskValue((int)ECollisionLayer.Enemy, value);
	}

	public void OnBodyEntered(Node2D body) {
		if (body.IsInGroup("Player")) {
			Main.ProcessPlayerDamage(1);
			QueueFree();
		}

		if (body.IsInGroup("Enemy")) {
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
		if (area.IsInGroup("Door")) {
			QueueFree();
		}
	}
}
