using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSeizure : IHorrorEvent
{
    public SoundSeizure(Sound sound)
    {
        m_SpawnSound = sound;
    }

    private Sound m_SpawnSound;
    
    public void Execute()
    {
        GameObject.Instantiate(m_SpawnSound);
    }
}
