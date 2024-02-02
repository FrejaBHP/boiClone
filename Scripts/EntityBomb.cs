using Godot;
using System;

public partial class EntityBomb : Node2D {
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		GetNode<Timer>("BombTimer").Start();
	}

	private void OnBombTimerTimeout() {
		Area2D detonationArea = GetNode<Area2D>("BombRadius");
        Godot.Collections.Array<Node2D> bodies = detonationArea.GetOverlappingBodies();

		foreach (var node in bodies) {
			if (node.IsInGroup("Enemy")) {
				Main.ProcessEnemyDamage(Main.Player, node as Enemy, 100);
			}
			else if (node.IsInGroup("Player")) {
				Main.ProcessPlayerDamage(Main.Player, 2);
			}
		}

		QueueFree();
	}
}
