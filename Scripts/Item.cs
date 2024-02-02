using Godot;
using System;
using System.Collections.Generic;

public abstract class Item {
    public abstract void OnPickedUp();

    public abstract void OnRemoved();
}
