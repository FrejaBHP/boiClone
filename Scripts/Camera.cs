using Godot;
using System;

public partial class Camera : Camera2D {
	public override void _Ready() {
		Main.Camera = this;
	}
}
