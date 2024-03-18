using Godot;
using System;

public partial class Enemy : CharacterBody2D {
    public bool CountsTowardsEnemyCount { get; protected set; }
	public override void _Ready() {
        AddToGroup("Enemy");
        Setup();
    }
    
    protected virtual void Setup() {

    }

    public Vector2 GetPlayerVector(Vector2 enemyPos, Vector2 playerPos) {
		return playerPos - enemyPos;
	}

	public Vector2 GetPlayerVectorNormalised() {
		Vector2 direction = GetPlayerVector(GlobalPosition, Main.PlayerPosition);
		direction = direction.Normalized();

		return direction;
	}

    protected virtual bool GetIfFlipSpriteH(Vector2 norm) {
        if (norm.X <= 0) {
            return false;
        }
        else {
            return true;
        }
    }
}
