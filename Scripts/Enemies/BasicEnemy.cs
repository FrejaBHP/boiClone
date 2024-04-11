using Godot;

public partial class BasicEnemy : Enemy {
	[Export]
	private EnemyCollisionComponent CollisionComponent;
	[Export]
	private EnemyMovementComponent MovementComponent;
	
	private AnimatedSprite2D enemySprites;
	readonly StateMachine stateMachine = new();

	internal class MovementState : IState {
        readonly BasicEnemy owner;

		internal MovementState(BasicEnemy enemy) {
			owner = enemy;
		}

		public void Enter() {
			owner.enemySprites.Play("moving");
		}

		public void Execute() {
			owner.MovementComponent.MoveTowardsPlayer();
		}

		public void Exit() {
			owner.enemySprites.Stop();
		}
	}

    protected override void Setup() {
		enemySprites = GetNode<AnimatedSprite2D>("EnemySprites");
		CountsTowardsEnemyCount = true;

		MovementState movementState = new(this);
		stateMachine.ChangeToState(movementState);
    }

    public override void _Process(double delta) {
		stateMachine.Process();
		CollisionComponent.CheckBodyCollision();
		enemySprites.FlipH = GetIfFlipSpriteH(GetPlayerVectorNormalised());
    }
}
