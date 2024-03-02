using Cinemachine;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : NetworkBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] Transform camFollowPos;
    [SerializeField] private float rotationSpeed = 5f;
    float xAxis, yAxis;

    [Header("Input Settings")]
    Player playerInput;
    Vector2 look;

    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineVirtualCamera idleCam;
    [SerializeField] private CinemachineVirtualCamera crouchCam;
    [SerializeField] private CinemachineVirtualCamera proneCam;

    [Header("Animator")]
    Animator anim;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
        }
        else
        {
            GetComponent<PlayerCameraController>().enabled = false;
        }
    }
    private void Awake()
    {
        playerInput = new Player();
        anim = GetComponentInChildren<Animator>();
    }
    private void OnEnable()
    {
        playerInput.Enable();
        playerInput.PlayerInput.Look.performed += OnLookPerformed;
        playerInput.PlayerInput.Look.canceled += OnLookCanceled;
    }

    private void OnDisable()
    {
        playerInput.Disable();
        playerInput.PlayerInput.Look.performed -= OnLookPerformed;
        playerInput.PlayerInput.Look.canceled -= OnLookCanceled;
    }

    void Update()
    {
        if (anim == null)
        {
            Debug.LogError("Animator is null in PlayerCameraController!", this);
            return;
        }

        UpdateCameraState();
    }

    private void LateUpdate()
    {
        if (camFollowPos == null)
        {
            Debug.LogError("camFollowPos is null in PlayerCameraController!", this);
            return;
        }

        CameraRotation();
    }

    void CameraRotation()
    {
        xAxis += look.x * rotationSpeed * Time.deltaTime;
        yAxis -= look.y * rotationSpeed * Time.deltaTime;

        yAxis = Mathf.Clamp(yAxis, -30f, 70f);

        camFollowPos.localEulerAngles = new Vector3(yAxis, camFollowPos.localEulerAngles.y, camFollowPos.localEulerAngles.z);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, xAxis, transform.eulerAngles.z);
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        look = Vector2.zero;
    }

    void UpdateCameraState()
    {
        if (!IsOwner || crouchCam == null || proneCam == null || idleCam == null)
        {
            return;
        }

        if (anim.GetBool("Crouching"))
        {
            SwitchCamera(crouchCam);
        }
        else if (anim.GetBool("Prone"))
        {
            SwitchCamera(proneCam);
        }
        else
        {
            SwitchCamera(idleCam);
        }
    }
    private void SwitchCamera(CinemachineVirtualCamera targetCamera)
    {
        CinemachineBrain cinemachineBrain = FindObjectOfType<CinemachineBrain>();

        targetCamera.Priority = 11;

        // Remove priority from the other cameras
        CinemachineVirtualCamera otherCamera1 = targetCamera == crouchCam ? idleCam : crouchCam;
        CinemachineVirtualCamera otherCamera2 = targetCamera == proneCam ? idleCam : proneCam;

        SetCameraPriority(otherCamera1, 10);
        SetCameraPriority(otherCamera2, 10);

        // Force an immediate update to activate the new camera
        cinemachineBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
        cinemachineBrain.ManualUpdate();
    }

    //Set the priority for each camera.
    private void SetCameraPriority(CinemachineVirtualCamera camera, int priority)
    {
        camera.Priority = priority;
    }

}
