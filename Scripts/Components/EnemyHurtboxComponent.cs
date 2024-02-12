using Godot;
using System;

[Tool]
public partial class EnemyHurtboxComponent : Area2D {
	[Export]
	private EnemyHealthComponent HealthComponent;
	
	private void OnAreaEntered(Area2D area) {
		if (area.IsInGroup("Projectile")) {
			Projectile proj = area as Projectile;
			HealthComponent.TakeDamage(proj.Damage);
		}
	}
}
