using System;
using UnityEngine;

public class Initialized : MonoBehaviour
{
    [SerializeField] private UpdateBakingInRunTime updateBakingInRunTime;
    [SerializeField] private SpawnController spawnController;
    [SerializeField] private GameController gameController;
    [SerializeField] private FirstPersonController player;
    [SerializeField] private Timer timer;
    
    private void Awake()
    {
        ComponentRoot.Bind(updateBakingInRunTime);
        ComponentRoot.Bind(spawnController);
        ComponentRoot.Bind(gameController);
        ComponentRoot.Bind(player);
        ComponentRoot.Bind(timer);
        ComponentRoot.Bind(new Inventory());
        ComponentRoot.Bind(new LoginController());
    }
}