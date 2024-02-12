using Godot;
using System;

[Tool]
public partial class EnemyCollisionComponent : Node2D {
	[Export]
	private bool dealsContactDamage;
	/*
	public void CheckBodyCollision() {
        for (int i = 0; i < GetSlideCollisionCount(); i++) {
			KinematicCollision2D col = GetSlideCollision(i);
			
			if (col.GetCollider().IsClass("CharacterBody2D")) {
				Character collider = (Character)col.GetCollider();
				
				if (collider.IsInGroup("Player")) {
					Main.ProcessPlayerDamage(this, 1);
				}
				
				else if (collider.IsInGroup("Enemy")) {
					Main.ProcessEnemyDamage(this, (Enemy)collider, Damage);
				}
				
			}
		}
    }
	*/

}
