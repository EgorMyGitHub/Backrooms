// CHANGE LOG
// 
// CHANGES || version VERSION
//
// "Enable/Disable Headbob, Changed look rotations - should result in reduced camera jitters" || version 1.0.1

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

#if UNITY_EDITOR
    using UnityEditor;
    using System.Net;
#endif

public class FirstPersonController : MonoBehaviour
{
    private Rigidbody m_Rb;
    
    #region Camera Movement Variables

    public Camera playerCamera;

    public float fov = 60f;
    public bool invertCamera = false;
    public bool cameraCanMove = true;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;

    // Crosshair
    public bool lockCursor = true;
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;

    // Internal Variables
    private float m_Yaw = 0.0f;
    private float m_Pitch = 0.0f;
    private Image m_CrosshairObject;

    #region Camera Zoom Variables

    public bool enableZoom = true;
    public bool holdToZoom = false;
    public KeyCode zoomKey = KeyCode.Mouse1;
    public float zoomFOV = 30f;
    public float zoomStepTime = 5f;

    // Internal Variables
    private bool m_IsZoomed = false;

    #endregion
    #endregion

    #region Movement Variables

    public bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float maxVelocityChange = 10f;

    // Internal Variables
    public bool isWalking = false;

    #region Sprint

    public bool enableSprint = true;
    public bool unlimitedSprint = false;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float sprintSpeed = 7f;
    public float sprintDuration = 5f;
    public float sprintCooldown = .5f;
    public float sprintFOV = 80f;
    public float sprintFOVStepTime = 10f;

    // Sprint Bar
    public bool useSprintBar = true;
    public bool hideBarWhenFull = true;
    public Image sprintBarBg;
    public Image sprintBar;
    public float sprintBarWidthPercent = .3f;
    public float sprintBarHeightPercent = .015f;

    // Internal Variables
    private CanvasGroup m_SprintBarCg;
    public bool isSprinting = false;
    private float m_SprintRemaining;
    private float m_SprintBarWidth;
    private float m_SprintBarHeight;
    private bool m_IsSprintCooldown = false;
    private float m_SprintCooldownReset;

    private bool m_IsCursorLock;

    #endregion

    #region Jump

    public bool enableJump = true;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpPower = 5f;

    // Internal Variables
    private bool m_IsGrounded = false;

    #endregion

    #region Crouch

    public bool enableCrouch = true;
    public bool holdToCrouch = true;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public float crouchHeight = .75f;
    public float speedReduction = .5f;

    // Internal Variables
    private bool m_IsCrouched = false;
    private Vector3 m_OriginalScale;

    #endregion
    #endregion

    #region Head Bob

