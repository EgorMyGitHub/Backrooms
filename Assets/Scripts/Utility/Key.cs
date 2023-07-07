using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : ICanPickup
{
    protected override void Pickup()
    {
        inventory.KeyPickup();
        Destroy(gameObject);
    }
}
