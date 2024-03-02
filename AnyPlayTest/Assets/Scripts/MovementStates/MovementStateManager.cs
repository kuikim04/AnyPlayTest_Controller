using FishNet.Component.Transforming;
using FishNet.Example.Scened;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransform))]
public class MovementStateManager : NetworkBehaviour
{
    [Header("Player Input")]
    public Player PlayerInput;

    #region MovementParameters
    [Header("Movement Parameters")]

    public float CurrentMoveSpeed;
    public float WalkSpeed = 3, WalkBackSpeed = 2;
    public float RunSpeed = 7, RunBackSpeed = 5;
    public float CrouchSpeed = 2, CrouchBackSpeed = 1;
    public float ProneSpeed = 2, ProneBackSpeed = 1;

    CharacterController characterController;
    #endregion

    #region GroundCheckParameters
    [Header("Ground Check Parameters")]

    [SerializeField] Transform groundedCheck;
    [SerializeField] LayerMask groundMask;

    #endregion

    #region GravityParameters
    [Header("Gravity Parameters")]

    [SerializeField] float gravity = -9.81f;
    Vector3 velocity;

    #endregion

    #region JumpParameters
    [Header("Jump Parameters")]

    [SerializeField] float jumpHeight;

    #endregion

    #region StateParameters

    MovementBaseState currentState;

    public IdleState IdleState = new IdleState();
    public WalkState WalkState = new WalkState();
    public CrouchState CrouchState =new CrouchState();
    public RunState RunState = new RunState();
    public ProneState ProneState = new ProneState();

    #endregion

    #region AnimatorParameters

    [HideInInspector] public Animator Animator;

    [SerializeField] private float animationSmoothTime;
    Vector2 currentAnimationBlendVector;
    Vector2 animationVelocity;
    int moveXAnimationParameterID;
    int moveYAnimationParameterID;

    #endregion

    #region CameraParameters

    private Transform cameraTransform;
    private Camera playerCamera;

    #endregion

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (base.IsOwner)
        {
            playerCamera = Camera.main; 
            playerCamera.transform.SetParent(transform);
            cameraTransform = playerCamera.transform;
        }
        else
        {
            gameObject.GetComponent<MovementStateManager>().enabled = false;
        }
    }

    private void Awake()
    {
        PlayerInput = new Player();
    }

    private void OnEnable()
    {
        PlayerInput.Enable();
    }

    private void OnDisable()
    {
        PlayerInput.Disable();
    }

    void Start()
    {
        Animator = GetComponent<Animator>();
        groundedCheck = GetComponent<Transform>();
        characterController = GetComponent<CharacterController>();


        moveXAnimationParameterID = Animator.StringToHash("hzInput");
        moveYAnimationParameterID = Animator.StringToHash("vInput");

        SwitchState(IdleState);
    }

    void Update()
    {
        if (cameraTransform != null && characterController != null
            && Animator != null)
        {
            GetDirectionAndMove();
            GetJump();
            Gravity();

            currentState.UpdateState(this);
        }
    }

    public void SwitchState(MovementBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    void GetDirectionAndMove()
    {
        if (IsGrounded() && velocity.y < 0) // Ensure the character is grounded and reset

        {
            velocity.y = 0f;
        }

        Vector2 movementInput = PlayerInput.PlayerInput.Move.ReadValue<Vector2>();

        currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, movementInput,
            ref animationVelocity, animationSmoothTime); // Smoothly adjust animation blend vector when receiving new input.

        if (cameraTransform != null && characterController != null)
        {
            Vector3 move = new Vector3(currentAnimationBlendVector.x, 0, currentAnimationBlendVector.y);
            move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
            move.y = 0;

            characterController.Move(move * Time.deltaTime * CurrentMoveSpeed);
        }
        else
        {
            Debug.LogWarning("CameraTransform or CharacterController is null. Movement not applied.", this);
        }


        Animator.SetFloat(moveXAnimationParameterID, currentAnimationBlendVector.x);
        Animator.SetFloat(moveYAnimationParameterID, currentAnimationBlendVector.y);
    }

    void GetJump()
    {
        if (PlayerInput.PlayerInput.Jump.triggered && IsGrounded())
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        }
    }
    bool IsGrounded()
    {
        return Physics.CheckSphere(groundedCheck.position, 0.1f, groundMask);
    }
    void Gravity()
    {
        if (!IsGrounded()) velocity.y += gravity * Time.deltaTime; 
        else if (velocity.y < 0) velocity.y = -2; // Apply gravity only if the character is not grounded

        characterController.Move(velocity * Time.deltaTime); 
    }
}
