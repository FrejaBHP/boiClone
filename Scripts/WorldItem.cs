using Godot;
using System;

public partial class WorldItem : StaticBody2D {
	private bool itemRemoved = false;
	public bool ItemRemoved {
		get => itemRemoved;
		set {
			itemRemoved = value;
			HideItemSprite();
		}
	}

	public void PlayerCollided() {
		if (!ItemRemoved) {
			int itemID = (int)GetMeta("itemID");
			ItemRemoved = true;
			Main.GiveItem(itemID);
		}
	}

	private void HideItemSprite() {
		GetNode<Sprite2D>("ItemSpriteItem").Visible = false;
	}
}
