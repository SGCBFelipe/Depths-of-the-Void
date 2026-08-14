using UnityEngine;
using Unity.Cinemachine; // Necessário para a versão 3 do Cinemachine

[RequireComponent(typeof(CharacterController))]
public class HeadbobSystem : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Arraste a sua FirstPersonCamera (Cinemachine) para cá")]
    [SerializeField] private CinemachineCamera virtualCamera;

    private CharacterController controller;
    private CinemachineBasicMultiChannelPerlin noiseComponent;

    [Header("Configurações de Caminhada")]
    [SerializeField] private float walkAmplitude = 1.0f; // Força do balanço
    [SerializeField] private float walkFrequency = 1.5f; // Velocidade do passo

    [Header("Configurações de Corrida")]
    [SerializeField] private float sprintAmplitude = 2.5f;
    [SerializeField] private float sprintFrequency = 3.0f;

    [Header("Suavização")]
    [SerializeField] private float blendSpeed = 10f; // Quão rápido a câmera começa/para de balançar

    private float targetAmplitude = 0f;
    private float targetFrequency = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        // Pega o componente de Noise de dentro da câmera do Cinemachine
        if (virtualCamera != null)
        {
            noiseComponent = virtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        }
        else
        {
            Debug.LogWarning("CinemachineCamera não atribuída no HeadbobSystem!");
        }
    }

    private void Update()
    {
        if (noiseComponent == null) return;

        CheckMovement();
        ApplyHeadbob();
    }

    private void CheckMovement()
    {
        // Pegamos a velocidade real do jogador apenas nos eixos X e Z (ignorando pulos/quedas)
        Vector3 flatVelocity = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
        float currentSpeed = flatVelocity.magnitude;

        // Verifica se o jogador está no chão e se movendo
        if (currentSpeed > 0.1f && controller.isGrounded)
        {
            // Se a velocidade for maior que 6 (ajuste de acordo com o seu movePlayer), ele está correndo
            bool isSprinting = currentSpeed > 6f;

            targetAmplitude = isSprinting ? sprintAmplitude : walkAmplitude;
            targetFrequency = isSprinting ? sprintFrequency : walkFrequency;
        }
        else
        {
            // Se estiver parado ou no ar, para de balançar
            targetAmplitude = 0f;
            targetFrequency = 0f;
        }
    }

    private void ApplyHeadbob()
    {
        // Interpola suavemente o valor atual para o valor alvo (evita trancos na câmera)
        noiseComponent.AmplitudeGain = Mathf.Lerp(noiseComponent.AmplitudeGain, targetAmplitude, Time.deltaTime * blendSpeed);
        noiseComponent.FrequencyGain = Mathf.Lerp(noiseComponent.FrequencyGain, targetFrequency, Time.deltaTime * blendSpeed);
    }
}