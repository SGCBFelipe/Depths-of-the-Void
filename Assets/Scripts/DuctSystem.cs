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

    [Header("Segurança (Failsafe)")]
    [Tooltip("Distância máxima (em metros) que o jogador pode se afastar antes de cancelar a interação")]
    [SerializeField] private float maxInteractDistance = 3f;

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
        if (billboard != null)
            billboard.SetActive(false);
    }

    private void Update()
    {
        if (canInteract && currentPlayer != null && !currentPlayer.IsInsideDuct)
        {
            // Mede a distância até o limite físico do colisor, e não o centro do objeto
            float distance = Vector3.Distance(currentPlayer.transform.position, interactionArea.ClosestPoint(currentPlayer.transform.position));

            if (distance > maxInteractDistance)
            {
                OnPlayerExitEntrance();
                return;
            }

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

        // Salva a referência local e a limpa globalmente para evitar bugs de memória ao teleportar
        PlayerDuctHandler playerToEnter = currentPlayer;
        currentPlayer = null;

        playerToEnter.EnterDuct(internSpawn);
    }

    public void TriggerExit(PlayerDuctHandler player)
    {
        if (player != null && player.IsInsideDuct)
        {
            player.ExitDuct(externSpawn);
        }
    }

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