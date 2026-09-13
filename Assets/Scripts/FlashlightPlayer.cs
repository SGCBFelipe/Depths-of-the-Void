using UnityEngine;
using UnityEngine.InputSystem;
public class Flashlight : MonoBehaviour
{
    public Light flashlightLight;
    [SerializeField] private InputActionReference flashlight;

    private void OnEnable()
    {
        flashlight?.action.Enable();
    }

    private void OnDisable()
    {
        flashlight?.action.Disable();
    }

    void Update()
    {
        // apertar F
        if (flashlight.action.WasPressedThisFrame())
        {
            flashlightLight.enabled = !flashlightLight.enabled;
        }
    }
}
