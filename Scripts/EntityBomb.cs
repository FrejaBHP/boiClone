using Godot;
using System;

public partial class EntityBomb : RigidBody2D {
	public override void _Ready() {
		GetNode<AnimatedSprite2D>("BombSprites").Play("bombFuse");
		GetNode<Timer>("BombTimer").Start();
	}

	private void OnPlayerExited(Node2D body) {
		if (body.IsInGroup("Player") && Freeze) {
			SetDeferred(PropertyName.Freeze, false);
		}
	}

	private void OnBombTimerTimeout() {
		EntityExplosion explosion = Main.entityExplosion.Instantiate<EntityExplosion>();
		explosion.Radius = 48;
		explosion.Damage = 100;
		explosion.Knockback = 1000;

		World.CurrentRoom.EntitiesNode.AddChild(explosion);
		explosion.GlobalPosition = GlobalPosition;
		
		QueueFree();
	}
}
