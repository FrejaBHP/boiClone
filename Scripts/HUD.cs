using Godot;
using System;
using System.Collections.Generic;

public partial class HUD : CanvasLayer {
	private static GridContainer heartsGridContainer;

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
		heartsGridContainer = GetNode<GridContainer>("HeartsContainer");

		coinsText = GetNode<Label>("PickupsContainer/CoinText");
		bombsText = GetNode<Label>("PickupsContainer/BombText");
		keysText = GetNode<Label>("PickupsContainer/KeyText");

		speedText = GetNode<Label>("StatsContainer/SpeedText");
		rateText = GetNode<Label>("StatsContainer/RateText");
		damageText = GetNode<Label>("StatsContainer/DamageText");
		rangeText = GetNode<Label>("StatsContainer/RangeText");
		shotSpeedText = GetNode<Label>("StatsContainer/ShotSpeedText");

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
	}

	#region UpdateMethods
	// Health
	public static void RedrawHearts(List<HeartContainer> conts, List<HeartBase> loose) {
		foreach (var child in heartsGridContainer.GetChildren()) {
			child.QueueFree();
		}

		foreach (HeartContainer container in conts) {
            Sprite2D heart = new Sprite2D {
                Texture = container.RedHeart.Sprite
            };
        }
	}

	public static void InsertHeartAtPos(int n, Texture2D image) {
		Control control = new();

        Sprite2D heart = new() {
            Texture = image
        };
		
        heartsGridContainer.AddChild(control);
		control.AddChild(heart);
	}

	public static void UpdateHeartAtPos(int n, Texture2D image) {
		heartsGridContainer.GetChild<Control>(n).GetChild<Sprite2D>(0).Texture = image;
	}

	public static void UpdateLastHeart(Texture2D image) {
		heartsGridContainer.GetChild<Control>(heartsGridContainer.GetChildCount()).GetChild<Sprite2D>(0).Texture = image;
	}

	public static void RemoveLastHeart() {
		heartsGridContainer.GetChild(heartsGridContainer.GetChildCount()).QueueFree();
	}

	// Pickups
	public static void HUDUpdateCoins(int coins) {
		if (coins < 10) {
			coinsText.Text = $"C: 0{coins}";
		}
		else {
			coinsText.Text = $"C: {coins}";
		}
	}

	public static void HUDUpdateBombs(int bombs) {
		if (bombs < 10) {
			bombsText.Text = $"B: 0{bombs}";
		}
		else {
			bombsText.Text = $"B: {bombs}";
		}
	}

	public static void HUDUpdateKeys(int keys) {
		if (keys < 10) {
			keysText.Text = $"K: 0{keys}";
		}
		else {
			keysText.Text = $"K: {keys}";
		}
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
}
