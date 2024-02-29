using System.Collections.Generic;
using Godot;

public static class ItemCollection {
    public static List<ItemData> ItemDataSet { get; private set; }

    public static void CompileItemList() {
        // int id, string name, Type type, Texture2D sprite, ItemCategories cFlags, ItemPools pFlags, string desc
        ItemDataSet = new() {
            new(0, "Damage Item", typeof(BasicItem), GD.Load<Texture2D>("Images/testWep.png"), ItemCategories.NONE, ItemPools.Treasure, "DMG Up!"),
            new(1, "Tears Item", typeof(TearsItem), GD.Load<Texture2D>("Images/testWep.png"), ItemCategories.NONE, ItemPools.Treasure, "Rate Up!"),
            new(2, "Orange", typeof(Orange), GD.Load<Texture2D>("Images/orange.png"), ItemCategories.NONE, ItemPools.Treasure, "You feel healthy!"),
            new(3, "Cool Book", typeof(ActiveItemTest), GD.Load<Texture2D>("Images/Items/book.png"), ItemCategories.NONE, ItemPools.Treasure, "For testing purposes!"),
            new(4, "Cooler Book", typeof(ActiveItemTest2), GD.Load<Texture2D>("Images/Items/book2.png"), ItemCategories.NONE, ItemPools.Treasure, "For even more testing purposes!")
        };

        GD.Print($"Loaded item datasets: {ItemDataSet.Count}");
    }
}