    public bool enableHeadBob = true;
    public Transform joint;
    public float bobSpeed = 10f;
    public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);

    // Internal Variables
    private Vector3 m_JointOriginalPos;
    private float m_Timer = 0;

    #endregion

    public GameObject glitch;
    
    private const float speedRot = 10;
    
    private bool isRotateTo = false;

    private Vector3 rotateTo;
    private float oldCameraRotY;

    private float rotateLost;

    private Vector3 glitchStartSize;
    
    private void Awake()
    {
        glitchStartSize = glitch.transform.localScale;
        
        m_Rb = GetComponent<Rigidbody>();

        m_CrosshairObject = GetComponentInChildren<Image>();

        // Set internal variables
        playerCamera.fieldOfView = fov;
        m_OriginalScale = transform.localScale;
        m_JointOriginalPos = joint.localPosition;

        if (!unlimitedSprint)
        {
            m_SprintRemaining = sprintDuration;
            m_SprintCooldownReset = sprintCooldown;
        }
    }

    void Start()
    {
        if(lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            m_IsCursorLock = true;
        }

        if(crosshair)
        {
            m_CrosshairObject.sprite = crosshairImage;
            m_CrosshairObject.color = crosshairColor;
        }

        #region Sprint Bar

        m_SprintBarCg = GetComponentInChildren<CanvasGroup>();

        if(useSprintBar)
        {
            sprintBarBg.gameObject.SetActive(true);
            sprintBar.gameObject.SetActive(true);

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            m_SprintBarWidth = screenWidth * sprintBarWidthPercent;
            m_SprintBarHeight = screenHeight * sprintBarHeightPercent;

            sprintBarBg.rectTransform.sizeDelta = new Vector3(m_SprintBarWidth, m_SprintBarHeight, 0f);
            sprintBar.rectTransform.sizeDelta = new Vector3(m_SprintBarWidth - 2, m_SprintBarHeight - 2, 0f);

            if(hideBarWhenFull)
            {
                m_SprintBarCg.alpha = 0;
            }
        }
        else
        {
            //sprintBarBG.gameObject.SetActive(false);
            //sprintBar.gameObject.SetActive(false);
        }

        #endregion
    }

    float m_CamRotation;

    private void SetLockCursor(bool value)
    {
        Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !value;
        m_IsCursorLock = value;

        cameraCanMove = value;
    }
    
    private void Update()
    {
        #region Camera

        if (Input.GetKeyDown(KeyCode.F1))
        {
            SetLockCursor(!m_IsCursorLock);
        }
            
        
        // Control camera movement
        if(cameraCanMove)
        {
            m_Yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;

            if (!invertCamera)
            {
                m_Pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
            }
            else
            {
                // Inverted Y
                m_Pitch += mouseSensitivity * Input.GetAxis("Mouse Y");
            }

            // Clamp pitch between lookAngle
            m_Pitch = Mathf.Clamp(m_Pitch, -maxLookAngle, maxLookAngle);

            transform.localEulerAngles = new Vector3(0, m_Yaw, 0);
            playerCamera.transform.localEulerAngles = new Vector3(m_Pitch, 0, 0);
        }

        if (isRotateTo)
        {
            var target = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, rotateTo.y, 0), speedRot * Time.deltaTime);

            rotateLost -= Math.Abs(oldCameraRotY - target.eulerAngles.y);

            oldCameraRotY = target.eulerAngles.y;
            
            m_Yaw = target.eulerAngles.y;

            transform.localEulerAngles = new Vector3(0, m_Yaw, 0);

            if (rotateLost <= 0)
                isRotateTo = false;
        }

        glitch.transform.localScale = new Vector3(glitchStartSize.x * playerCamera.fieldOfView / fov, glitchStartSize.y, glitchStartSize.z * playerCamera.fieldOfView / fov);

        #region Camera Zoom

        if (enableZoom)
        {
            // Changes isZoomed when key is pressed
            // Behavior for toogle zoom
            if(Input.GetKeyDown(zoomKey) && !holdToZoom && !isSprinting)
            {
                if (!m_IsZoomed)
                {
                    m_IsZoomed = true;
                }
                else
                {
                    m_IsZoomed = false;
                }
            }

            // Changes isZoomed when key is pressed
            // Behavior for hold to zoom
            if(holdToZoom && !isSprinting)
            {
                if(Input.GetKeyDown(zoomKey))
                {
                    m_IsZoomed = true;
                }
                else if(Input.GetKeyUp(zoomKey))
                {
                    m_IsZoomed = false;
                }
            }

            // Lerps camera.fieldOfView to allow for a smooth transistion
            if(m_IsZoomed)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomFOV, zoomStepTime * Time.deltaTime);
            }
            else if(!m_IsZoomed && !isSprinting)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, zoomStepTime * Time.deltaTime);
            }
        }

        #endregion
        #endregion

        #region Sprint

        if(enableSprint)
        {
            if(isSprinting)
            {
                m_IsZoomed = false;
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVStepTime * Time.deltaTime);

                // Drain sprint remaining while sprinting
                if(!unlimitedSprint)
                {
                    m_SprintRemaining -= 1 * Time.deltaTime;
                    if (m_SprintRemaining <= 0)
                    {
                        isSprinting = false;
                        m_IsSprintCooldown = true;
                    }
                }
            }
            else
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, sprintFOVStepTime * Time.deltaTime);
                m_SprintRemaining = Mathf.Clamp(m_SprintRemaining += 1 * Time.deltaTime, 0, sprintDuration);
            }

            // Handles sprint cooldown 
            // When sprint remaining == 0 stops sprint ability until hitting cooldown
            if(m_IsSprintCooldown)
            {
                sprintCooldown -= 1 * Time.deltaTime;
                if (sprintCooldown <= 0)
                {
                    m_IsSprintCooldown = false;
                }
            }
            else
            {
                sprintCooldown = m_SprintCooldownReset;
            }

            // Handles sprintBar 
            if(useSprintBar && !unlimitedSprint)
            {
                float sprintRemainingPercent = m_SprintRemaining / sprintDuration;
                sprintBar.transform.localScale = new Vector3(sprintRemainingPercent, 1f, 1f);
            }
        }

        #endregion

        #region Jump

        // Gets input and calls jump method
        if(enableJump && Input.GetKeyDown(jumpKey) && m_IsGrounded)
        {
            Jump();
        }

        #endregion

        #region Crouch

        if (enableCrouch)
        {
            if(Input.GetKeyDown(crouchKey) && !holdToCrouch)
            {
                Crouch();
            }
            
            if(Input.GetKeyDown(crouchKey) && holdToCrouch)
            {
                m_IsCrouched = false;
                Crouch();
            }
            else if(Input.GetKeyUp(crouchKey) && holdToCrouch)
            {
                m_IsCrouched = true;
                Crouch();
            }
        }

        #endregion

        CheckGround();

        if(enableHeadBob)
        {
            HeadBob();
        }
    }

    public void RotateTo(Transform to)
    {
        isRotateTo = true;
        
        var direction = (to.position - transform.position).normalized;
        
        rotateTo = Quaternion.LookRotation(direction).eulerAngles;
        
        oldCameraRotY = playerCamera.transform.eulerAngles.y;

        rotateLost = Math.Abs(rotateTo.y - oldCameraRotY);
    }
    
    void FixedUpdate()
    {
        #region Movement

        if (playerCanMove)
        {
            // Calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            // Checks if player is walking and isGrounded
            // Will allow head bob
            if (targetVelocity.x != 0 || targetVelocity.z != 0 && m_IsGrounded)
            {
                isWalking = true;
            }
            else
            {
                isWalking = false;
            }

            // All movement calculations shile sprint is active
            if (enableSprint && Input.GetKey(sprintKey) && m_SprintRemaining > 0f && !m_IsSprintCooldown)
            {
                targetVelocity = transform.TransformDirection(targetVelocity) * sprintSpeed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = m_Rb.velocity;
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;

                // Player is only moving when valocity change != 0
                // Makes sure fov change only happens during movement
                if (velocityChange.x != 0 || velocityChange.z != 0)
                {
                    isSprinting = true;

                    if (m_IsCrouched)
                    {
                        Crouch();
                    }

                    if (hideBarWhenFull && !unlimitedSprint)
                    {
//                        sprintBarCG.alpha += 5 * Time.deltaTime;
                    }
                }

                m_Rb.AddForce(velocityChange, ForceMode.VelocityChange);
            }
            // All movement calculations while walking
            else
            {
                isSprinting = false;

                if (hideBarWhenFull && m_SprintRemaining == sprintDuration)
                {
                    //sprintBarCG.alpha -= 3 * Time.deltaTime;
                }

                targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = m_Rb.velocity;
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;

                m_Rb.AddForce(velocityChange, ForceMode.VelocityChange);
            }
        }

        #endregion
    }
    
    // Sets isGrounded based on a raycast sent straigth down from the player object
    private void CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = .75f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            Debug.DrawRay(origin, direction * distance, Color.red);
            m_IsGrounded = true;
        }
        else
        {
            m_IsGrounded = false;
        }
    }
    
    private void Jump()
    {
        // Adds force to the player rigidbody to jump
        if (m_IsGrounded)
        {
            m_Rb.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);
            m_IsGrounded = false;
        }

        // When crouched and using toggle system, will uncrouch for a jump
        if(m_IsCrouched && !holdToCrouch)
        {
            Crouch();
        }
    }

    private void Crouch()
    {
        // Stands player up to full height
        // Brings walkSpeed back up to original speed
        if(m_IsCrouched)
        {
            transform.localScale = new Vector3(m_OriginalScale.x, m_OriginalScale.y, m_OriginalScale.z);
            walkSpeed /= speedReduction;

            m_IsCrouched = false;
        }
        // Crouches player down to set height
        // Reduces walkSpeed
        else
        {
            transform.localScale = new Vector3(m_OriginalScale.x, crouchHeight, m_OriginalScale.z);
            walkSpeed *= speedReduction;

            m_IsCrouched = true;
        }
    }

    private void HeadBob()
    {
        if(isWalking)
        {
            // Calculates HeadBob speed during sprint
            if(isSprinting)
            {
                m_Timer += Time.deltaTime * (bobSpeed + sprintSpeed);
            }
            // Calculates HeadBob speed during crouched movement
            else if (m_IsCrouched)
            {
                m_Timer += Time.deltaTime * (bobSpeed * speedReduction);
            }
            // Calculates HeadBob speed during walking
            else
            {
                m_Timer += Time.deltaTime * bobSpeed;
            }
            // Applies HeadBob movement
            joint.localPosition = new Vector3(m_JointOriginalPos.x + Mathf.Sin(m_Timer) * bobAmount.x, m_JointOriginalPos.y + Mathf.Sin(m_Timer) * bobAmount.y, m_JointOriginalPos.z + Mathf.Sin(m_Timer) * bobAmount.z);
        }
        else
        {
            // Resets when play stops moving
            m_Timer = 0;
            joint.localPosition = new Vector3(Mathf.Lerp(joint.localPosition.x, m_JointOriginalPos.x, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.y, m_JointOriginalPos.y, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.z, m_JointOriginalPos.z, Time.deltaTime * bobSpeed));
        }
    }
}



