using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : CharacterBody2D {
	protected PackedScene Bomb = GD.Load<PackedScene>("Scenes/entityBomb.tscn");
	protected PackedScene Projectile = GD.Load<PackedScene>("Scenes/projectile.tscn");

	#region Properties
	public List<Item> Inventory { get; set; }
	public Item ActiveItem { get; set; }

	public AttackType AttackType { get; set; }
	public AttackFlags AttackFlags { get; set; }

	public float EffectiveDamage { get; private set; }
	public float EffectiveFireRate { get; private set; }
	public int AmountPerAttack { get; set; }
	public float AttackWidth { get; set; }

	public bool IsAlive { get; set; }
	public float IFrameTime { get; set; }
	public bool Invulnerable { get; private set; }

	private Timer refireTimer;
	private Timer iFrameTimer;
	private Timer bombPlacementTimer;

	private float baseSpeed = 208f;
	private float refireDelay;

	private bool canShoot;
	private bool canPlaceBomb;

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
	public float Speed {
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
	public float AttackDelay {
		get => attackDelay;
		set {
			attackDelay = value;
			CalculateAttackRate();
			HUD.UpdateRate(EffectiveFireRate);
		} 
	}

	private float attackDelayBonus = 0;
	public float AttackDelayBonus {
		get => attackDelayBonus;
		set {
			attackDelayBonus = value;
			CalculateAttackRate();
			HUD.UpdateRate(EffectiveFireRate);
		} 
	}

	private float flatAttackRateBonus = 0;
	public float FlatAttackRateBonus {
		get => flatAttackRateBonus;
		set {
			flatAttackRateBonus = value;
			CalculateAttackRate();
			HUD.UpdateRate(EffectiveFireRate);
		} 
	}

	private float damage;
	public float Damage {
		get => damage;
		set {
			damage = value;
			CalculateAttackDamage();
			HUD.UpdateDamage(EffectiveDamage);
		} 
	}

	private float damageBonus = 0;
	public float DamageBonus {
		get => damageBonus;
		set {
			damageBonus = value;
			CalculateAttackDamage();
			HUD.UpdateDamage(EffectiveDamage);
		} 
	}

	private float flatDamageBonus = 0;
	public float FlatDamageBonus {
		get => flatDamageBonus;
		set {
			flatDamageBonus = value;
			CalculateAttackDamage();
			HUD.UpdateDamage(EffectiveDamage);
		}
	}
	
	private float range;
	public float Range {
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
    public float ShotSpeed { 
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
	#endregion

    public Player() {
		Main.Player = this;
		Inventory = new();
		IsAlive = true;
		IFrameTime = 1f;
		canShoot = true;
		canPlaceBomb = true;
	}

    public override void _Ready() {
		refireTimer = GetNode<Timer>("RefireTimer");
		iFrameTimer = GetNode<Timer>("IFrameTimer");
		bombPlacementTimer = GetNode<Timer>("BombPlacementTimer");
		
		iFrameTimer.WaitTime = IFrameTime;

		AddToGroup("Player");
    }

    public override void _PhysicsProcess(double delta) {
		Vector2 dir = Input.GetVector("left", "right", "up", "down");
		Move(dir);
		Main.PlayerPosition = GlobalPosition;
	}

	public void Move(Vector2 dir) {
		if (dir != Vector2.Zero) {
			Velocity = dir * (baseSpeed * Speed);
			MoveAndSlide();
			CheckBodyCollision();
		}
	}

    private void CheckBodyCollision() {
        for (int i = 0; i < GetSlideCollisionCount(); i++) {
			KinematicCollision2D col = GetSlideCollision(i);
			if (col.GetCollider().IsClass("StaticBody2D")) { // So far, only item pedestals are StaticBody2D objects, so no further checks are needed
				ItemPedestal pedestal = col.GetCollider() as ItemPedestal;
				pedestal.OnPlayerCollision();
			}
		}
    }

	public override void _Input(InputEvent @event) {
		int shootingDir;
		if (@event.IsActionPressed("shootup")) {
			shootingDir = 0;
			StartAttack(shootingDir);
		}
		else if (@event.IsActionPressed("shootright")) {
			shootingDir = 1;
			StartAttack(shootingDir);
		}
		else if (@event.IsActionPressed("shootdown")) {
			shootingDir = 2;
			StartAttack(shootingDir);
		}
		else if (@event.IsActionPressed("shootleft")) {
			shootingDir = 3;
			StartAttack(shootingDir);
		}

		if (@event.IsActionPressed("placebomb")) {
			PlaceBomb();
		}

		if (@event.IsActionPressed("activateItem")) {
			ActivateItem();
		}
    }

	private void StartAttack(int dir) {
		if (canShoot) {
			switch (AttackType) {
				case AttackType.Projectile:
					canShoot = false;
					refireTimer.Start();
					Attack.PrepareProjectileAttack(this, dir);
					break;

				case AttackType.Beam:
					canShoot = false;
					refireTimer.Start();
					Attack.PrepareBeamAttack(this, dir);
					break;
			
				default:
					break;
			}
		}
	}

	public void InstantAttack(int dir, AttackType type, bool overrideType, AttackFlags flags, bool overrideFlags) { // WIP
		switch (type) {
			case AttackType.Projectile:
				Attack.PrepareProjectileAttack(this, dir);
				break;
		
			default:
				break;
		}
	}

	public void OnRefireTimerTimeout() {
		canShoot = true;
		
		int shootingDir;
		if (Input.IsActionPressed("shootup")) {
			shootingDir = 0;
			StartAttack(shootingDir);
		}
		else if (Input.IsActionPressed("shootright")) {
			shootingDir = 1;
			StartAttack(shootingDir);
		}
		else if (Input.IsActionPressed("shootdown")) {
			shootingDir = 2;
			StartAttack(shootingDir);
		}
		else if (Input.IsActionPressed("shootleft")) {
			shootingDir = 3;
			StartAttack(shootingDir);
		}
	}

	private void PlaceBomb() {
		if (Bombs > 0 && canPlaceBomb) {
			EntityBomb bomb = Bomb.Instantiate() as EntityBomb;
			World.CurrentRoom.EntitiesNode.AddChild(bomb);
			bomb.GlobalPosition = GlobalPosition;

			Bombs--;

			canPlaceBomb = false;
			bombPlacementTimer.Start();
		}
	}

	private void OnBombPlacementTimerTimeout() {
		canPlaceBomb = true;
	}

	private void ActivateItem() {
		if (ActiveItem != null) {
			IActiveEffect a = ActiveItem as IActiveEffect;

			if (a.Charge >= a.ChargesPerActivation) {
				a.OnActivation();
			}
		}
	}

	public void GainCharge(float charge) {
		if (ActiveItem != null) {
			IActiveEffect a = ActiveItem as IActiveEffect;

			a.SetCharge(a.Charge + charge);
		}
	}

	public void GiveHeartContainers(int amount, int halves) {
		for (int i = 0; i < amount; i++) {
			HeartContainers.Add(new(0));
			HUD.InsertHeartAtIndex(HeartContainers.Count - 1, HeartContainers[^1].RedHeart.Sprite);
		}
		GiveHeart(halves, HeartEType.RedHeart);
	}

	public void RemoveHeartContainers(int amount) {
		for (int i = 0; i < amount; i++) {
			HeartContainers.RemoveAt(HeartContainers.Count - 1);
			HUD.RemoveHeartAtIndex(HeartContainers.Count - 1);
		}
		CheckDeathConditionAndPossiblyDie();
	}

	public void GiveHeart(int halves, HeartEType type) {
		// Healing is performed in half-heart steps
		switch (type) {
			case HeartEType.RedHeart:
				for (int i = 0; i < HeartContainers.Count; i++) {
					while (HeartContainers[i].RedHeart.Halves < 2 && halves > 0) {
						HeartContainers[i].RedHeart.Halves++;
						halves--;
						HUD.UpdateHeartAtIndex(i, HeartContainers[i].RedHeart.Sprite);
					}
				}
				break;
			
			case HeartEType.BlueHeart:
				while (halves > 0) {
					int totalHearts = HeartContainers.Count + LooseHearts.Count;

					if (LooseHearts.Count == 0 && totalHearts != 12) {
						LooseHearts.Add((HeartBlue)new(1));
						HUD.InsertHeartAtIndex(totalHearts, LooseHearts[0].Sprite);
					}
					else if (LooseHearts[^1].Halves == 2 && totalHearts != 12) {
						LooseHearts.Add((HeartBlue)new(1));
						HUD.InsertHeartAtIndex(totalHearts, LooseHearts[^1].Sprite);
					}
					else if (LooseHearts[^1].Halves == 1) {
						LooseHearts[^1].Halves++;
						HUD.UpdateHeartAtIndex(totalHearts - 1, LooseHearts[^1].Sprite);
					}
					else {
						break;
					}
					halves--;
				}
				break;
			
			case HeartEType.BlackHeart:
				while (halves > 0) {
					int totalHearts = HeartContainers.Count + LooseHearts.Count;

					if (LooseHearts.Count == 0 && totalHearts != 12) {
						LooseHearts.Add((HeartBlack)new(1));
						HUD.InsertHeartAtIndex(totalHearts, LooseHearts[0].Sprite);
					}
					else if (LooseHearts[^1].Halves == 2 && totalHearts != 12) {
						LooseHearts.Add((HeartBlack)new(1));
						HUD.InsertHeartAtIndex(totalHearts, LooseHearts[^1].Sprite);
					}
					else if (LooseHearts[^1].Halves == 1) {
						// Black Hearts convert half Blue Hearts if possible
						if (LooseHearts[^1].GetType() == typeof(HeartBlue)) {
							LooseHearts[^1] = (HeartBlack)new(2);
						}
						else {
							LooseHearts[^1].Halves++;
						}
						HUD.UpdateHeartAtIndex(totalHearts - 1, LooseHearts[^1].Sprite);
					}
					else if (totalHearts == 12) {
						// Black Hearts convert first found Blue Heart if hearts are full
						int heartIndex = LooseHearts.FindIndex(h => h.GetType() == typeof(HeartBlue));
						if (heartIndex != -1) {
							LooseHearts[heartIndex] = (HeartBlack)new(2);
							HUD.UpdateHeartAtIndex(HeartContainers.Count + heartIndex, LooseHearts[heartIndex].Sprite);

							if (halves % 2 == 0) {
								halves--;
							}
						}
					}
					else {
						break;
					}
					halves--;
				}
				break;

			default:
				break;
		}
	}

	public void TakeDamage(int damage, int type) { // TODO: Implement Red Heart-only damage switch
		// Like healing, taking damage is performed in half-heart steps
		for (int i = 0; i < damage; i++) {
			if (LooseHearts.Count > 0) {
				LooseHearts[^1].Halves -= 1;

				if (LooseHearts[^1].Halves == 0) {
					if (LooseHearts[^1].GetType() == typeof(HeartBlack)) {
						(LooseHearts[^1] as HeartBlack).OnBroken();
					}
					LooseHearts.RemoveAt(LooseHearts.Count - 1);
					HUD.RemoveLastHeart();
				}
				else {
					HUD.UpdateLastHeart(LooseHearts[^1].Sprite);
				}
			}
			else if (GetRedHearts() > 0) {
				int cIndex = (int)Math.Ceiling(((double)GetRedHearts() / 2) - 1);
				HeartContainers[cIndex].RedHeart.Halves -= 1;
				HUD.UpdateHeartAtIndex(cIndex, HeartContainers[cIndex].RedHeart.Sprite);
			}

			CheckDeathConditionAndPossiblyDie();
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

	private void CheckDeathConditionAndPossiblyDie() {
		if (GetRedHearts() == 0 && GetOtherHearts() == 0) {
			Die();
		}
	}

	private void Die() {
		Main.Player.IsAlive = false;
	}

    private void OnTakeDamage() {
        Invulnerable = true;
		iFrameTimer.Start();
    }

	private void IFrameTimerTimeout() {
		Invulnerable = false;
	}

	public void CalculateAttackDamage() {
		// Sets damage variable for other methods to use
		EffectiveDamage = (float)(Damage * Math.Sqrt(DamageBonus * 1.2 + 1) + FlatDamageBonus);
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
		EffectiveFireRate = effectiveAttackRate;
		refireDelay = 1f / EffectiveFireRate;
		refireTimer.WaitTime = refireDelay;
	}
}
