using Godot;

public partial class BasicEnemy : Enemy {
	[Export]
	private EnemyCollisionComponent CollisionComponent;
	[Export]
	private EnemyMovementComponent MovementComponent;

    protected override void Setup() {
		CountsTowardsEnemyCount = true;
    }

    public override void _Process(double delta) {
		MovementComponent.MoveTowardsPlayer(this);
		CollisionComponent.CheckBodyCollision(this);
    }
}
