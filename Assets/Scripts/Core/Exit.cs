using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    private Inventory inventory;
    private GameController gameController;
    
    private void Awake()
    {
        inventory = ComponentRoot.Resolve<Inventory>();
        gameController = ComponentRoot.Resolve<GameController>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.TryGetComponent(out FirstPersonController player) && inventory.keyInInventory)
        {
            Destroy(player);
            gameController.SetGameOver(true);
        }
    }
}
