using UnityEngine;

public class PlayerDuctHandler : MonoBehaviour
{
    [Header("Configurações de Escala no Duto")]
    [Tooltip("Ex: 0.5 vai reduzir o jogador para metade do tamanho original")]
    [SerializeField] private float ductScaleFactor = 0.5f;

    [Header("Ajustes do Controller (Opcional)")]
    [SerializeField] private float ductHeight = 0.6f;
    [SerializeField] private Vector3 ductCenter = new Vector3(0f, 0.3f, 0f);

    private CharacterController controller;

    // Guarda as dimensões e a escala originais
    private float originalHeight;
    private Vector3 originalCenter;
    private Vector3 originalScale;

    public bool IsInsideDuct { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        // Salva os valores originais no início do jogo
        originalHeight = controller.height;
        originalCenter = controller.center;
        originalScale = transform.localScale;
    }

    /// <summary>
    /// Teleporta o jogador para dentro do duto, encolhe sua escala e ajusta a colisão.
    /// </summary>
    public void EnterDuct(Transform spawnPoint)
    {
        IsInsideDuct = true;

        // Desativa o controller para a Unity permitir alterações de física e posição sem conflitos
        controller.enabled = false;

        // Aplica a nova escala (0.5x do tamanho original)
        transform.localScale = originalScale * ductScaleFactor;

        // Reposiciona no ponto de spawn interno
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        // Ajusta a altura e o centro do controller para o duto
        controller.height = ductHeight;
        controller.center = ductCenter;

        // Reativa o controller
        controller.enabled = true;
    }

    /// <summary>
    /// Teleporta o jogador para fora do duto e restaura sua escala e colisão normais.
    /// </summary>
    public void ExitDuct(Transform spawnPoint)
    {
        IsInsideDuct = false;

        controller.enabled = false;

        // Restaura a escala original (1x1x1)
        transform.localScale = originalScale;

        // Reposiciona no ponto de spawn externo
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        // Restaura o controller original
        controller.height = originalHeight;
        controller.center = originalCenter;

        controller.enabled = true;
    }
}
