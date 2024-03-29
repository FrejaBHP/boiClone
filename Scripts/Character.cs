using Godot;
using System;

public partial class Character : CharacterBody2D {
	protected PackedScene Projectile = GD.Load<PackedScene>("Scenes/projectile.tscn");
	public virtual float Speed { get; set; }

	protected float moveSpeed = 208f;

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

	protected virtual void Die() {

	}

	protected virtual void OnTakeDamage() {

	}

	protected virtual void RefireTimerTimeout() {
		//CanShoot = true;
	}
}
