interface IInstantEffect {
    void OnPickedUp();
    void OnRemoved();
}

interface ITriggeredEffect {
    double Chance { get; set; }
    void OnTrigger();
}

interface IActiveEffect {
    double ChargesPerActivation { get; set; }
    double Charge { get; set; }
    double MaxCharges { get; set; }

    void SetCharge(double newCharge) {
        if (newCharge >= MaxCharges) {
            Charge = MaxCharges;
        }
        else {
            Charge = newCharge;
        }
        HUD.SetActiveChargeBarCharge(Charge);
    }

    void OnActivation();
}


public class Item {
    public int ItemDataID { get; protected set; }
}
