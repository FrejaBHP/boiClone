using System.Collections.Generic;
using Godot;

public enum EnemyNames {
    BasicEnemy
}

public static class EnemyCollection {
    public static List<PackedScene> EnemyScenes { get; private set; }

    public static void CompileEnemyList() {
        EnemyScenes = new() {
            GD.Load<PackedScene>("Scenes/Enemies/basic_enemy.tscn")
        };
        GD.Print($"Loaded enemy types: {EnemyScenes.Count}");
    }
}
