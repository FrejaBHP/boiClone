using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : Character {
	#region Properties
	public bool IsAlive { get; set; }
	public float IFrameTime { get; set; }

	private Timer RefireTimer;
	private Timer IFrameTimer;

	private float effectiveDamage;
	private float effectiveFireRate;
	private float refireDelay;

	#region Health
	public List<HeartContainer> HeartContainers { get; set; } = new();
	public List<HeartBase> LooseHearts { get; set; } = new();
	#endregion

	#region Pickups
	private int coins;
	public int Coins {
		get => coins;
		set {
			coins = value;
			HUD.UpdateCoins(coins);
		}
	}

	private int bombs;
	public int Bombs {
		get => bombs;
		set {
			bombs = value;
			HUD.UpdateBombs(bombs);
		}
	}

	private int keys;
	public int Keys {
		get => keys;
		set {
			keys = value;
			HUD.UpdateKeys(keys);
		}
	}
	#endregion

	#region Stats
	private float speed;
	public override float Speed {
		get => speed;
		set {
			speed = value;
			HUD.UpdateSpeed(speed + speedBonus);
		}
	}

	private float speedBonus = 0;
	public float SpeedBonus {
		get => speedBonus;
		set {
			speedBonus = value;
			HUD.UpdateSpeed(speed + speedBonus);
		}
	}

	private float attackDelay;
	public override float AttackDelay {
		get => attackDelay;
		set {
			attackDelay = value;
			CalculateAttackRate();
			HUD.UpdateRate(effectiveFireRate);
		} 
	}

	private float attackDelayBonus = 0;
	public float AttackDelayBonus {
		get => attackDelayBonus;
		set {
			attackDelayBonus = value;
			CalculateAttackRate();
			HUD.UpdateRate(effectiveFireRate);
		} 
	}

	private float flatAttackRateBonus = 0;
	public float FlatAttackRateBonus {
		get => flatAttackRateBonus;
		set {
			flatAttackRateBonus = value;
			CalculateAttackRate();
			HUD.UpdateRate(effectiveFireRate);
		} 
	}

	private float damage;
	public override float Damage {
		get => damage;
		set {
			damage = value;
			CalculateAttackDamage();
			HUD.UpdateDamage(effectiveDamage);
		} 
	}

	private float damageBonus = 0;
	public float DamageBonus {
		get => damageBonus;
		set {
			damageBonus = value;
			CalculateAttackDamage();
			HUD.UpdateDamage(effectiveDamage);
		} 
	}

	private float flatDamageBonus = 0;
	public float FlatDamageBonus {
		get => flatDamageBonus;
		set {
			flatDamageBonus = value;
			CalculateAttackDamage();
			HUD.UpdateDamage(effectiveDamage);
		}
	}
	
	private float range;
	public override float Range {
		get => range;
		set {
			range = value;
			HUD.UpdateRange(range + rangeBonus);
		}
	}

	private float rangeBonus = 0;
	public float RangeBonus {
		get => rangeBonus;
		set {
			rangeBonus = value;
			HUD.UpdateRange(range + rangeBonus);
		}
	}

	private float shotSpeed;
    public override float ShotSpeed { 
		get => shotSpeed; 
		set { 
			shotSpeed = value; 
			HUD.UpdateShotSpeed(shotSpeed + shotSpeedBonus);
		} 
	}

    private float shotSpeedBonus = 0;
	public float ShotSpeedBonus { 
		get => shotSpeedBonus; 
		set { 
			shotSpeedBonus = value; 
			HUD.UpdateShotSpeed(shotSpeed + shotSpeedBonus);
		} 
	}

	private float luck;
    public float Luck { 
		get => luck; 
		set { 
			luck = value; 
			HUD.UpdateLuck(luck + luckBonus);
		} 
	}

    private float luckBonus = 0;
	public float LuckBonus { 
		get => luckBonus; 
		set { 
			luckBonus = value; 
			HUD.UpdateLuck(luck + luckBonus);
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
		RefireTimer = GetNode<Timer>("RefireTimer");
		IFrameTimer = GetNode<Timer>("IFrameTimer");
		
		IFrameTimer.WaitTime = IFrameTime;

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
			if (col.GetCollider().IsClass("StaticBody2D")) { // So far, only item pedestals are StaticBody2D objects, so no further checks are needed
				WorldItem pedestal = col.GetCollider() as WorldItem;
				pedestal.PlayerCollided();
			}
		}
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
			RefireTimer.Start();

			Projectile proj = Projectile.Instantiate() as Projectile;
			proj.SetProjectileProperties(
				shotSpeed + shotSpeedBonus, 	// Shot Speed
				effectiveDamage, 				// Damage for the projectile to deal
				range + rangeBonus, 			// Projectile range
				0 								// 0 sets flag for hitting enemies
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

	public void GiveHeartContainers(int n, int halves) {
		for (int i = 0; i < n; i++) {
			if (halves >= 2) {
				HeartContainers.Add(new(2));
				halves -= 2;
			}
			else if (halves == 1) {
				HeartContainers.Add(new(1));
				halves -= 1;
			}
			else {
				HeartContainers.Add(new(0));
			}

			HUD.InsertHeartAtPos(HeartContainers.Count - 1, HeartContainers[^1].RedHeart.Sprite);
		}
	}

	public void GiveHeart(int halves, HeartEType type) {
		switch (type) {
			case HeartEType.RedHeart:
				for (int i = 0; i < HeartContainers.Count; i++) {
					while (HeartContainers[i].RedHeart.Halves < 2 && halves > 0) {
						HeartContainers[i].RedHeart.Halves++;
						halves--;
						HUD.UpdateHeartAtPos(i, HeartContainers[i].RedHeart.Sprite);
					}
				}
				break;
			
			case HeartEType.BlueHeart:
				while (halves > 0) {
					int totalHearts = HeartContainers.Count + LooseHearts.Count;

					if (LooseHearts.Count == 0 && totalHearts != 12) {
						LooseHearts.Add((HeartBlue)new(1));
						HUD.InsertHeartAtPos(totalHearts, LooseHearts[0].Sprite);
					}
					else if (LooseHearts[^1].Halves == 2 && totalHearts != 12) {
						LooseHearts.Add((HeartBlue)new(1));
						HUD.InsertHeartAtPos(totalHearts, LooseHearts[^1].Sprite);
					}
					else if (LooseHearts[^1].Halves == 1) {
						LooseHearts[^1].Halves++;
						HUD.UpdateHeartAtPos(totalHearts - 1, LooseHearts[^1].Sprite);
					}
					halves--;
				}
				break;

			default:
				break;
		}
	}

	public void TakeDamage(int damage, int type) {
		for (int i = 0; i < damage; i++) {
			if (LooseHearts.Count > 0) {
				LooseHearts.Last().Halves -= 1;

				if (LooseHearts.Last().Halves == 0) {
					LooseHearts.RemoveAt(LooseHearts.Count - 1);
					HUD.RemoveLastHeart();
				}
				else {
					HUD.UpdateLastHeart(LooseHearts.Last().Sprite);
				}
			}
			else if (GetRedHearts() > 0) {
				int cIndex = (int)Math.Ceiling(((double)GetRedHearts() / 2) - 1);
				HeartContainers[cIndex].RedHeart.Halves -= 1;
				HUD.UpdateHeartAtPos(cIndex, HeartContainers[cIndex].RedHeart.Sprite);
			}

			CheckDeathCondition();
			if (!IsAlive) {
				break;
			}
		}

		if (IsAlive) {
			OnTakeDamage();
		}
	}

	public int GetRedHearts() {
		int redHeartHalves = 0;
		foreach (HeartContainer container in HeartContainers) {
			redHeartHalves += container.RedHeart.Halves;
		}

		return redHeartHalves;
	}

	public int GetOtherHearts() {
		int otherHeartHalves = 0;
		foreach (HeartBase heart in LooseHearts) {
			otherHeartHalves += heart.Halves;
		}

		return otherHeartHalves;
	}

	private void CheckDeathCondition() {
		if (GetRedHearts() == 0 && GetOtherHearts() == 0) {
			Die();
		}
	}

	protected override void Die() {
		Main.Player.IsAlive = false;
	}

    protected override void OnTakeDamage() {
        Invulnerable = true;
		IFrameTimer.Start();
    }

	private void IFrameTimerTimeout() {
		Invulnerable = false;
	}

	public void CalculateAttackDamage() {
		// Sets damage variable for other methods to use
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
		RefireTimer.WaitTime = refireDelay;
	}
}
