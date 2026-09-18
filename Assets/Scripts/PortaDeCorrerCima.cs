using UnityEngine;

public class PortaDeCorrerCima : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform mesh; // arraste o objeto "Mesh" (a folha da porta)
    [SerializeField] private DetectorDeJogador detector; // arraste o objeto filho com o Sphere Collider

    [Header("Deslize")]
    [SerializeField] private Vector3 direcaoDeslize = Vector3.up; // desliza pra cima
    [SerializeField] private float distanciaDeslize = 2f;
    [SerializeField] private float velocidadeAbertura = 2f;

    private Vector3 posicaoFechada;
    private Vector3 posicaoAberta;

    void Start()
    {
        if (mesh == null)
            Debug.LogWarning("PortaDeCorrerCima: arraste o objeto 'Mesh' no campo Mesh do Inspector.");

        if (detector == null)
            Debug.LogWarning("PortaDeCorrerCima: arraste o objeto com o DetectorDeJogador no campo Detector do Inspector.");

        posicaoFechada = mesh.localPosition;
        posicaoAberta = posicaoFechada + direcaoDeslize.normalized * distanciaDeslize;
    }

    void Update()
    {
        if (mesh == null || detector == null) return;

        Vector3 alvo = detector.jogadorDetectado ? posicaoAberta : posicaoFechada;
        mesh.localPosition = Vector3.Lerp(mesh.localPosition, alvo, velocidadeAbertura * Time.deltaTime);
    }
}