using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private InputAction key;
    
    private void Start()
    {
        gameObject.SetActive(true);
        
        PlayerInput.Inputs[key.bindings.FirstOrDefault().path].performed += SetActive;
    }

    private void SetActive(InputAction.CallbackContext obj)
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
