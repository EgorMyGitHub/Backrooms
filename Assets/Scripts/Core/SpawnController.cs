using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class SpawnController : MonoBehaviour
{
    [SerializeField] private List<Rooms> allRooms = new();

    [SerializeField] private Exit spawnExit;

    [SerializeField] private GameObject spawnTable;
    
    [SerializeField] private int doorCount;
    [SerializeField] private int keyCount;
    [SerializeField] private int nextBotsCount;

    [SerializeField] public float countSpawn;
    [SerializeField] public int maxDelay = 1;
    
    private UpdateBakingInRunTime m_UpdateBakingInRunTime;

    private bool m_IsEnd = false;

    private Timer timer;

    public bool isSpawned { get; private set; }
    
    private void Start()
    {
        timer = ComponentRoot.Resolve<Timer>();
        m_UpdateBakingInRunTime = ComponentRoot.Resolve<UpdateBakingInRunTime>();
    }

    public async Task<bool> Spawn(Transform[] spawnPoses)
    {
        foreach (var item in spawnPoses)
        {
            await Task.Delay(Random.Range(0, maxDelay));
            
            if(Contains(item))
                continue;
            
            if(countSpawn <= 0)
            {
                EndSpawn();
                return false;
            }
            
            countSpawn--;
            
            var rooms = GetVariable.GetRandomRooms();

            var position = item.position;
            var obj = Instantiate(rooms, new Vector3(position.x, 0.01f, position.z), Quaternion.identity);
            
            allRooms.Add(obj);
        }
        
        return true;
    }

    private async Task EndSpawn()
    {
        if(m_IsEnd)
            return;
        
        m_IsEnd = true;

        //await Task.Delay(500);
        
        m_UpdateBakingInRunTime.UpdateBaking();

        for (int i = 0; i < nextBotsCount; i++)
        {
            SpawnNextBot();
        }

        SpawnDoors();
        SpawnKeys();
        
        timer.StartTimer();

        isSpawned = true;
    }

    private void SpawnKeys()
    {
        for (int i = 0; i < keyCount;)
        {
            var index = Random.Range(0, allRooms.Count);

            if (allRooms[index].SpawnKey(spawnTable))
                i++;
            
            allRooms.RemoveAt(index);
        }
    }

    private void SpawnDoors()
    {
        for (int i = 0; i < doorCount;)
        {
            var index = Random.Range(0, allRooms.Count);

            if (allRooms[index].SpawnExit(spawnExit))
                i++;
            
            allRooms.RemoveAt(index);
        }
    }
    
    private void SpawnNextBot()
    {
        var randomNextBot = GetVariable.GetRandomNextBots();
        
        var randomPos = Vector3.zero;
            
        while (Math.Abs(randomPos.z) <= 70 
               || Math.Abs(randomPos.x) <= 70)
        {
            randomPos = new Vector3(Random.Range(100, -60), 0.01f, Random.Range(-350, 200));
        }
        
        Instantiate(randomNextBot, randomPos, Quaternion.identity);
    }
    
    private bool Contains(Transform item)
    {
        var result = allRooms.Find(i => i != null && item != null && i.transform.position == item.position && i != item);

        return result != null;
    }
}
