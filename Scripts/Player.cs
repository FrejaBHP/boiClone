using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : Character {
	public bool IsAlive { get; set; }
	public float IFrameTime { get; set; }

	public float DamageBonus { get; set; } = 0;
    public float HealthBonus { get; set; } = 0;
    public float RangeBonus { get; set; } = 0;
	public float ShotSpeedBonus { get; set; } = 0;
    public float SpeedBonus { get; set; } = 0;
    public float AttackDelayBonus { get; set; } = 0;
	public float FlatDamageBonus { get; set; } = 0;
	public float FlatAttackRateBonus { get; set; } = 0;

	public List<Item> Inventory { get; set; }

	public Player() {
		Main.Player = this;
		Inventory = new();
		IsAlive = true;
		IFrameTime = 1f;
		CanShoot = true;
	}

    public override void _Ready() {
		GetNode<Timer>("IFrameTimer").WaitTime = IFrameTime;
		GetNode<Timer>("RefireTimer").WaitTime = 1f / CalculateAttackRate();
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
				int itemID = (int)pedestal.GetMeta("itemID");
				
				if (!pedestal.ItemRemoved) {
					//THIS SHOULD BE TURNED INTO A SIGNAL AND HANDLED BY EITHER WORLD OR MAIN
					pedestal.ItemRemoved = true;
					pedestal.GetChild<Sprite2D>(2).Visible = false;

					Item item = (Item)Activator.CreateInstance(ItemCollection.ItemTypes[itemID]);
					Inventory.Add(item);
					Main.AddItemStats(item);
					GD.Print($"Picked up {Inventory.Last().ItemName}!");
				}
			}
		}
    }

	public override Projectile SetProjectileProperties(Projectile proj) {
		float effectiveDamage = (float)(Damage * Math.Sqrt(DamageBonus * 1.2 + 1) + FlatDamageBonus);
		proj.Damage = effectiveDamage;

		proj.Speed = 256 * ShotSpeed + ShotSpeedBonus;
		proj.Range = Range + RangeBonus;

		proj.Lifetime = (proj.Range * 32) / proj.Speed;

		proj.Radius = 8; //Currently Unused
		proj.Knockback = 5; //Currently Unused

		proj.Piercing = ShotsArePiercing;
		proj.Spectral = ShotsAreSpectral;

		if (proj.Spectral) {
			proj.SetCollisionMaskValue(3, false); //disables collision with rocks
		}
		proj.SetCollisionMaskValue(4, false); //disables collision with player

		return proj;
	}

	private void UpdateStats(int statIndex, float amount) {
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
	}

    protected override void ShootProjectile(int dir) {
		if (CanShoot) {
			CanShoot = false;
			GetNode<Timer>("RefireTimer").Start();

			Projectile proj = Projectile.Instantiate() as Projectile;
			proj = SetProjectileProperties(proj);
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

	private void IFrameTimerTimeout() {
		Invulnerable = false;
	}

    protected override void OnTakeDamage() {
        Invulnerable = true;
		GetNode<Timer>("IFrameTimer").Start();
    }

	protected override void RefireTimerTimeout() {
		GetNode<Timer>("RefireTimer").WaitTime = 1f / CalculateAttackRate();
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

	public float CalculateAttackRate() {
		//This is inefficient to calculate on every attack
		float effectiveAttackDelay = 16 - 6 * (float)Math.Sqrt(((AttackDelay + AttackDelayBonus) * 1.3) + 1);
		float effectiveAttackRate;

		if (5 > effectiveAttackDelay) {
			effectiveAttackRate = 5 + FlatAttackRateBonus;
		}
		else {
			effectiveAttackRate = 30 / (effectiveAttackDelay + 1) + FlatAttackRateBonus;
		}

		//GD.Print($"Delay: {effectiveAttackDelay}, Rate: {effectiveAttackRate}");
		return effectiveAttackRate;
	}
}
