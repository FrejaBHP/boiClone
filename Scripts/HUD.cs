using Godot;
using System;
using System.Collections.Generic;

public partial class HUD : CanvasLayer {
	#region Labels
	private static Label lifeDisplay;

	private static Label coinsText;
	private static Label bombsText;
	private static Label keysText;

	private static Label speedText;
	private static Label rateText;
	private static Label damageText;
	private static Label rangeText;
	private static Label shotSpeedText;

	private static List<Label> pickupLabelList;
	private static List<Label> statLabelList;
	#endregion
	
	public override void _Ready() {
		lifeDisplay = GetNode<Label>("Life");

		coinsText = GetNode<Label>("CoinText");
		bombsText = GetNode<Label>("BombText");
		keysText = GetNode<Label>("KeyText");

		speedText = GetNode<Label>("SpeedText");
		rateText = GetNode<Label>("RateText");
		damageText = GetNode<Label>("DamageText");
		rangeText = GetNode<Label>("RangeText");
		shotSpeedText = GetNode<Label>("ShotSpeedText");

		pickupLabelList = new() {
			coinsText,
			bombsText,
			keysText
		};

		statLabelList = new() {
			speedText,
			rateText,
			damageText,
			rangeText,
			shotSpeedText
		};
		
		PositionLabels();
	}

	#region UpdateMethods
	// Health
	public static void HUDUpdateHealth(float health, float maxHealth) {
		lifeDisplay.Text = $"Health: {health:F1} / {maxHealth:F1}";
	}

	// Pickups
	public static void HUDUpdateCoins(int coins) {
		coinsText.Text = $"C: {coins}";
	}

	public static void HUDUpdateBombs(int bombs) {
		bombsText.Text = $"B: {bombs}";
	}

	public static void HUDUpdateKeys(int keys) {
		keysText.Text = $"K: {keys}";
	}

	// Stats
	public static void HUDUpdateSpeed(float speed) {
		speedText.Text = $"Speed: {speed:F2}";
	}

	public static void HUDUpdateRate(float rate) {
		rateText.Text = $"Rate: {rate:F2}";
	}

	public static void HUDUpdateDamage(float dmg) {
		damageText.Text = $"DMG: {dmg:F2}";
	}

	public static void HUDUpdateRange(float range) {
		rangeText.Text = $"Range: {range:F2}";
	}

	public static void HUDUpdateShotSpeed(float shotSpeed) {
		shotSpeedText.Text = $"SSpeed: {shotSpeed:F2}";
	}
	#endregion

	private static void PositionLabels() {
		Vector2 pos;

		int leftMargin = 10;
		int pickupLabelOffset = 100;

		int statLabelYOffset = 300;
		int statLabelYGap = 30;
		
		pos.X = leftMargin;
		pos.Y = leftMargin;
		lifeDisplay.Position = pos;

		// Position pickup labels
		pos.Y = pickupLabelOffset;
		for (int i = 0; i < pickupLabelList.Count; i++) {
			pickupLabelList[i].Position = pos;
			
			if (i != pickupLabelList.Count - 1)
				pos.Y += statLabelYGap;
		}

		// Position stat labels
		pos.Y = statLabelYOffset;
		for (int i = 0; i < statLabelList.Count; i++) {
			statLabelList[i].Position = pos;
			
			if (i != statLabelList.Count - 1)
				pos.Y += statLabelYGap;
		}
	}
}
