using Godot;
using System;

public partial class Character : CharacterBody2D {
	protected PackedScene Projectile = GD.Load<PackedScene>("Scenes/projectile.tscn");
	
	public virtual float MaxHealth { get; set; }
	public virtual float Health { get; set; }
	public virtual float Damage { get; set; }
	public virtual float Range { get; set; }
	public virtual float Speed { get; set; }
	public virtual float ShotSpeed { get; set; }
	public virtual float AttackDelay { get; set; }

	public virtual int Team { get; set; }

	public virtual bool CanShoot { get; set; }
	public virtual bool ShotsArePiercing { get; set; }
	public virtual bool ShotsAreSpectral { get; set; }
	public virtual bool Invulnerable { get; set; }

	protected float moveSpeed = 208f;

	public virtual Projectile SetProjectileProperties(Projectile proj) {
		proj.Speed = 256 * ShotSpeed;
		proj.Range = Range;
		proj.Damage = Damage;
		proj.Piercing = ShotsArePiercing;
		proj.Spectral = ShotsAreSpectral;

		proj.Lifetime = (proj.Range * 32) / proj.Speed;

		proj.Radius = 8;
		proj.Knockback = 5;

		if (proj.Spectral) {
			proj.SetCollisionMaskValue(3, false); //disables collision with rocks
		}

		if (Team == 0) {
			proj.SetCollisionMaskValue(4, false); //disables collision with player
		}
		if (Team == 1) {
			proj.SetCollisionMaskValue(5, false); //disables collision with enemies
		}

		return proj;
	}

	protected virtual void CheckBodyCollision() {
		
	}

	protected virtual void ShootProjectile(int dir) {

	}

	public virtual void CheckTile() {
		/*
		Vector2I tileCoord = World.CurrentTileMap.LocalToMap(ToLocal(GlobalPosition));
		Vector2I tileDef = World.CurrentTileMap.GetCellAtlasCoords(0, tileCoord);

		if (IsInGroup("Player")) {
			if (tileDef.X == 4 && tileDef.Y == 1) {
				GD.Print("Dør!");
			}
		}
		*/
	}

	public virtual void Move(Vector2 dir) {
		if (dir != Vector2.Zero) {
			Velocity = dir * (moveSpeed * Speed);
			MoveAndSlide();
			//CheckTile();
			CheckBodyCollision();
		}
	}

	public virtual void ModifyHealth(float change) {
		if (!Invulnerable) {
			Health += change;
			GD.Print($"HP: {Health}, diff: {change}");

			if (change < 0) {
				OnTakeDamage();
			}
		
			if (Health <= 0) {
				if (IsInGroup("Player")) {
					Main.Player.IsAlive = false;
				}
				else {
					Die();
				}
			}
		}
	}

	protected virtual void Die() {
		World.DecreaseEnemyCount();
		QueueFree();
	}

	protected virtual void OnTakeDamage() {

	}

	protected virtual void RefireTimerTimeout() {
		CanShoot = true;
	}
}
