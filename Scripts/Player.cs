using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : Character {
	#region Properties
	public bool IsAlive { get; set; }
	public float IFrameTime { get; set; }

	private float effectiveDamage;
	private float effectiveFireRate;
	private float refireDelay;

	#region Health
	private float health;
	public override float Health {
		get => health;
		set {
			health = value;
			HUD.HUDUpdateHealth(health, maxHealth + healthBonus);
		}
	}

	private float healthBonus = 0;
	public float HealthBonus {
		get => healthBonus;
		set {
			healthBonus = value;
			HUD.HUDUpdateHealth(health, maxHealth + healthBonus);
		}
	}

	private float maxHealth;
	public override float MaxHealth {
		get => maxHealth;
		set {
			maxHealth = value;
			HUD.HUDUpdateHealth(health, maxHealth + healthBonus);
		}
	}
	#endregion

	#region Pickups
	private int coins;
	public int Coins {
		get => coins;
		set {
			coins = value;
			HUD.HUDUpdateCoins(coins);
		}
	}

	private int bombs;
	public int Bombs {
		get => bombs;
		set {
			bombs = value;
			HUD.HUDUpdateBombs(bombs);
		}
	}

	private int keys;
	public int Keys {
		get => keys;
		set {
			keys = value;
			HUD.HUDUpdateKeys(keys);
		}
	}
	#endregion

	#region Stats
	private float speed;
	public override float Speed {
		get => speed;
		set {
			speed = value;
			HUD.HUDUpdateSpeed(speed + speedBonus);
		}
	}

	private float speedBonus = 0;
	public float SpeedBonus {
		get => speedBonus;
		set {
			speedBonus = value;
			HUD.HUDUpdateSpeed(speed + speedBonus);
		}
	}

	private float attackDelay;
	public override float AttackDelay {
		get => attackDelay;
		set {
			attackDelay = value;
			CalculateAttackRate();
			HUD.HUDUpdateRate(effectiveFireRate);
		} 
	}

	private float attackDelayBonus = 0;
	public float AttackDelayBonus {
		get => attackDelayBonus;
		set {
			attackDelayBonus = value;
			CalculateAttackRate();
			HUD.HUDUpdateRate(effectiveFireRate);
		} 
	}

	private float flatAttackRateBonus = 0;
	public float FlatAttackRateBonus {
		get => flatAttackRateBonus;
		set {
			flatAttackRateBonus = value;
			CalculateAttackRate();
			HUD.HUDUpdateRate(effectiveFireRate);
		} 
	}

	private float damage;
	public override float Damage {
		get => damage;
		set {
			damage = value;
			CalculateAttackDamage();
			HUD.HUDUpdateDamage(effectiveDamage);
		} 
	}

	private float damageBonus = 0;
	public float DamageBonus {
		get => damageBonus;
		set {
			damageBonus = value;
			CalculateAttackDamage();
			HUD.HUDUpdateDamage(effectiveDamage);
		} 
	}

	private float flatDamageBonus = 0;
	public float FlatDamageBonus {
		get => flatDamageBonus;
		set {
			flatDamageBonus = value;
			CalculateAttackDamage();
			HUD.HUDUpdateDamage(effectiveDamage);
		}
	}
	
	private float range;
	public override float Range {
		get => range;
		set {
			range = value;
			HUD.HUDUpdateRange(range + rangeBonus);
		}
	}

	private float rangeBonus = 0;
	public float RangeBonus {
		get => rangeBonus;
		set {
			rangeBonus = value;
			HUD.HUDUpdateRange(range + rangeBonus);
		}
	}

	private float shotSpeed;
    public override float ShotSpeed { 
		get => shotSpeed; 
		set { 
			shotSpeed = value; 
			HUD.HUDUpdateShotSpeed(shotSpeed + shotSpeedBonus);
		} 
	}

    private float shotSpeedBonus = 0;
	public float ShotSpeedBonus { 
		get => shotSpeedBonus; 
		set { 
			shotSpeedBonus = value; 
			HUD.HUDUpdateShotSpeed(shotSpeed + shotSpeedBonus);
		} 
	}
	#endregion

    public List<Item> Inventory { get; set; }
	#endregion

    public Player() {
		Main.Player = this;
		Inventory = new();
		IsAlive = true;
		IFrameTime = 1f;
		CanShoot = true;
	}

    public override void _Ready() {
		GetNode<Timer>("IFrameTimer").WaitTime = IFrameTime;
		GetNode<Timer>("RefireTimer").WaitTime = refireDelay;
		AddToGroup("Player");
    }

    public override void _PhysicsProcess(double delta) {
		Vector2 dir = Input.GetVector("left", "right", "up", "down");
		Move(dir);
		Main.PlayerPosition = GlobalPosition;
	}

    protected override void CheckBodyCollision() {
        for (int i = 0; i < GetSlideCollisionCount(); i++) {
			KinematicCollision2D col = GetSlideCollision(i);
			if (col.GetCollider().IsClass("StaticBody2D")) { //So far, only item pedestals are StaticBody2D objects, so no further checks are needed
				WorldItem pedestal = col.GetCollider() as WorldItem;
				pedestal.PlayerCollided();
			}
		}
    }

	private void UpdateStats(int statIndex, float amount) {
		/*
		switch (statIndex) {
			case 0:
				break;

			case 1:
				if (amount > 0) {
					Health += amount;
				}
				MaxHealth += amount;
				break;
			
			default:
				break;
		}
		*/
	}

	public override void _Input(InputEvent @event) {
		int shootingDir;
		if (@event.IsActionPressed("shootup")) {
			shootingDir = 0;
			ShootProjectile(shootingDir);
		}
		if (@event.IsActionPressed("shootright")) {
			shootingDir = 1;
			ShootProjectile(shootingDir);
		}
		if (@event.IsActionPressed("shootdown")) {
			shootingDir = 2;
			ShootProjectile(shootingDir);
		}
		if (@event.IsActionPressed("shootleft")) {
			shootingDir = 3;
			ShootProjectile(shootingDir);
		}
    }

    protected override void ShootProjectile(int dir) {
		if (CanShoot) {
			CanShoot = false;
			GetNode<Timer>("RefireTimer").Start();

			Projectile proj = Projectile.Instantiate() as Projectile;
			proj.SetProjectileProperties(
				shotSpeed + shotSpeedBonus,
				effectiveDamage,
				range + rangeBonus,
				0
			);
			GetNode<World>("/root/Main/World").AddChild(proj);

			Transform2D trans = Transform2D.Identity;
			trans.Origin = GlobalPosition;

			float rotation = (float)Math.PI * 0.5f * dir;
			trans.X.X = trans.Y.Y = Mathf.Cos(rotation);
			trans.X.Y = trans.Y.X = Mathf.Sin(rotation);
			trans.Y.X *= -1;

			proj.Transform = trans;
		}
	}

	protected override void RefireTimerTimeout() {
		GetNode<Timer>("RefireTimer").WaitTime = refireDelay;
		CanShoot = true;
		
		int shootingDir;
		if (Input.IsActionPressed("shootup")) {
			shootingDir = 0;
			ShootProjectile(shootingDir);
		}
		if (Input.IsActionPressed("shootright")) {
			shootingDir = 1;
			ShootProjectile(shootingDir);
		}
		if (Input.IsActionPressed("shootdown")) {
			shootingDir = 2;
			ShootProjectile(shootingDir);
		}
		if (Input.IsActionPressed("shootleft")) {
			shootingDir = 3;
			ShootProjectile(shootingDir);
		}
	}

    protected override void OnTakeDamage() {
        Invulnerable = true;
		GetNode<Timer>("IFrameTimer").Start();
    }

	private void IFrameTimerTimeout() {
		Invulnerable = false;
	}

	public void CalculateAttackDamage() {
		effectiveDamage = (float)(Damage * Math.Sqrt(DamageBonus * 1.2 + 1) + FlatDamageBonus);
	}

	public void CalculateAttackRate() {
		float effectiveAttackDelay = 16 - 6 * (float)Math.Sqrt(((AttackDelay + AttackDelayBonus) * 1.3) + 1);
		float effectiveAttackRate;

		if (5 > effectiveAttackDelay) {
			effectiveAttackRate = 5 + FlatAttackRateBonus;
		}
		else {
			effectiveAttackRate = 30 / (effectiveAttackDelay + 1) + FlatAttackRateBonus;
		}

		// Sets attack rate-related variables for other methods to use
		effectiveFireRate = effectiveAttackRate;
		refireDelay = 1f / effectiveFireRate;
	}
}
