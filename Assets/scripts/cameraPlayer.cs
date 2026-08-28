using UnityEngine;
using UnityEngine.InputSystem;

public class cameraPlayer : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference lookAction;

    [Header("References")]
    [SerializeField] private Transform offsetCamera; // No Cinemachine, chamamos isso de "CameraRoot" ou "CameraTarget"

    [Header("Settings")]
    [SerializeField] private float sensitivity = 0.5f;

    [SerializeField] private float minPitch = -90f;
    [SerializeField] private float maxPitch = 90f;

    private float pitch;

    private void OnEnable()
    {
        lookAction?.action.Enable();
    }

    private void OnDisable()
    {
        lookAction?.action.Disable();
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
        if (lookAction == null || offsetCamera == null) return;

        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        float mouseX = lookInput.x * sensitivity;
        float mouseY = lookInput.y * sensitivity;

        // Rotação horizontal (Vira o corpo inteiro do Player)
        transform.Rotate(Vector3.up * mouseX);

        // Rotação vertical (Levanta/Abaixa apenas o pescoço/alvo da câmera)
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        offsetCamera.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}