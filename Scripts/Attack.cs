using System;
using Godot;
using Vector2 = Godot.Vector2;

public enum AttackType {
    Projectile = 0,
    Beam = 1
}

[Flags]
public enum AttackFlags {
    NONE = 0,
    Piercing = 1 << 0,
    Spectral = 1 << 1,
    Ring = 1 << 2,
    BeamFixed = 1 << 3,
    BeamStatic = 1 << 4
}

public static class Attack {
    private static readonly PackedScene projBase = GD.Load<PackedScene>("Scenes/projectile.tscn");
    private static readonly PackedScene beamBase = GD.Load<PackedScene>("Scenes/beam.tscn");
    private static readonly float projectileAngleCoefficient = 0.08f; // 0.1f = 18deg,  0.01f = 1,8deg

    public static void PrepareProjectileAttack(Player player, int dir) {
        float speed = player.ShotSpeed + player.ShotSpeedBonus;
        float damage = player.EffectiveDamage;
        float range = player.Range + player.RangeBonus;
        int amount = player.AmountPerAttack;
        float hurts = 1;
        float rotation = (float)Math.PI * 0.5f * dir;

        ProjectileAttack(player, speed, damage, range, amount, hurts, rotation, player.AttackFlags);
    }

    public static void PrepareProjectileAttack(Enemy enemy, EnemyProjectileComponent projComp, Vector2 direction) {
        float speed = projComp.ShotSpeed;
        float damage = 10;
        float range = projComp.Range;
        int amount = projComp.ProjectilesPerAttack;
        float hurts = 0;
        float rotation = direction.Angle() + 0.5f * (float)Math.PI;

        ProjectileAttack(enemy, speed, damage, range, amount, hurts, rotation, projComp.AttackFlags);
    }

    private static void ProjectileAttack(CharacterBody2D attacker, float speed, float damage, float range, int amount, float hurts, float rotation, AttackFlags flags) {
        float[] rotationOffsets = CalculateOffsets(amount, flags.HasFlag(AttackFlags.Ring));
        float newRotation;

        for (int i = 0; i < amount; i++) {
            Projectile proj = projBase.Instantiate() as Projectile;
            proj.Speed = 256 * speed;
            proj.Damage = damage;
            proj.Range = range;
            proj.Lifetime = proj.Range * 32 / proj.Speed;

            if (flags.HasFlag(AttackFlags.Piercing)) {
                proj.SetPiercing(true);
            }
            if (flags.HasFlag(AttackFlags.Spectral)) {
                proj.SetSpectral(true);
            }

            if (hurts == 0) {
			    proj.HurtsPlayer(true);
		    }
            if (hurts == 1) {
			    proj.HurtsEnemies(true);
		    }

            proj.AddToGroup(attacker.Name + "Proj");
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

    public static void PrepareBeamAttack(Player player, int dir) {
        float ticks = 9;
        float damage = player.EffectiveDamage;
        float range = player.Range + player.RangeBonus;
        int amount = player.AmountPerAttack;
        float hurts = 1;
        float rotation = (float)Math.PI * 0.5f * dir;
        float width = player.AttackWidth;

        BeamAttack(player, ticks, damage, range, amount, hurts, rotation, width, player.AttackFlags);
    }

    public static async void BeamAttack(CharacterBody2D attacker, float ticks, float damage, float range, int amount, float hurts, float rotation, float width, AttackFlags flags) {
        float[] rotationOffsets = CalculateOffsets(amount, flags.HasFlag(AttackFlags.Ring));
        float newRotation;

        for (int i = 0; i < amount; i++) {
            Beam beam = beamBase.Instantiate() as Beam;
            beam.Damage = damage;
            beam.Ticks = ticks;
            beam.Range = range * 12; // Range stat only 37,5% effective

            beam.Fixed = flags.HasFlag(AttackFlags.BeamFixed);
            beam.Static = flags.HasFlag(AttackFlags.BeamStatic);

            if (!beam.IsNodeReady()) {
                await beam.SetReferences();
            }

            beam.SetWidth(width);
            newRotation = rotation + rotationOffsets[i];
            beam.SetAngle(Vector2.FromAngle(newRotation));

            if (hurts == 0) {
			    beam.HurtsPlayer(true);
		    }
            if (hurts == 1) {
			    beam.HurtsEnemies(true);
		    }

            if (flags.HasFlag(AttackFlags.BeamStatic)) {
                beam.AddToGroup(attacker.Name + "BeamStatic");
            }
            else {
                beam.AddToGroup(attacker.Name + "Beam");
            }
            World.CurrentRoom.ProjectilesNode.AddChild(beam);
            //GD.Print(beam.GetGroups()[0]);
        }
    }

    private static float[] CalculateOffsets(int amount, bool ring) {
        float[] offsets = new float[amount];
        if (ring) {
            float offsetPerStep = ((float)Math.PI * 2) / amount;
            for (int i = 0; i < amount; i++) {
                offsets[i] = offsetPerStep * i;
            }
        }
        else {
            if (amount % 2 != 0) {
                int numberOfOffsets = (amount - 1) / 2;
                float offset = -projectileAngleCoefficient * numberOfOffsets;
                for (int i = 0; i < amount; i++) {
                    offsets[i] = (float)Math.PI * offset;
                    offset += projectileAngleCoefficient;
                }
            }
            else if (amount != 1) {
                int numberOfOffsets = amount / 2;
                float offset = -projectileAngleCoefficient * numberOfOffsets;
                offset += projectileAngleCoefficient / 2;
                for (int i = 0; i < amount; i++) {
                    offsets[i] = (float)Math.PI * offset;
                    offset += projectileAngleCoefficient;
                }
            }
            else {
                offsets[0] = 0;
            }
        }
        return offsets;
    }
}
