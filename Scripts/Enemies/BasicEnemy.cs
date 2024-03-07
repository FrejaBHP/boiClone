using Godot;

public partial class BasicEnemy : Enemy {
	[Export]
	private EnemyCollisionComponent CollisionComponent;
	[Export]
	private EnemyMovementComponent MovementComponent;

    public override void _Ready() {
		CountsTowardsEnemyCount = true;
    }

    public override void _Process(double delta) {
		MovementComponent.MoveTowardsPlayer(this);
		CollisionComponent.CheckBodyCollision(this);
    }
}
