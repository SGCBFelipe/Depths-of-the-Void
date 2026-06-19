using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class movePlayer : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference jumpAction;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController controller;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (moveAction == null)
            Debug.LogError("Move Action não foi atribuída!", this);

        if (sprintAction == null)
            Debug.LogError("Sprint Action não foi atribuída!", this);

        if (jumpAction == null)
            Debug.LogError("Jump Action não foi atribuída!", this);
    }

    private void OnEnable()
    {
        moveAction?.action.Enable();
        sprintAction?.action.Enable();
        jumpAction?.action.Enable();
    }

    private void OnDisable()
    {
        moveAction?.action.Disable();
        sprintAction?.action.Disable();
        jumpAction?.action.Disable();
    }

    private void Update()
    {
        HandleMovement();
        HandleJumpAndGravity();
    }

    private void HandleMovement()
    {
        if (moveAction == null)
            return;

        Vector2 input = moveAction.action.ReadValue<Vector2>();

        bool isSprinting =
            sprintAction != null &&
            sprintAction.action.IsPressed();

        float currentSpeed =
            isSprinting ? sprintSpeed : walkSpeed;

        Vector3 moveDirection =
            transform.right * input.x +
            transform.forward * input.y;

        controller.Move(
            moveDirection *
            currentSpeed *
            Time.deltaTime
        );

#if UNITY_EDITOR
        if (input != Vector2.zero)
        {
            Debug.Log($"Input: {input}");
        }
#endif
    }

    private void HandleJumpAndGravity()
    {
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (
                jumpAction != null &&
                jumpAction.action.WasPressedThisFrame()
            )
            {
                verticalVelocity =
                    Mathf.Sqrt(
                        jumpHeight *
                        -2f *
                        gravity
                    );
            }
        }

        verticalVelocity += gravity * Time.deltaTime;

        controller.Move(
            Vector3.up *
            verticalVelocity *
            Time.deltaTime
        );
    }
}