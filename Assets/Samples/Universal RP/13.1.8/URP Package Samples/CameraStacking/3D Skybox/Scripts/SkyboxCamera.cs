using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SkyboxCamera : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float skyboxScale = 1f;

    private Vector3 m_MainCamStartPos;
    private Vector3 m_SkyboxCamStartPos;

    // Start is called before the first frame update
    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        m_MainCamStartPos = mainCamera.transform.position;
        m_SkyboxCamStartPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mainCamDeltaPos = mainCamera.transform.position - m_MainCamStartPos;
        transform.position = m_SkyboxCamStartPos + mainCamDeltaPos * skyboxScale;

        transform.rotation = mainCamera.transform.rotation;
    }
}
