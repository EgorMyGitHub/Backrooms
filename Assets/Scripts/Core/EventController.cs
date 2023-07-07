using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class EventController : MonoBehaviour
{
    [SerializeField] private Animator cameraAnim;
    [SerializeField] private Animator screamerAnim;
    
    [SerializeField] private int minTickTime;
    [SerializeField] private int maxTickTime;

    [SerializeField] private GameObject decoyExit;
    [SerializeField] private GameObject screamerObj;

    [SerializeField] private Sound sound;
    
    private Tick tick;
    
    private List<IHorrorEvent> events;
    
    private void Start()
    {
        FirstPersonController player = ComponentRoot.Resolve<FirstPersonController>();
        
        events = new()
        {
            new Screamer(cameraAnim, screamerAnim, player, screamerObj),
            new DecoyExitSpawn(decoyExit, player),
            new SoundSeizure(sound),
        };

        tick = new Tick(minTickTime, maxTickTime);
        tick.tick += CastRandomEvent;
    }

    private void OnDestroy()
    {
        tick.tick -= CastRandomEvent;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F2))
            CastRandomEvent();
    }

    private void CastRandomEvent()
    {
        events[Random.Range(0, events.Count)].Execute();
    }
}

public class Tick
{
    public Tick(int minTickTime, int maxTickTime)
    {
        m_maxTickTime = maxTickTime;
        m_minTickTime = minTickTime;

        TickStarted();
    }
    
    private int m_minTickTime;
    private int m_maxTickTime;

    public event Action tick;
    
    private async Task TickStarted()
    {
        while (true)
        {
            await WaitAndExecute();

            await Task.Delay(1000);
        }
    }

    private async Task WaitAndExecute()
    {
        await Task.Delay(Random.Range(m_minTickTime, m_maxTickTime) * 1000);
        
        tick.Invoke();
    }
}
