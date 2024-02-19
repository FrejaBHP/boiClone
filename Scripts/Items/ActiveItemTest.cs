public partial class ActiveItemTest : Item, IActiveEffect {
    public double ChargesPerActivation { get; set; }
    public double Charge { get; set; }
    public double MaxCharges { get; set; }

    private readonly IActiveEffect a;

    public ActiveItemTest() {
        a = this;
        
        ChargesPerActivation = 6;
        Charge = 6;
        MaxCharges = 6;
    }

    public void OnActivation() {
        Main.Player.GiveHeart(2, HeartEType.BlueHeart);
        a.SetCharge(0);
    }
}
