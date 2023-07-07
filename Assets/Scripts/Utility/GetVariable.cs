using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GetVariable : MonoBehaviour
{
    [SerializeField] private Rooms[] rooms;
    [SerializeField] private NextBots[] nextBots;
    [SerializeField] private GameObject[] screamers;

    private static Rooms[] _variablesRoom;
    private static NextBots[] _variablesNextBots;
    private static GameObject[] _variablesScreamers;

    private void Awake()
    {
        _variablesRoom = rooms;
        _variablesNextBots = nextBots;
        _variablesScreamers = screamers;
    }

    public static Rooms GetRandomRooms() =>
        _variablesRoom[Random.Range(0, _variablesRoom.Length)];
    
    public static NextBots GetRandomNextBots() =>
        _variablesNextBots[Random.Range(0, _variablesNextBots.Length)];
    
    public static GameObject GetRandomScreamers() =>
        _variablesScreamers[Random.Range(0, _variablesScreamers.Length)];
}
