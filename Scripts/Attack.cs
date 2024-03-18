using System;
using Godot;

public enum AttackType {
    Projectile = 0
}

[Flags]
public enum AttackFlags {
    NONE = 0
}

public static class Attack {
    private static readonly PackedScene projBase = GD.Load<PackedScene>("Scenes/projectile.tscn");
    private static readonly float projectileAngleCoefficient = 0.08f; // 0.1f = 18deg,  0.01f = 1,8deg

    public static void PrepareProjectileAttack(Player player, int dir) {
        float speed = player.ShotSpeed + player.ShotSpeedBonus;
        float damage = player.EffectiveDamage;
        float range = player.Range + player.RangeBonus;
        int amount = player.ProjectilesPerAttack;
        float hurts = 1;
        float rotation = (float)Math.PI * 0.5f * dir;

        ProjectileAttack(player, speed, damage, range, amount, hurts, rotation);
    }

    public static void PrepareProjectileAttack(Enemy enemy, EnemyProjectileComponent projComp, Vector2 direction) {
        float speed = projComp.ShotSpeed;
        float damage = 10;
        float range = projComp.Range;
        int amount = projComp.ProjectilesPerAttack;
        float hurts = 0;
        float rotation = direction.Angle() + 0.5f * (float)Math.PI;

        ProjectileAttack(enemy, speed, damage, range, amount, hurts, rotation);
    }

    private static void ProjectileAttack(CharacterBody2D attacker, float speed, float damage, float range, int amount, float hurts, float rotation) {
        float[] rotationOffsets = new float[amount];
        float newRotation;

        if (amount % 2 != 0) {
            int numberOfOffsets = (amount - 1) / 2;
            float offset = -projectileAngleCoefficient * numberOfOffsets;
            for (int i = 0; i < amount; i++) {
                rotationOffsets[i] = (float)Math.PI * offset;
                offset += projectileAngleCoefficient;
            }
        }
        else if (amount != 1) {
            int numberOfOffsets = amount / 2;
            float offset = -projectileAngleCoefficient * numberOfOffsets;
            offset += projectileAngleCoefficient / 2;
            for (int i = 0; i < amount; i++) {
                rotationOffsets[i] = (float)Math.PI * offset;
                offset += projectileAngleCoefficient;
            }
        }
        else {
            rotationOffsets[0] = 0;
        }

        for (int i = 0; i < amount; i++) {
            Projectile proj = projBase.Instantiate() as Projectile;
            proj.Speed = 256 * speed;
            proj.Damage = damage;
            proj.Range = range;
            proj.Lifetime = proj.Range * 32 / proj.Speed;

            if (hurts == 0) {
			    proj.SetCollisionMaskValue((int)ECollisionLayer.Player, true);
		    }
            if (hurts == 1) {
			    proj.SetCollisionMaskValue((int)ECollisionLayer.Enemy, true);
		    }

            World.CurrentRoom.ProjectilesNode.AddChild(proj);

            Transform2D trans = Transform2D.Identity;
			trans.Origin = attacker.GlobalPosition;

			newRotation = rotation + rotationOffsets[i];
			trans.X.X = trans.Y.Y = Mathf.Cos(newRotation);
			trans.X.Y = trans.Y.X = Mathf.Sin(newRotation);
			trans.Y.X *= -1;

			proj.GlobalTransform = trans;
        }
    }
}
