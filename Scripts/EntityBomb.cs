using Godot;
using System;

public partial class EntityBomb : Node2D {
	public override void _Ready() {
		GetNode<AnimatedSprite2D>("BombSprites").Play("bombFuse");
		GetNode<Timer>("BombTimer").Start();
	}

	private void OnBombTimerTimeout() {
		EntityExplosion explosion = Main.entityExplosion.Instantiate<EntityExplosion>();
		explosion.Radius = 48;
		explosion.Damage = 100;
		explosion.Knockback = 1000;
		explosion.GlobalPosition = GlobalPosition;

		GetNode<World>("/root/Main/World").AddChild(explosion);
		
		QueueFree();
	}
}
