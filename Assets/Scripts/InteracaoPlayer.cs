using UnityEngine;
using UnityEngine.InputSystem;

public class InteracaoPlayer : MonoBehaviour
{
    [Header("Configurações")]
    public float distanciaInteracao = 3f;

    [Tooltip("Atribua a ação de Input no Inspector")]
    [SerializeField] private InputActionReference interact;

    private void OnEnable()
    {
        interact?.action.Enable();
    }

    private void OnDisable()
    {
        interact?.action.Disable();
    }

    private void Update()
    {
        // Lê diretamente se o botão foi pressionado neste frame, sem usar callbacks
        if (interact.action.WasPressedThisFrame())
        {
            ExecutarRaycast();
        }
    }

    private void ExecutarRaycast()
    {
        // Cria um raio partindo do centro da câmera para a frente
        Ray raio = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(raio, out RaycastHit hit, distanciaInteracao))
        {
            // Tenta achar o componente do botão no objeto que o raio atingiu
            BotaoElevador botao = hit.collider.GetComponent<BotaoElevador>();

            if (botao != null && botao.elevador != null)
            {
                // Chama a função de mover passando o índice configurado no botão
                botao.elevador.IrParaAndar(botao.indiceAndar);
            }
        }
    }
}