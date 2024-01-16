using Godot;
using System;
using System.Collections.Generic;

public abstract class Item {
	public abstract int ItemID { get; }
    public abstract string ItemName { get; }

    public abstract void OnPickedUp();

    public abstract void OnRemoved();
}
