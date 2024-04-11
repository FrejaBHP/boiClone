using Godot;
using System;
using System.Collections;
using System.Threading.Tasks;

public partial class Beam : Node2D {
	public float Range { get; set; }
	public float Damage { get; set; }
	public float Knockback { get; set; }
	public float Ticks { get; set; }
	public bool Fixed { get; set; }
	public bool Static { get; set; }

	private float remainingTicks;

	private Area2D beamArea;
	private RayCast2D wallRayCast;
	private Vector2 rayCastPoint;
	private Vector2 rayDiffMid;
	private Sprite2D beamMiddleSprite;
	private RectangleShape2D beamHurtbox;
	private Vector2 beamAngle;
	private float beamLength;
	private int beamDamageCounter = 0;

	private bool isDrawn = false;

	//private int test = 0;

	private EnemyHealthComponent enemyHealthComp;

    public override void _Ready() {
		SetReferences();
		remainingTicks = Ticks; // Beam will last as long as it has damage left to deal
    }

	public Task SetReferences() {
        beamArea = GetNode<Area2D>("BeamArea");
		beamHurtbox = beamArea.GetChild<CollisionShape2D>(0).Shape as RectangleShape2D;
        wallRayCast = GetNode<RayCast2D>("WallRayCast");
		beamMiddleSprite = beamArea.GetNode<Sprite2D>("BeamMiddle");
        return Task.CompletedTask;
    }

    public override void _PhysicsProcess(double delta) {
		if (remainingTicks > 0) {
			GlobalPosition = Main.Player.GlobalPosition;

			if (!Fixed) {
				SetDynamicRangeBeamProperties();
			}
			else {
				SetFixedRangeBeamProperties();
			}

			if (!Static) {
				AdjustAndMoveBeam();
			}

			beamDamageCounter++;
			if (beamDamageCounter == 3) {
				beamDamageCounter = 0;
				GetOverlapsAndDealDamage();
			}
		}
		else {
			QueueFree();
		}
	}

	public void GetOverlapsAndDealDamage() {
		Godot.Collections.Array<Area2D> areas = beamArea.GetOverlappingAreas();
		//test++;
		//GD.Print(test);
		
		foreach (Area2D area in areas) {
			if (area.GetParent() != null && area.GetParent().GetNodeOrNull("EnemyHealthComponent") != null) {
				enemyHealthComp = area.GetParent().GetNode<EnemyHealthComponent>("EnemyHealthComponent");
				enemyHealthComp.TakeDamage(Damage);
			}
		}

		remainingTicks--;
	}

	private void SetStaticBeamProperties() {

	}

	private void SetFixedRangeBeamProperties() {
		wallRayCast.TargetPosition = beamAngle * Range;
		wallRayCast.ForceRaycastUpdate();

		if (wallRayCast.IsColliding()) {
			rayCastPoint = wallRayCast.GetCollisionPoint();
			beamLength = GlobalPosition.DistanceTo(rayCastPoint);
			rayDiffMid = (rayCastPoint - GlobalPosition) / 2;
		}
		else {
			beamLength = Range;
			rayDiffMid = wallRayCast.TargetPosition / 2;
		}
	}

	private void SetDynamicRangeBeamProperties() {
		wallRayCast.TargetPosition = beamAngle * 1000;
		wallRayCast.ForceRaycastUpdate();
		rayCastPoint = wallRayCast.GetCollisionPoint();
		beamLength = GlobalPosition.DistanceTo(rayCastPoint);
		rayDiffMid = (rayCastPoint - GlobalPosition) / 2;
	}

	private void AdjustAndMoveBeam() {
		beamHurtbox.Size = beamHurtbox.Size with { Y = beamLength };
		beamMiddleSprite.Scale = beamMiddleSprite.Scale with { Y = beamLength / 16 };
		beamArea.Position = rayDiffMid;

		if (!isDrawn) {
			beamMiddleSprite.Visible = true;
			isDrawn = true;
		}
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
