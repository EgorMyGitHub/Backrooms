using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UpdateBakingInRunTime : MonoBehaviour
{
    [SerializeField] private NavMeshSurface faces;
    
    [SerializeField] private bool isTest;

    private List<NavMeshSurface> m_TestUpdate = new List<NavMeshSurface>();

    private NavMeshSurface m_Old;
    
    private void Update()
    {
        if (isTest)
            faces.BuildNavMesh();
    }

    public void UpdateBaking()
    {
        faces.BuildNavMesh();
    }
}
