using Godot;
using System;

[Tool]
public partial class EnemyCollisionComponent : Node2D {
	[Export]
	private bool dealsContactDamage;
	
	public void CheckBodyCollision(Enemy enemy) {
        for (int i = 0; i < enemy.GetSlideCollisionCount(); i++) {
			KinematicCollision2D col = enemy.GetSlideCollision(i);
			
			if (col.GetCollider().IsClass("CharacterBody2D")) {
				CharacterBody2D collider = (CharacterBody2D)col.GetCollider();
				
				if (collider.IsInGroup("Player") && dealsContactDamage) {
					Main.ProcessPlayerDamage(enemy, 1);
				}
			}
		}
    }
}
