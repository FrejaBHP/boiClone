using Godot;
using System;

[Tool]
public partial class EnemyProjectileComponent : Node2D {
	[Export]
	public float Range { get; set; }
	[Export]
	public float ShotSpeed { get; set; }
	[Export]
	public float ProjectileDelay { get; set; }

	private bool canShoot = true;
	private Timer refireTimer;

    public override void _Ready() {
		refireTimer = GetNode<Timer>("RefireTimer");
    }
}
