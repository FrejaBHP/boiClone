using Godot;
using System;
using System.Collections.Generic;

public partial class HUD : CanvasLayer {
	private static GridContainer heartsGridContainer;

	#region Labels
	private static Label lifeDisplay;

	private static Label coinsLabel;
	private static Label bombsLabel;
	private static Label keysLabel;

	private static Label speedLabel;
	private static Label rateLabel;
	private static Label damageLabel;
	private static Label rangeLabel;
	private static Label shotSpeedLabel;
	private static Label luckLabel;
	#endregion
	
	public override void _Ready() {
		heartsGridContainer = GetNode<GridContainer>("HeartsContainer");

		coinsLabel = GetNode<Label>("PickupsContainer/CoinLabel");
		bombsLabel = GetNode<Label>("PickupsContainer/BombLabel");
		keysLabel = GetNode<Label>("PickupsContainer/KeyLabel");

		speedLabel = GetNode<Label>("StatsContainer/SpeedLabel");
		rateLabel = GetNode<Label>("StatsContainer/RateLabel");
		damageLabel = GetNode<Label>("StatsContainer/DamageLabel");
		rangeLabel = GetNode<Label>("StatsContainer/RangeLabel");
		shotSpeedLabel = GetNode<Label>("StatsContainer/ShotSpeedLabel");
		luckLabel = GetNode<Label>("StatsContainer/LuckLabel");
	}

	#region UpdateMethods
	// Health
	public static void RedrawHearts(List<HeartContainer> conts, List<HeartBase> loose) {
		foreach (var child in heartsGridContainer.GetChildren()) {
			child.QueueFree();
		}

		foreach (HeartContainer container in conts) {
            Sprite2D heart = new() {
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
		heartsGridContainer.MoveChild(control, n);
	}

	public static void UpdateHeartAtPos(int n, Texture2D image) {
		heartsGridContainer.GetChild<Control>(n).GetChild<Sprite2D>(0).Texture = image;
	}

	public static void UpdateLastHeart(Texture2D image) {
		heartsGridContainer.GetChild<Control>(heartsGridContainer.GetChildCount() - 1).GetChild<Sprite2D>(0).Texture = image;
	}

	public static void RemoveLastHeart() {
		heartsGridContainer.GetChild(heartsGridContainer.GetChildCount() - 1).Free();
	}

	// Pickups
	public static void UpdateCoins(int coins) {
		if (coins < 10) {
			coinsLabel.Text = $"C: 0{coins}";
		}
		else {
			coinsLabel.Text = $"C: {coins}";
		}
	}

	public static void UpdateBombs(int bombs) {
		if (bombs < 10) {
			bombsLabel.Text = $"B: 0{bombs}";
		}
		else {
			bombsLabel.Text = $"B: {bombs}";
		}
	}

	public static void UpdateKeys(int keys) {
		if (keys < 10) {
			keysLabel.Text = $"K: 0{keys}";
		}
		else {
			keysLabel.Text = $"K: {keys}";
		}
	}

	// Stats
	public static void UpdateSpeed(float speed) {
		speedLabel.Text = $"Speed: {speed:F2}";
	}

	public static void UpdateRate(float rate) {
		rateLabel.Text = $"Rate: {rate:F2}";
	}

	public static void UpdateDamage(float dmg) {
		damageLabel.Text = $"DMG: {dmg:F2}";
	}

	public static void UpdateRange(float range) {
		rangeLabel.Text = $"Range: {range:F2}";
	}

	public static void UpdateShotSpeed(float shotSpeed) {
		shotSpeedLabel.Text = $"SSpeed: {shotSpeed:F2}";
	}
	public static void UpdateLuck(float luck) {
		luckLabel.Text = $"Luck: {luck:F2}";
	}
	#endregion
}
