using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private Text progressText;

    [SerializeField] private GameObject loadingObj;
    
    private SpawnController m_Controller;

    private float m_StartCount;
    
    private void Start()
    {
        loadingObj.SetActive(true);
        
        m_Controller = ComponentRoot.Resolve<SpawnController>();
        m_StartCount = m_Controller.countSpawn;
    }

    private void Update()
    {
        if(m_Controller.isSpawned)
            Destroy(gameObject);

        progressBar.value = (m_StartCount - m_Controller.countSpawn) / m_StartCount;
        progressText.text = $"{(100 * (m_StartCount - m_Controller.countSpawn) / m_StartCount)}%";
    }
}
