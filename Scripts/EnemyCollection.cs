using System.Collections.Generic;
using Godot;

public static class EnemyCollection {
    public static List<PackedScene> EnemyScenes { get; set; }
    static EnemyCollection() {
        //CompileEnemyList();
    }

    public static void CompileEnemyList() {
        EnemyScenes = new() {
            GD.Load<PackedScene>("Scenes/Enemies/basic_enemy.tscn")
        };
        GD.Print($"Loaded enemies: {EnemyScenes.Count}");
    }
}
