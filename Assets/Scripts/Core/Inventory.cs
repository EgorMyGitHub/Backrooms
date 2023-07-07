using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public bool keyInInventory { get; private set; }

    public void KeyPickup()
    {
        keyInInventory = true;
    }
}
