using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Rooms : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoses;
    [SerializeField] private Transform[] doorAndKeySpawnPoses;

    [SerializeField] private GameObject end;
    
    private SpawnController m_SpawnController;
    
    private bool m_IsSpawn = false;
    private bool m_IsEndSpawn = false;

    private void Start()
    {
        m_SpawnController = ComponentRoot.Resolve<SpawnController>();
        
        if(m_SpawnController.countSpawn > 0)
        {
            Spawn();
        }
        else
        {
            SpawnEnd();
        }
    }

    private async void Spawn()
    {
        m_IsSpawn = true;
        
        var result = await m_SpawnController.Spawn(spawnPoses);
        
        if(!result)
            SpawnEnd();
    }
    
    private void SpawnEnd()
    {
        m_IsEndSpawn = true;
        
        var pos = new Vector3(
            transform.position.x - 8.49f,
            transform.position.y,
            transform.position.z + 28.14f);
        
        Instantiate(end, pos, Quaternion.identity);

        Destroy(gameObject);
    }

    public bool SpawnKey(GameObject table)
    {
        if (m_IsEndSpawn)
            return false;
        
        var spawnTransform = doorAndKeySpawnPoses[Random.Range(0, doorAndKeySpawnPoses.Length)];

        var position = spawnTransform.position;
        Instantiate(table, new Vector3(position.x, 0.8f, position.z), spawnTransform.rotation);

        return true;
    }
    
    public bool SpawnExit(Exit spawnExit)
    {
        if (m_IsEndSpawn)
            return false;
        
        var spawnTransform = doorAndKeySpawnPoses[Random.Range(0, doorAndKeySpawnPoses.Length)];

        Instantiate(spawnExit, spawnTransform.position, spawnTransform.rotation);

        return true;
    }
}
