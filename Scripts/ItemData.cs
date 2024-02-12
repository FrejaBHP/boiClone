using System;
using System.Collections.Generic;
using Godot;

[Flags]
public enum ItemCategories {
    NONE = 0
}

[Flags]
public enum ItemPools {
    NONE = 0,
    Treasure = 1 << 0,
    Boss = 1 << 1,
    Secret = 1 << 2
}

public partial class ItemData {
    public int ID { get; }
    public string Name { get; }
    public Type Type { get; }
    public Texture2D Sprite { get; }
    public ItemCategories CategoryFlags { get; }
    public ItemPools PoolFlags { get; }
    public string Description { get; }

    public ItemData(int id, string name, Type type, Texture2D sprite, ItemCategories cFlags, ItemPools pFlags, string desc) {
        ID = id;
        Name = name;
        Type = type;
        Sprite = sprite;
        CategoryFlags = cFlags;
        PoolFlags = pFlags;
        Description = desc;
    }
}
