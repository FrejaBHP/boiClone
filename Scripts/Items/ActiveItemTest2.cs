public partial class ActiveItemTest2 : Item, IActiveEffect {
    public double ChargesPerActivation { get; set; }
    public double Charge { get; set; }
    public double MaxCharges { get; set; }

    public override int ItemDataID => base.ItemDataID;

    private readonly IActiveEffect a;

    public ActiveItemTest2() {
        ItemDataID = 4;
        a = this;
        
        ChargesPerActivation = 4;
        Charge = 4;
        MaxCharges = 4;
    }

    public void OnActivation() {
        Main.Player.GiveHeart(1, HeartEType.BlackHeart);
        a.SetCharge(0);
    }
}
