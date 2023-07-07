using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StepsController : MonoBehaviour
{
    [SerializeField] private float sprintingSpawnCooldown;
    [SerializeField] private float walkingSpawnCooldown;

    [SerializeField] private Sound[] walkingSound;
    [SerializeField] private Sound[] sprintingSound;
    
    private FirstPersonController player;

    private void Start()
    {
        player = ComponentRoot.Resolve<FirstPersonController>();
        
        StartCoroutine(UpdatesStepsSound());
    }

    private IEnumerator UpdatesStepsSound()
    {
        while (true)
        {
            CreateStepsSound();
            
            yield return new WaitForSeconds(player.isSprinting ? sprintingSpawnCooldown : walkingSpawnCooldown);
        }
    }
    
    private void CreateStepsSound()
    {
        if (player.isSprinting)
        {
            Instantiate(sprintingSound[Random.Range(0, sprintingSound.Length)]);
        }
        else if(player.isWalking)
        {
            Instantiate(walkingSound[Random.Range(0, walkingSound.Length)]);
        }
    }
}
