using Godot;
using System;

public partial class NewItemShowcase : CanvasLayer {
	private static Label itemNameLabel;
	private static Label itemDescLabel;
	private static Sprite2D splash;
	private static Timer showcaseTimer;

	public override void _Ready() {
		Main.ItemShowcase = this;

		showcaseTimer = GetNode<Timer>("ShowcaseTimer");
		splash = GetNode<Sprite2D>("TextBG");
		itemNameLabel = GetNode<Label>("ItemName");
		itemDescLabel = GetNode<Label>("ItemDescription");
	}

	private void SetNewName(string input) {
		itemNameLabel.Text = input;
	}

	private void SetNewDesc(string input) {
		itemDescLabel.Text = input;
	}

	public void ShowNewItem(ItemData itemData) {
		SetNewName(itemData.Name);
		SetNewDesc(itemData.Description);
		showcaseTimer.Start();
		Visible = true;
	}

	private void OnShowcaseTimerTimeout() {
		Visible = false;
	}
	

	private void CenterContainer() {

	}
}
