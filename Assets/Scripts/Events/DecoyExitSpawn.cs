using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoyExitSpawn : IHorrorEvent
{
    public DecoyExitSpawn(GameObject spawnObject, FirstPersonController player)
    {
        m_SpawnObject = spawnObject;
        m_Player = player;
    }

    private GameObject m_SpawnObject;
    private FirstPersonController m_Player;
    
    public void Execute()
    {
        Vector3 spawnPos = new Vector3(m_Player.transform.position.x + Random.Range(-5, 5), -0.1f, m_Player.transform.position.z + Random.Range(-5, 5));
        
        var obj = GameObject.Instantiate(m_SpawnObject, spawnPos, Quaternion.identity);
        
        m_Player.RotateTo(obj.transform);
    }
}
