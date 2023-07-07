using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class NextBots : MonoBehaviour
{
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float sprintDuration;
    [SerializeField] private float sprintCooldown;
    
    [SerializeField] private bool isTest;
    [SerializeField] private bool isTouchTest;

    [SerializeField] private Sound sound;
    
    private NavMeshAgent m_Agent;

    private FirstPersonController m_Player;

    private bool m_IsSprint = false;
    private bool m_IsCooldown = false;
    private bool m_IsSeesPlayer;

    private float m_CurrentSprintDuration;
    private float m_CurrentSprintCooldown;
    private float m_DefaultSpeed;

    private Vector3 oldPlayerPos;
    private Vector3 targetPos = Vector3.zero;
    
    private void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Player = ComponentRoot.Resolve<FirstPersonController>();
        
        m_DefaultSpeed = m_Agent.speed;

        if (isTouchTest)
        {
            m_Agent.acceleration = 50;
            m_Agent.angularSpeed = 360;
            m_DefaultSpeed = 50;
            sprintSpeed = 50;
        }
    }

    private void Update()
    {
        Movement();
        Sprint();
    }

    private void Movement()
    {
        if(m_Player == null)
            return;
        
        var playerPos = m_Player.transform.position;

        if (m_IsSeesPlayer)
            targetPos = new Vector3(playerPos.x, 0, playerPos.z);
        else if(targetPos == Vector3.zero)
        {
            do
            {
                targetPos = new Vector3(playerPos.x + Random.Range(-40, 40), 0,
                    playerPos.z + Random.Range(-40, 40));
            } while (VectorComparison(transform.position, targetPos, 10));
        }

        if (!VectorComparison(playerPos, targetPos, 40))
        {
            targetPos = Vector3.zero;
        }
        
        if (oldPlayerPos != Vector3.zero)
        {
            targetPos = new Vector3(oldPlayerPos.x, 0, oldPlayerPos.z);
        }
        
        if (new Vector2(targetPos.x, targetPos.z) != new Vector2(playerPos.x, playerPos.z) && VectorComparison(transform.position, targetPos, 5) && !m_IsSeesPlayer)
        {
            oldPlayerPos = VectorComparison(oldPlayerPos, targetPos, 5) ? Vector3.zero : oldPlayerPos;
            
            targetPos = Vector3.zero;
        }

        m_Agent.destination = targetPos;
    }

    private bool VectorComparison(Vector3 vector, Vector3 vector1, float value)
    {
        if (Math.Abs(vector.x - vector1.x) < value
            && Math.Abs(vector.z - vector1.z) < value)
        {
            return true;
        }

        return false;
    }
    
    private void Sprint()
    {
        m_CurrentSprintDuration -= Time.deltaTime;
        m_CurrentSprintCooldown -= Time.deltaTime;
        
        if (m_CurrentSprintDuration > 0 && !m_IsCooldown)
        {
            m_Agent.speed = sprintSpeed;
            return;
        }
        else if(!m_IsCooldown)
        {
            m_Agent.speed = m_DefaultSpeed;
            m_CurrentSprintCooldown = sprintCooldown;
            
            m_IsCooldown = true;
        }

        if (m_CurrentSprintCooldown < 0 && m_IsCooldown)
        {
            m_IsCooldown = false;
            m_CurrentSprintDuration = sprintDuration;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<FirstPersonController>(out var player))
        {
            Instantiate(sound);
            player.RotateTo(transform);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<FirstPersonController>(out var player))
        {
            m_IsSeesPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<FirstPersonController>(out var player))
        {
            oldPlayerPos = player.transform.position;
            m_IsSeesPlayer = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.TryGetComponent<FirstPersonController>(out var player))
        {
            if(!isTest)
                Destroy(player.gameObject);
            
            ComponentRoot.Resolve<GameController>().SetGameOver(false);
        }
    }
}
