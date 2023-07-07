using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class Screamer : IHorrorEvent
{
    public Screamer(
        Animator camera,
        Animator screamer,
        FirstPersonController player,
        GameObject obj)
    {
        m_Camera = camera;
        m_Screamer = screamer;
        m_Player = player;
        m_SoundObj = obj;
    }

    private FirstPersonController m_Player;
    
    private Animator m_Camera;
    private Animator m_Screamer;
    
    private GameObject m_SoundObj;
    
    public async void Execute()
    {
        var position = m_Camera.transform.position;

        Vector3 randomVector = Vector3.zero;

        while (Math.Abs(randomVector.x) < 2f
            && Math.Abs(randomVector.z) < 2f)
        {
            randomVector = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        }
        
        var pos = position + randomVector;
        
        var randomScream = GameObject.Instantiate(GetVariable.GetRandomScreamers(), pos, Quaternion.identity);

        GameObject.Destroy(randomScream.GetComponent<NextBots>());
        
        m_Player.RotateTo(randomScream.transform);
        
        m_Camera.SetTrigger("Screamer");
        m_Screamer.SetTrigger("Screamer");

        GameObject.Instantiate(m_SoundObj, Vector3.zero, Quaternion.identity);
        
        await Task.Delay(1000);

        GameObject.Destroy(randomScream);
    }
}
