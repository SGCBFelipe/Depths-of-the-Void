using UnityEngine;
using UnityEngine.InputSystem;

public class DuctSystem : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;

    [Header("Referências do Duto")]
    [SerializeField] private Transform internSpawn;
    [SerializeField] private Transform externSpawn;
    [SerializeField] private GameObject billboard;

    [Header("Gatilhos (Triggers)")]
    [SerializeField] private Collider interactionArea;
    [SerializeField] private Collider exitArea;

    private bool canInteract = false;
    private PlayerDuctHandler currentPlayer;

    private void OnEnable()
    {
        interactAction?.action.Enable();
    }

    private void OnDisable()
    {
        interactAction?.action.Disable();
    }

    private void Start()
    {
        // Garante que o Billboard comece escondido
        if (billboard != null)
            billboard.SetActive(false);
    }

    private void Update()
    {
        // Se o player está na área de entrada e apertou 'E' (ou botão de interagir)
        if (canInteract && currentPlayer != null && !currentPlayer.IsInsideDuct)
        {
            if (interactAction != null && interactAction.action.WasPressedThisFrame())
            {
                EnterDuct();
            }
        }
    }

    private void EnterDuct()
    {
        canInteract = false;
        if (billboard != null) billboard.SetActive(false);

        currentPlayer.EnterDuct(internSpawn);
    }

    // Método chamado quando o player encosta na área de saída (Exit)
    public void TriggerExit(PlayerDuctHandler player)
    {
        if (player != null && player.IsInsideDuct)
        {
            player.ExitDuct(externSpawn);
        }
    }

    // Detecção da Área de Interação (Entrada)
    public void OnPlayerEnterEntrance(PlayerDuctHandler player)
    {
        currentPlayer = player;
        canInteract = true;

        if (billboard != null)
            billboard.SetActive(true);
    }

    public void OnPlayerExitEntrance()
    {
        canInteract = false;

        if (billboard != null)
            billboard.SetActive(false);

        currentPlayer = null;
    }
}