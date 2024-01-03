using Godot;
using System;
using System.Collections.Generic;

public partial class Item {
	public int ItemID { get; set; }
    public string ItemName { get; set; }
    public virtual float DamageBonus { get; set; } = 0;
    public virtual float HealthBonus { get; set; } = 0;
    public virtual float RangeBonus { get; set; } = 0;
    public virtual float ShotSpeedBonus { get; set; } = 0;
    public virtual float SpeedBonus { get; set; } = 0;
    public virtual float AttackDelayBonus { get; set; } = 0;
    public virtual float FlatDamageBonus { get; set; } = 0;
	public virtual float FlatAttackRateBonus { get; set; } = 0;
    //public abstract Texture2D ItemSprite { get; set; }

    public virtual List<float> ItemBonuses { get; }

    public Item() {
        ItemBonuses = new() {
            DamageBonus,
            HealthBonus,
            RangeBonus,
            ShotSpeedBonus,
            SpeedBonus,
            AttackDelayBonus,
            FlatDamageBonus,
            FlatAttackRateBonus
        };
    }

    protected virtual void OnPickedUp() {
        
    }

    protected virtual void OnRemoved() {

    }
}
