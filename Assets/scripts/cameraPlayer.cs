using UnityEngine;
using UnityEngine.InputSystem;

public class cameraPlayer : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference lookAction;

    [Header("References")]
    [SerializeField] private Transform offsetCamera;

    [Header("Settings")]
    [SerializeField] private float sensitivity = 0.1f;

    [SerializeField] private float minPitch = -90f;
    [SerializeField] private float maxPitch = 90f;

    private float pitch;

    private void OnEnable()
    {
        lookAction.action.Enable();
    }

    private void OnDisable()
    {
        lookAction.action.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleLook();
    }

    private void HandleLook()
    {
        Vector2 lookInput =
            lookAction.action.ReadValue<Vector2>();

        float mouseX = lookInput.x * sensitivity;
        float mouseY = lookInput.y * sensitivity;

        // Rotação horizontal
        transform.Rotate(Vector3.up * mouseX);

        // Rotação vertical
        pitch -= mouseY;
        pitch = Mathf.Clamp(
            pitch,
            minPitch,
            maxPitch
        );

        offsetCamera.localRotation =
            Quaternion.Euler(pitch, 0f, 0f);
    }
}