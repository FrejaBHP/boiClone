using Godot;
using System;

public partial class NewItemShowcase : CanvasLayer {
	private static Label itemNameLabel;
	private static Label itemDescLabel;
	private static Sprite2D splash;
	private static Timer showcaseTimer;

	public override void _Ready() {
		Main.ItemShowcase = this;
		Visible = false;

		showcaseTimer = GetNode<Timer>("ShowcaseTimer");
		splash = GetNode<Sprite2D>("TextBG");
		itemNameLabel = GetNode<Label>("ItemName");
		itemDescLabel = GetNode<Label>("ItemDescription");
		
		//itemNameText.Size = itemNameText.Size with { X = 200, Y = 50 };
		//itemDescText.Size = itemDescText.Size with { X = 300, Y = 150 };
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
