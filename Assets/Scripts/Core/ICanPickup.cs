using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ICanPickup : MonoBehaviour
{
    [SerializeField] private InputAction pickupKey;
    
    private bool isCanPickup;

    protected Inventory inventory;
    
    private void Start()
    {
        inventory = ComponentRoot.Resolve<Inventory>();
        
        PlayerInput.Inputs[pickupKey.bindings.FirstOrDefault().path].performed += TryPickup;
    }

    private void OnDestroy()
    {
        if(!PlayerInput.Inputs.ContainsKey(pickupKey.bindings.FirstOrDefault().path))
            return;
        
        PlayerInput.Inputs[pickupKey.bindings.FirstOrDefault().path].performed -= TryPickup;
    }

    private void TryPickup(InputAction.CallbackContext callbackContext)
    {
        if(isCanPickup)
            Pickup();
    }

    protected abstract void Pickup();

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<FirstPersonController>(out var player))
        {
            isCanPickup = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<FirstPersonController>(out var player))
        {
            isCanPickup = false;
        }
    }
}
