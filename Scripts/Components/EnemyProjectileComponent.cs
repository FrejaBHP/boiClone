using Godot;
using System;

[Tool]
public partial class EnemyProjectileComponent : Node2D {
	[Export]
	public float Range { get; set; }
	[Export]
	public float ShotSpeed { get; set; }
	[Export]
	public float AttackTime { get; set; }
	[Export]
	public float AttackDelay { get; set; }
	[Export]
	public int ProjectilesPerAttack { get; set; }

	public bool CanAttack { get; set; }
	public Timer RefireTimer { get; private set; }
	public Timer DelayTimer { get; private set; }

    public override void _Ready() {
		RefireTimer = GetNode<Timer>("RefireTimer");
		RefireTimer.WaitTime = AttackTime;
		DelayTimer = GetNode<Timer>("DelayTimer");
		DelayTimer.WaitTime = AttackDelay;
    }

	public void GenericProjectileAttack(Enemy enemy, Vector2 direction) {
		Attack.PrepareProjectileAttack(enemy, this, direction);
	}
}
