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
		if (Lifetime > 0) {
			GlobalPosition += Transform.X * (float)(Speed * delta);
			Lifetime -= (float)delta;
		}
		else {
			QueueFree();
		}
	}

	public void OnBodyEntered(Node2D body) {
		if (body.IsInGroup("Player")) {
			Main.ProcessDamage((Character)body, Main.Player, Main.BasePlayerDamageTaken);
			QueueFree();
		}

		if (body.IsInGroup("Enemy")) {
			Main.ProcessDamage(Main.Player, (Character)body, Damage);

			if (!Piercing) {
				QueueFree();
			}
		}

		else {
			QueueFree();
		}
	}

	public void OnAreaEntered(Area2D area) {
		if (area.HasMeta("door")) {
			QueueFree();
		}
	}
}
