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

    protected virtual bool GetIfFlipSpriteH(Vector2 norm) {
        if (norm.X <= 0) {
            return false;
        }
        else {
            return true;
        }
    }
}
