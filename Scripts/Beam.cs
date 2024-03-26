using Godot;
using System;
using System.Collections;
using System.Threading.Tasks;

public partial class Beam : Node2D {
	public float Range { get; set; }
	public float Damage { get; set; }
	public float Knockback { get; set; }
	public float Duration { get; set; }
	public bool Static { get; set; }

	private float remainingDuration;

	private Timer beamDamageTimer;
	private Area2D beamArea;
	private RayCast2D wallRayCast;
	private Vector2 rayCastPoint;
	private Vector2 rayMidPoint;
	private Sprite2D beamMiddleSprite;
	private RectangleShape2D beamHurtbox;
	private Vector2 beamAngle;
	private float beamLength;

	private EnemyHealthComponent enemyHealth;

    public override void _Ready() {
		remainingDuration = Duration;
		SetReferences();
		
		beamDamageTimer.Start();
    }

	public Task SetReferences() {
        beamArea = GetNode<Area2D>("BeamArea");
		beamHurtbox = beamArea.GetChild<CollisionShape2D>(0).Shape as RectangleShape2D;
		beamDamageTimer = GetNode<Timer>("BeamDamageTimer");
        wallRayCast = GetNode<RayCast2D>("WallRayCast");
		beamMiddleSprite = beamArea.GetNode<Sprite2D>("BeamMiddle");
        return Task.CompletedTask;
    }

    public override void _PhysicsProcess(double delta) {
		if (remainingDuration > 0) {
			remainingDuration -= (float)delta;
			GlobalPosition = Main.Player.GlobalPosition;
		}
		else {
			QueueFree();
		}

		if (Range == 0) {
			SetDynamicRangeBeamProperties();
		}
		else {
			SetFixedRangeBeamProperties();
		}

		if (!Static) {
			AdjustAndMoveBeam();
		}
	}

	public void OnBeamDamageTimerTimeout() {
		Godot.Collections.Array<Area2D> areas = beamArea.GetOverlappingAreas();
		
		foreach (Area2D area in areas) {
			if (area.GetParent() != null && area.GetParent().GetNodeOrNull("EnemyHealthComponent") != null) {
				enemyHealth = area.GetParent().GetNode<EnemyHealthComponent>("EnemyHealthComponent");
				enemyHealth.TakeDamage(Damage);
			}
		}
	}

	private void SetStaticBeamProperties() {

	}

	private void SetFixedRangeBeamProperties() {
		wallRayCast.TargetPosition = beamAngle * Range;

		wallRayCast.ForceRaycastUpdate();
		rayCastPoint = wallRayCast.GetCollisionPoint();

		if (rayCastPoint != Vector2.Zero) {
			beamLength = GlobalPosition.DistanceTo(rayCastPoint);
		}
		else {
			beamLength = Range;
		}
		
		rayMidPoint = (rayCastPoint - GlobalPosition) / 2;
	}

	private void SetDynamicRangeBeamProperties() {
		wallRayCast.TargetPosition = beamAngle * 1000;
		wallRayCast.ForceRaycastUpdate();
		rayCastPoint = wallRayCast.GetCollisionPoint();
		beamLength = GlobalPosition.DistanceTo(rayCastPoint);
		rayMidPoint = (rayCastPoint - GlobalPosition) / 2;
	}

	private void AdjustAndMoveBeam() {
		beamHurtbox.Size = beamHurtbox.Size with { Y = beamLength };
		beamArea.Position = rayMidPoint;
		beamMiddleSprite.Scale = beamMiddleSprite.Scale with { Y = beamLength / 16 };
	}

	public void SetAngle(Vector2 direction) {
		beamAngle = direction.Rotated(-0.5f * (float)Math.PI);
		beamArea.Rotation = direction.Angle();
	}

	public void SetWidth(float width) {
		beamHurtbox.Size = beamHurtbox.Size with { X = width };
		beamMiddleSprite.Scale = beamMiddleSprite.Scale with { X = width / 16 };
	}

	public void HurtsPlayer(bool value) {
		beamArea.SetCollisionMaskValue((int)ECollisionLayer.Player, value);
	}

	public void HurtsEnemies(bool value) {
		beamArea.SetCollisionMaskValue((int)ECollisionLayer.Enemy, value);
	}
}