// Custom Editor
#if UNITY_EDITOR
    [CustomEditor(typeof(FirstPersonController)), InitializeOnLoadAttribute]
    public class FirstPersonControllerEditor : Editor
    {
    FirstPersonController m_Fpc;
    SerializedObject m_SerFpc;

    private void OnEnable()
    {
        m_Fpc = (FirstPersonController)target;
        m_SerFpc = new SerializedObject(m_Fpc);
    }

    public override void OnInspectorGUI()
    {
        m_SerFpc.Update();

        EditorGUILayout.Space();
        GUILayout.Label("Modular First Person Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
        GUILayout.Label("By Jess Case", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        GUILayout.Label("version 1.0.1", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        EditorGUILayout.Space();

        #region Camera Setup

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Camera Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        m_Fpc.playerCamera = (Camera)EditorGUILayout.ObjectField(new GUIContent("Camera", "Camera attached to the controller."), m_Fpc.playerCamera, typeof(Camera), true);
        m_Fpc.fov = EditorGUILayout.Slider(new GUIContent("Field of View", "The camera’s view angle. Changes the player camera directly."), m_Fpc.fov, m_Fpc.zoomFOV, 179f);
        m_Fpc.cameraCanMove = EditorGUILayout.ToggleLeft(new GUIContent("Enable Camera Rotation", "Determines if the camera is allowed to move."), m_Fpc.cameraCanMove);

        GUI.enabled = m_Fpc.cameraCanMove;
        m_Fpc.invertCamera = EditorGUILayout.ToggleLeft(new GUIContent("Invert Camera Rotation", "Inverts the up and down movement of the camera."), m_Fpc.invertCamera);
        m_Fpc.mouseSensitivity = EditorGUILayout.Slider(new GUIContent("Look Sensitivity", "Determines how sensitive the mouse movement is."), m_Fpc.mouseSensitivity, .1f, 10f);
        m_Fpc.maxLookAngle = EditorGUILayout.Slider(new GUIContent("Max Look Angle", "Determines the max and min angle the player camera is able to look."), m_Fpc.maxLookAngle, 40, 90);
        GUI.enabled = true;

        m_Fpc.lockCursor = EditorGUILayout.ToggleLeft(new GUIContent("Lock and Hide Cursor", "Turns off the cursor visibility and locks it to the middle of the screen."), m_Fpc.lockCursor);

        m_Fpc.crosshair = EditorGUILayout.ToggleLeft(new GUIContent("Auto Crosshair", "Determines if the basic crosshair will be turned on, and sets is to the center of the screen."), m_Fpc.crosshair);

        // Only displays crosshair options if crosshair is enabled
        if(m_Fpc.crosshair) 
        { 
            EditorGUI.indentLevel++; 
            EditorGUILayout.BeginHorizontal(); 
            EditorGUILayout.PrefixLabel(new GUIContent("Crosshair Image", "Sprite to use as the crosshair.")); 
            m_Fpc.crosshairImage = (Sprite)EditorGUILayout.ObjectField(m_Fpc.crosshairImage, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            m_Fpc.crosshairColor = EditorGUILayout.ColorField(new GUIContent("Crosshair Color", "Determines the color of the crosshair."), m_Fpc.crosshairColor);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--; 
        }

        EditorGUILayout.Space();

        #region Camera Zoom Setup

        GUILayout.Label("Zoom", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        m_Fpc.enableZoom = EditorGUILayout.ToggleLeft(new GUIContent("Enable Zoom", "Determines if the player is able to zoom in while playing."), m_Fpc.enableZoom);

        GUI.enabled = m_Fpc.enableZoom;
        m_Fpc.holdToZoom = EditorGUILayout.ToggleLeft(new GUIContent("Hold to Zoom", "Requires the player to hold the zoom key instead if pressing to zoom and unzoom."), m_Fpc.holdToZoom);
        m_Fpc.zoomKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Zoom Key", "Determines what key is used to zoom."), m_Fpc.zoomKey);
        m_Fpc.zoomFOV = EditorGUILayout.Slider(new GUIContent("Zoom FOV", "Determines the field of view the camera zooms to."), m_Fpc.zoomFOV, .1f, m_Fpc.fov);
        m_Fpc.zoomStepTime = EditorGUILayout.Slider(new GUIContent("Step Time", "Determines how fast the FOV transitions while zooming in."), m_Fpc.zoomStepTime, .1f, 10f);
        GUI.enabled = true;

        #endregion

        #endregion

        #region Movement Setup

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Movement Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        m_Fpc.playerCanMove = EditorGUILayout.ToggleLeft(new GUIContent("Enable Player Movement", "Determines if the player is allowed to move."), m_Fpc.playerCanMove);

        GUI.enabled = m_Fpc.playerCanMove;
        m_Fpc.walkSpeed = EditorGUILayout.Slider(new GUIContent("Walk Speed", "Determines how fast the player will move while walking."), m_Fpc.walkSpeed, .1f, m_Fpc.sprintSpeed);
        GUI.enabled = true;

        EditorGUILayout.Space();

        #region Sprint

        GUILayout.Label("Sprint", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        m_Fpc.enableSprint = EditorGUILayout.ToggleLeft(new GUIContent("Enable Sprint", "Determines if the player is allowed to sprint."), m_Fpc.enableSprint);

        GUI.enabled = m_Fpc.enableSprint;
        m_Fpc.unlimitedSprint = EditorGUILayout.ToggleLeft(new GUIContent("Unlimited Sprint", "Determines if 'Sprint Duration' is enabled. Turning this on will allow for unlimited sprint."), m_Fpc.unlimitedSprint);
        m_Fpc.sprintKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Sprint Key", "Determines what key is used to sprint."), m_Fpc.sprintKey);
        m_Fpc.sprintSpeed = EditorGUILayout.Slider(new GUIContent("Sprint Speed", "Determines how fast the player will move while sprinting."), m_Fpc.sprintSpeed, m_Fpc.walkSpeed, 20f);

        //GUI.enabled = !fpc.unlimitedSprint;
        m_Fpc.sprintDuration = EditorGUILayout.Slider(new GUIContent("Sprint Duration", "Determines how long the player can sprint while unlimited sprint is disabled."), m_Fpc.sprintDuration, 1f, 20f);
        m_Fpc.sprintCooldown = EditorGUILayout.Slider(new GUIContent("Sprint Cooldown", "Determines how long the recovery time is when the player runs out of sprint."), m_Fpc.sprintCooldown, .1f, m_Fpc.sprintDuration);
        //GUI.enabled = true;

        m_Fpc.sprintFOV = EditorGUILayout.Slider(new GUIContent("Sprint FOV", "Determines the field of view the camera changes to while sprinting."), m_Fpc.sprintFOV, m_Fpc.fov, 179f);
        m_Fpc.sprintFOVStepTime = EditorGUILayout.Slider(new GUIContent("Step Time", "Determines how fast the FOV transitions while sprinting."), m_Fpc.sprintFOVStepTime, .1f, 20f);

        m_Fpc.useSprintBar = EditorGUILayout.ToggleLeft(new GUIContent("Use Sprint Bar", "Determines if the default sprint bar will appear on screen."), m_Fpc.useSprintBar);

        // Only displays sprint bar options if sprint bar is enabled
        if(m_Fpc.useSprintBar)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            m_Fpc.hideBarWhenFull = EditorGUILayout.ToggleLeft(new GUIContent("Hide Full Bar", "Hides the sprint bar when sprint duration is full, and fades the bar in when sprinting. Disabling this will leave the bar on screen at all times when the sprint bar is enabled."), m_Fpc.hideBarWhenFull);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Bar BG", "Object to be used as sprint bar background."));
            m_Fpc.sprintBarBg = (Image)EditorGUILayout.ObjectField(m_Fpc.sprintBarBg, typeof(Image), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Bar", "Object to be used as sprint bar foreground."));
            m_Fpc.sprintBar = (Image)EditorGUILayout.ObjectField(m_Fpc.sprintBar, typeof(Image), true);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            m_Fpc.sprintBarWidthPercent = EditorGUILayout.Slider(new GUIContent("Bar Width", "Determines the width of the sprint bar."), m_Fpc.sprintBarWidthPercent, .1f, .5f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            m_Fpc.sprintBarHeightPercent = EditorGUILayout.Slider(new GUIContent("Bar Height", "Determines the height of the sprint bar."), m_Fpc.sprintBarHeightPercent, .001f, .025f);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }
        GUI.enabled = true;

        EditorGUILayout.Space();

        #endregion

        #region Jump

        GUILayout.Label("Jump", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        m_Fpc.enableJump = EditorGUILayout.ToggleLeft(new GUIContent("Enable Jump", "Determines if the player is allowed to jump."), m_Fpc.enableJump);

        GUI.enabled = m_Fpc.enableJump;
        m_Fpc.jumpKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Jump Key", "Determines what key is used to jump."), m_Fpc.jumpKey);
        m_Fpc.jumpPower = EditorGUILayout.Slider(new GUIContent("Jump Power", "Determines how high the player will jump."), m_Fpc.jumpPower, .1f, 20f);
        GUI.enabled = true;

        EditorGUILayout.Space();

        #endregion

        #region Crouch

        GUILayout.Label("Crouch", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

        m_Fpc.enableCrouch = EditorGUILayout.ToggleLeft(new GUIContent("Enable Crouch", "Determines if the player is allowed to crouch."), m_Fpc.enableCrouch);

        GUI.enabled = m_Fpc.enableCrouch;
        m_Fpc.holdToCrouch = EditorGUILayout.ToggleLeft(new GUIContent("Hold To Crouch", "Requires the player to hold the crouch key instead if pressing to crouch and uncrouch."), m_Fpc.holdToCrouch);
        m_Fpc.crouchKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Crouch Key", "Determines what key is used to crouch."), m_Fpc.crouchKey);
        m_Fpc.crouchHeight = EditorGUILayout.Slider(new GUIContent("Crouch Height", "Determines the y scale of the player object when crouched."), m_Fpc.crouchHeight, .1f, 1);
        m_Fpc.speedReduction = EditorGUILayout.Slider(new GUIContent("Speed Reduction", "Determines the percent 'Walk Speed' is reduced by. 1 being no reduction, and .5 being half."), m_Fpc.speedReduction, .1f, 1);
        GUI.enabled = true;

        #endregion

        #endregion

        #region Head Bob

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Head Bob Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        m_Fpc.enableHeadBob = EditorGUILayout.ToggleLeft(new GUIContent("Enable Head Bob", "Determines if the camera will bob while the player is walking."), m_Fpc.enableHeadBob);
        

        GUI.enabled = m_Fpc.enableHeadBob;
        m_Fpc.joint = (Transform)EditorGUILayout.ObjectField(new GUIContent("Camera Joint", "Joint object position is moved while head bob is active."), m_Fpc.joint, typeof(Transform), true);
        m_Fpc.bobSpeed = EditorGUILayout.Slider(new GUIContent("Speed", "Determines how often a bob rotation is completed."), m_Fpc.bobSpeed, 1, 20);
        m_Fpc.bobAmount = EditorGUILayout.Vector3Field(new GUIContent("Bob Amount", "Determines the amount the joint moves in both directions on every axes."), m_Fpc.bobAmount);
        GUI.enabled = true;

        #endregion

        m_Fpc.glitch = (GameObject)EditorGUILayout.ObjectField("glitch", m_Fpc.glitch, typeof(GameObject));
        
        //Sets any changes from the prefab
        if(GUI.changed)
        {
            EditorUtility.SetDirty(m_Fpc);
            Undo.RecordObject(m_Fpc, "FPC Change");
            m_SerFpc.ApplyModifiedProperties();
        }
    }

}

#endif