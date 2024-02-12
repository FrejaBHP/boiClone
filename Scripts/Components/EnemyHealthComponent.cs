using Godot;
using System;

[Tool]
public partial class EnemyHealthComponent : Node2D {
	[Signal]
	public delegate void DiedEventHandler();

	private float maxHealth;
	[Export]
	public float MaxHealth { 
		get => maxHealth; 
		set => maxHealth = value; 
	}

	private float health;
	[Export]
	public float Health { 
		get => health; 
		set {
			if (value > maxHealth) {
				health = maxHealth;
			}
			else {
				health = value; 
			}
			//GD.Print(health);
		}
	}

	[Export]
	public bool Retaliates;
	[Export]
	public bool Invulnerable;
	
	public void TakeDamage(float damage) {
		if (!Invulnerable) {
			Health -= damage;
			OnDamageTaken();

			if (Retaliates) {
				
			}
		
			if (Health <= 0) {
				Die();
				//OnNoHealth();
			}
		}
	}

	public void Heal(float heal) {
		Health += heal;
	}

	public virtual void Retaliation() {

	}

	public virtual void OnDamageTaken() {

	}

	public virtual void OnNoHealth() {

	}

	public void Die() {
		//EmitSignal(SignalName.Died);
		GetNode<World>("/root/Main/World").DecreaseEnemyCount();
		GetParent().QueueFree();
	}
}
