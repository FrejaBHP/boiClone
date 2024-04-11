using Godot;
using System;

public partial class EntityExplosion : Node2D {
	public int Radius { get; set; }
	public float Damage { get; set; }
	public float Knockback { get; set; }

	private Area2D explosionArea;
	private CircleShape2D shape;

	public override void _Ready() {
		explosionArea = GetNode<Area2D>("ExplosionRadius");

		shape = explosionArea.GetChild<CollisionShape2D>(0).Shape as CircleShape2D;
		shape.Radius = Radius;
		
		Explode();

		GetNode<AnimatedSprite2D>("ExplosionSprites").Play("explosion");
		GetNode<Timer>("ExplosionTimer").Start();
	}

	private async void Explode() {
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame); // why
		
        Godot.Collections.Array<Node2D> bodies = explosionArea.GetOverlappingBodies();
		foreach (Node2D node in bodies) {
			if (node.IsInGroup("Enemy") && node.HasNode("EnemyHealthComponent")) {
				EnemyHealthComponent enemyHealth = node.GetNode<EnemyHealthComponent>("EnemyHealthComponent");
				enemyHealth.TakeDamage(Damage);
			}
			else if (node.IsInGroup("Player")) {
				Main.ProcessPlayerDamage(2);
			}
			else {
                float distance = GlobalPosition.DistanceTo(node.GlobalPosition);
				float knockbackFactor = 1 - ((distance * 0.75f) / shape.Radius); // 0.75f = 100% -> ~25% knockback depending on distance
				Vector2 force = (node.GlobalTransform.Origin - GlobalTransform.Origin).Normalized() * (Knockback * knockbackFactor);
				//Vector2 reverseforce = (GlobalTransform.Origin - node.GlobalTransform.Origin).Normalized() * (Knockback * knockbackFactor);
				
				switch (node) {
					case Pickup:
						Pickup pickup = node as Pickup;
						pickup.ApplyCentralImpulse(force);
						break;
					
					case EntityBomb:
						EntityBomb bomb = node as EntityBomb;
						if (bomb.Freeze) {
							bomb.Set(EntityBomb.PropertyName.Freeze, false);
						}
						bomb.ApplyCentralImpulse(force);
						break;

					default:
						break;
				}
			}
		}
		World.CurrentRoom.BlowUpObstacleTiles(Position / 2, Radius / 2);
	}

	private void OnExplosionTimerTimeout() {
		QueueFree();
	}
}
