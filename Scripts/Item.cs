using Godot;
using System;
using System.Collections.Generic;

/*
[Flags]
public enum ItemCategories {
    NONE = 0

}

[Flags]
public enum ItemPools {
    NONE = 0,
    Treasure = 1 << 0
}
*/

public abstract class Item {
	//public abstract int ItemID { get; }
    //public abstract string ItemName { get; }
    //public abstract ItemCategories Categories { get; }
    //public abstract ItemPools Pools { get; }

    public abstract void OnPickedUp();

    public abstract void OnRemoved();
}
