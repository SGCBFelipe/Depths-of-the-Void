using UnityEngine;
using UnityEngine.InputSystem;

public class InteracaoPlayer : MonoBehaviour
{
    public float distanciaInteracao = 3f;
    public InputActionReference interact;

    private BotaoElevador botaoAtualMirado; // Guarda o botão que estamos olhando no momento

    private void OnEnable() => interact.action.Enable();
    private void OnDisable() => interact.action.Disable();

    private void Update()
    {
        ExecutarRaycastVisao();

        // Se clicou e estamos olhando para um botão, ativa a função de clique
        if (interact.action.WasPressedThisFrame() && botaoAtualMirado != null)
        {
            botaoAtualMirado.AoClicar();
        }
    }

    private void ExecutarRaycastVisao()
    {
        Ray raio = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(raio, out RaycastHit hit, distanciaInteracao))
        {
            BotaoElevador botaoVisto = hit.collider.GetComponent<BotaoElevador>();

            // Se olhamos para um botão novo
            if (botaoVisto != botaoAtualMirado)
            {
                if (botaoAtualMirado != null) botaoAtualMirado.AoTirarMira(); // Apaga o anterior
                botaoAtualMirado = botaoVisto;
                if (botaoAtualMirado != null) botaoAtualMirado.AoMirar();     // Acende o novo
            }
        }
        else
        {
            // Se olhamos para o nada, apaga o último botão mirado
            if (botaoAtualMirado != null)
            {
                botaoAtualMirado.AoTirarMira();
                botaoAtualMirado = null;
            }
        }
    }
}