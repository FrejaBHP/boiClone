using System;
using Godot;

public partial class ProjectileEnemy : Enemy {
	[Export]
	private EnemyCollisionComponent CollisionComponent;
	[Export]
	private EnemyMovementComponent MovementComponent;
    [Export]
    private EnemyProjectileComponent ProjectileComponent;

	readonly StateMachine stateMachine = new();
    private MovementState movementState;
    private AttackState attackState;

    private AnimatedSprite2D enemySprites;
    private readonly Random random = new();
    private int rnd;

	internal class MovementState : IState {
        readonly ProjectileEnemy owner;

		internal MovementState(ProjectileEnemy enemy) {
			owner = enemy;
		}

		public void Enter() {
			owner.enemySprites.Play("moving");
		}

		public void Execute() {
			owner.MovementComponent.MoveTowardsPlayer(owner);
			owner.CollisionComponent.CheckBodyCollision(owner);

			owner.enemySprites.FlipH = owner.GetIfFlipSpriteH(owner.GetPlayerVectorNormalised());

            if (owner.ProjectileComponent.CanAttack) {
                owner.RollToAttack();
                if (owner.rnd == 10) {
                    owner.PrepareAttack();
                }
            }
		}

		public void Exit() {
			owner.enemySprites.Stop();
		}
	}

    internal class AttackState : IState {
        readonly ProjectileEnemy owner;

		internal AttackState(ProjectileEnemy enemy) {
			owner = enemy;
		}

		public void Enter() {
			owner.enemySprites.Play("attacking");
            owner.ProjectileComponent.DelayTimer.Start();
		}

		public void Execute() {
			owner.CollisionComponent.CheckBodyCollision(owner);
            owner.enemySprites.FlipH = owner.GetIfFlipSpriteH(owner.GetPlayerVectorNormalised());
		}

		public void Exit() {
			owner.enemySprites.Stop();
            owner.ProjectileComponent.RefireTimer.Start();
            owner.ProjectileComponent.CanAttack = false;
		}
	}

    protected override void Setup() {
		enemySprites = GetNode<AnimatedSprite2D>("EnemySprites");
		CountsTowardsEnemyCount = true;

        ProjectileComponent.RefireTimer.Timeout += () => SetAttackReady();
        ProjectileComponent.DelayTimer.Timeout += () => Attack();

        movementState = new(this);
        attackState = new(this);
		
		stateMachine.ChangeToState(movementState);
        SetAttackReady();
    }

    public override void _Process(double delta) {
		stateMachine.Process();
    }

    private void SetAttackReady() {
        ProjectileComponent.CanAttack = true;
    }

    private void PrepareAttack() {
        stateMachine.ChangeToState(attackState);
    }

    private void Attack() {
        ProjectileComponent.GenericProjectileAttack(this, GetPlayerVectorNormalised());
        stateMachine.ChangeToState(movementState);
    }

    private void RollToAttack() {
        rnd = random.Next(11);
    }
}
