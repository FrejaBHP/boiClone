using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class HUD : CanvasLayer {
	private static CanvasLayer hud;
	private static GridContainer heartsGridContainer;

	private static TextureRect activeItemSprite;
	private static TextureProgressBar activeItemChargeBar;

	#region Labels
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
		hud = this;

		heartsGridContainer = GetNode<GridContainer>("HeartsContainer");

		activeItemSprite = GetNode<TextureRect>("ActiveItemContainer/ActiveItemSprite");
		activeItemChargeBar = GetNode<TextureProgressBar>("ActiveItemContainer/ChargeBar");

		coinsLabel = GetNode<Label>("PickupsContainer/CoinLabel");
		bombsLabel = GetNode<Label>("PickupsContainer/BombLabel");
		keysLabel = GetNode<Label>("PickupsContainer/KeyLabel");

		speedLabel = GetNode<Label>("StatsContainer/SpeedContainer/SpeedLabel");
		rateLabel = GetNode<Label>("StatsContainer/RateContainer/RateLabel");
		damageLabel = GetNode<Label>("StatsContainer/DamageContainer/DamageLabel");
		rangeLabel = GetNode<Label>("StatsContainer/RangeContainer/RangeLabel");
		shotSpeedLabel = GetNode<Label>("StatsContainer/ShotSpeedContainer/ShotSpeedLabel");
		luckLabel = GetNode<Label>("StatsContainer/LuckContainer/LuckLabel");
	}

	public static void HideHUD() {
		hud.Visible = true;
	}

	public static void ShowHUD() {
		hud.Visible = true;
	}

	#region UpdateMethods
	// Health
	public static void RedrawHearts(List<HeartContainer> conts, List<HeartBase> loose) { // Doesn't work and is currently unused
		foreach (var child in heartsGridContainer.GetChildren()) {
			child.QueueFree();
		}

		foreach (HeartContainer container in conts) {
            Sprite2D heart = new() {
                Texture = container.RedHeart.Sprite
            };
        }
	}

	public static void InsertHeartAtIndex(int index, Texture2D sprite) {
		Control control = new();

        Sprite2D heart = new() {
            Texture = sprite
        };
		
        heartsGridContainer.AddChild(control);
		control.AddChild(heart);
		heartsGridContainer.MoveChild(control, index);
	}

	public static void UpdateHeartAtIndex(int index, Texture2D sprite) {
		heartsGridContainer.GetChild<Control>(index).GetChild<Sprite2D>(0).Texture = sprite;
	}

	public static void RemoveHeartAtIndex(int index) {
		heartsGridContainer.GetChild(index).Free();
	}

	public static void UpdateLastHeart(Texture2D sprite) {
		heartsGridContainer.GetChild<Control>(heartsGridContainer.GetChildCount() - 1).GetChild<Sprite2D>(0).Texture = sprite;
	}

	public static void RemoveLastHeart() {
		heartsGridContainer.GetChild(heartsGridContainer.GetChildCount() - 1).Free();
	}

	// Active item
	public static void UpdateActiveItem(Texture2D sprite, double charge, double maxCharge) {
		SetActiveItemSprite(sprite);
		UpdateActiveChargeBar(charge, maxCharge);
		ShowActiveItemHUD();
	}

	private static void UpdateActiveChargeBar(double charge, double maxCharge) {
		SetActiveChargeBarCharge(charge);
		GetAndSetChargeBarSprite(maxCharge);
		SetActiveChargeBarLimit(maxCharge);
	}

	public static void ShowActiveItemHUD() {
		activeItemSprite.Visible = true;
		activeItemChargeBar.Visible = true;
	}

	public static void HideActiveItemHUD() {
		activeItemSprite.Visible = false;
		activeItemChargeBar.Visible = false;
	}

	public static void SetActiveItemSprite(Texture2D sprite) {
		activeItemSprite.Texture = sprite;
	}

	public static void SetActiveChargeBarCharge(double charge) {
		activeItemChargeBar.Value = charge;
	}

	private static void GetAndSetChargeBarSprite(double maxCharge) {
		switch (maxCharge) {
			case 1:
				SetActiveChargeBarSprite(48, 32);
				break;
			
			case 2:
				SetActiveChargeBarSprite(32, 32);
				break;
			
			case 3:
				SetActiveChargeBarSprite(16, 32);
				break;
			
			case 4:
				SetActiveChargeBarSprite(0, 32);
				break;
			
			case 5:
				SetActiveChargeBarSprite(64, 0);
				break;
			
			case 6:
				SetActiveChargeBarSprite(48, 0);
				break;
			
			case 8:
				SetActiveChargeBarSprite(64, 32);
				break;

			case 12:
				SetActiveChargeBarSprite(32, 0);
				break;

			default:
				GD.PushError($"Invalid charge count of {maxCharge}");
				break;
		}
	}

	private static void SetActiveChargeBarSprite(int x, int y) {
		Rect2 newRegion = new(x, y, 16, 32);
		AtlasTexture atlas = activeItemChargeBar.TextureOver as AtlasTexture;
		atlas.Region = newRegion;
	}

	private static void SetActiveChargeBarLimit(double maxCharge) {
		activeItemChargeBar.MaxValue = maxCharge;
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
		speedLabel.Text = $"{speed:F2}";
	}

	public static void UpdateRate(float rate) {
		rateLabel.Text = $"{rate:F2}";
	}

	public static void UpdateDamage(float dmg) {
		damageLabel.Text = $"{dmg:F2}";
	}

	public static void UpdateRange(float range) {
		rangeLabel.Text = $"{range:F2}";
	}

	public static void UpdateShotSpeed(float shotSpeed) {
		shotSpeedLabel.Text = $"{shotSpeed:F2}";
	}
	public static void UpdateLuck(float luck) {
		luckLabel.Text = $"{luck:F2}";
	}
	#endregion
}
