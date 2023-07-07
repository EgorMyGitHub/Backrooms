using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestNavMesh : MonoBehaviour
{
    [SerializeField] private Transform goPoint;

    private NavMeshAgent m_Agent;

    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        m_Agent.destination = goPoint.position;
    }
}
