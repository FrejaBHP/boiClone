using System.Collections.Generic;
using Godot;

public static class ItemCollection {
    public static List<ItemData> ItemDataSet { get; private set; }

    public static void CompileItemList() {
        // int id, string name, Type type, string path, ItemCategories cFlags, ItemPools pFlags, string desc
        ItemDataSet = new() {
            new(0, "Damage Item", typeof(BasicItem), "Images/TestWep.png", ItemCategories.NONE, ItemPools.Treasure, "DMG Up!"),
            new(1, "Tears Item", typeof(TearsItem), "Images/TestWep.png", ItemCategories.NONE, ItemPools.Treasure, "Rate Up!")
        };

        GD.Print($"Loaded item datasets: {ItemDataSet.Count}");
    }
}
