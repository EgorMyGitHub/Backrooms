using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DecoyExit : MonoBehaviour
{
    [SerializeField] private Animator anim;
    
    private async void OnCollisionEnter(Collision other)
    {
        if (other.collider.TryGetComponent<FirstPersonController>(out var player))
        {
            anim.SetTrigger("Decoy");

            Destroy(GetComponent<BoxCollider>());
            
            await Task.Delay(700);
            
            Destroy(gameObject);
        }
    }
}
