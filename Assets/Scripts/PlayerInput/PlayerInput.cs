using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private List<InputAction> keysName;

    public static Dictionary<string, InputAction> Inputs = new();

    private void Awake()
    {
        foreach (var item in keysName)
        {
            item.Enable();
            
            Inputs.Add(item.bindings.FirstOrDefault().path, item);
        }
    }

    private void OnDestroy()
    {
        Inputs = new();
    }
}
