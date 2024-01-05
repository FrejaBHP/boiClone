using Godot;
using System;

public partial class HUD : CanvasLayer {
	public static Label LifeDisplay { get; set; }
	public static Label DamageText { get; set; }
	public static Label RateText { get; set; }

	public override void _Ready() {
		//Tilføj separate billeder og sæt dem ind før teksten. Onsdagsopgave :)
		LifeDisplay = GetNode<Label>("Life");
		DamageText = GetNode<Label>("DamageText");
		RateText = GetNode<Label>("RateText");
		LifeDisplay.Text = "Life Placeholder";
		PositionLabels();
	}

	public static void HUDUpdateDamage(float dmg) {
		DamageText.Text = $"DMG: {dmg:F2}";
	}

	public static void HUDUpdateRate(float rate) {
		RateText.Text = $"Rate: {rate:F2}";
	}

	private static void PositionLabels() {
		int leftMargin = 10;
		int statLabelYOffset = 300;
		int statLabelYGap = 20;
		Vector2 pos;
		
		pos.X = leftMargin;
		pos.Y = leftMargin;
		LifeDisplay.Position = pos;

		pos.Y = statLabelYOffset;
		DamageText.Position = pos;

		pos.Y += statLabelYGap;
		RateText.Position = pos;
	}

	public override void _Process(double delta) {
	
	}
}
