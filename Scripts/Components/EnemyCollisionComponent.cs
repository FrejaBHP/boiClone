using Godot;
using System;

[Tool]
public partial class EnemyCollisionComponent : Node2D {
	[Export]
	private bool dealsContactDamage;

	public Enemy CompOwner { get; private set; }

	public override void _Ready() {
        CompOwner = (Enemy)GetParent();
    }
	
	public void CheckBodyCollision() {
        for (int i = 0; i < CompOwner.GetSlideCollisionCount(); i++) {
			KinematicCollision2D col = CompOwner.GetSlideCollision(i);
			
			if (col.GetCollider().IsClass("CharacterBody2D")) {
				CharacterBody2D collider = (CharacterBody2D)col.GetCollider();
				
				if (collider.IsInGroup("Player") && dealsContactDamage) {
					Main.ProcessPlayerDamage(CompOwner, 1);
				}
			}
		}
    }
}
