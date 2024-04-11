using Godot;
using System;

[Tool]
public partial class EnemyMovementComponent : Node2D {
	private float speedMult;
	[Export]
	public float SpeedMult { 
		get => speedMult; 
		set => speedMult = value; 
	}

	private float baseSpeed = 208f;

	public Enemy CompOwner { get; private set; }

    public override void _Ready() {
        CompOwner = (Enemy)GetParent();
    }

    public void Move(Vector2 dir) {
		if (dir != Vector2.Zero) {
			CompOwner.Velocity = dir * (baseSpeed * SpeedMult);
			CompOwner.MoveAndSlide();
		}
	}

	public void MoveTowardsPlayer() {
		if (Main.Player.IsAlive) {
			Move(CompOwner.GetPlayerVectorNormalised());
		}
	}
}
