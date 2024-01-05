using System.Collections.Generic;
using Godot;

public static class ItemCollection {
    public static List<System.Type> ItemTypes { get; set; }
    public static List<string> ItemSpritePaths { get; set; }

    public static void CompileItemList() {
        ItemTypes = new() {
            typeof(BasicItem),
            typeof(TearsItem)
        };

        ItemSpritePaths = new() {
            "Images/TestWep.png",
            "Images/TestWep.png"
        };
        GD.Print($"Loaded item types: {ItemTypes.Count}");
    }
}
