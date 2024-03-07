using Godot;
using System;

public partial class ItemPedestal : StaticBody2D {
	public bool ItemRemoved {
		get => itemRemoved;
		set {
			itemRemoved = value;
			HideItemSprite();
		}
	}

	public int ItemID { get; private set; }

	private bool itemRemoved = false;
	private bool interactedWith = false;

	private double heldCharge = 0;
	private Timer swapTimer;

	public override void _Ready() {
		swapTimer = GetNode<Timer>("SwapTimer");
		ItemID = (int)GetMeta("itemID");
	}

	public void OnPlayerCollision() {
		if (!ItemRemoved) {
			if (ItemCollection.ItemDataSet[ItemID].Type.GetInterface("IActiveEffect") != null && Main.Player.ActiveItem != null) {
				SwapItems();
			}
			else {
				GiveItem();
			}
		}
	}

	public void GiveItem() {
		ItemRemoved = true;
		Main.GiveItem(ItemID);
	}

	public void SwapItems() {
		if (swapTimer.TimeLeft == 0) {
			IActiveEffect a = Main.Player.ActiveItem as IActiveEffect;
			double chargeToHold = a.Charge;
			int idToHold = Main.Player.ActiveItem.ItemDataID;

			Main.GiveItem(ItemID);
			if (interactedWith) {
				a = Main.Player.ActiveItem as IActiveEffect;
				a.SetCharge(heldCharge);
			}
			heldCharge = chargeToHold;

			ItemID = idToHold;
			SetMeta("itemID", ItemID);
			SetItemSprite(ItemID);

			interactedWith = true;
			swapTimer.Start();
		}
	}

	public void SetItemSprite(int id) {
		Texture2D sprite = ItemCollection.ItemDataSet[id].Sprite;
		GetNode<Sprite2D>("ItemSpriteItem").Texture = sprite;
	}

	private void HideItemSprite() {
		GetNode<Sprite2D>("ItemSpriteItem").Visible = false;
	}
}
