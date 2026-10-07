using UnityEngine;

public class PortaDeCorrer : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform mesh; // arraste o objeto "Mesh" (a folha da porta)
    [SerializeField] private DetectorDeJogador detector; // arraste o objeto filho com o Sphere Collider

    [Header("Deslize")]
    [SerializeField] private Vector3 direcaoDeslize = Vector3.right; // pra qual lado ela desliza no eixo local da porta
    [SerializeField] private float distanciaDeslize = 2f;
    [SerializeField] private float velocidadeAbertura = 2f;

    private Vector3 posicaoFechada;
    private Vector3 posicaoAberta;

    void Start()
    {
        if (mesh == null)
            Debug.LogWarning("PortaDeCorrer: arraste o objeto 'Mesh' no campo Mesh do Inspector.");

        if (detector == null)
            Debug.LogWarning("PortaDeCorrer: arraste o objeto com o DetectorDeJogador no campo Detector do Inspector.");

        posicaoFechada = mesh.localPosition;

        // MODIFICAÇÃO AQUI: Convertendo a direção global para a direção local do objeto mesh
        Vector3 direcaoLocal = mesh.TransformDirection(direcaoDeslize.normalized);

        // A posicaoAberta agora leva em consideração a rotação da porta
        posicaoAberta = posicaoFechada + direcaoLocal * distanciaDeslize;
    }

    void Update()
    {
        if (mesh == null || detector == null) return;

        Vector3 alvo = detector.jogadorDetectado ? posicaoAberta : posicaoFechada;
        mesh.localPosition = Vector3.Lerp(mesh.localPosition, alvo, velocidadeAbertura * Time.deltaTime);
    }
}