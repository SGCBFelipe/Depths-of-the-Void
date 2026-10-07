using UnityEngine;

/// <summary>
/// Elevador / plataforma móvel que se desloca E gira quando o jogador está em cima.
/// Move-se nos 3 eixos (X, Y, Z) e gira nos 3 eixos (ângulos de Euler).
/// Posição e rotação andam sincronizadas: chegam ao destino ao mesmo tempo.
///
/// SETUP:
/// 1. Coloque este script no objeto da plataforma (precisa de um Collider sólido para o jogador pisar).
/// 2. Adicione OUTRO BoxCollider no mesmo objeto, marque "Is Trigger" e deixe-o um pouco
///    mais alto que a plataforma (para pegar o jogador em cima dela).
/// 3. Coloque a tag "Player" no jogador (ou mude a tag no Inspector).
/// 4. Ajuste "Deslocamento" e "Angulo Rotacao" no Inspector.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ElevadorPlataforma : MonoBehaviour
{
    [Header("Movimento")]
    [Tooltip("Quanto a plataforma se desloca a partir da posição inicial (X, Y, Z em unidades do mundo).")]
    [SerializeField] private Vector3 deslocamento = new Vector3(0f, 5f, 0f);

    [Tooltip("Velocidade em unidades por segundo.")]
    [SerializeField] private float velocidade = 2f;

    [Header("Rotação")]
    [Tooltip("Quanto a plataforma gira (graus em X, Y, Z) em relação à rotação inicial. Ex.: (0, 90, 0) gira 90° no eixo Y. Aceita valores acima de 180 (ex.: 360 = volta completa).")]
    [SerializeField] private Vector3 anguloRotacao = new Vector3(0f, 90f, 0f);

    [Tooltip("Velocidade de rotação em graus por segundo.")]
    [SerializeField] private float velocidadeRotacao = 45f;

    [Header("Comportamento")]
    [Tooltip("Tempo de espera (s) antes de começar a mover depois que o jogador sobe.")]
    [SerializeField] private float atrasoParaIniciar = 0.3f;

    [Tooltip("Se ligado, a plataforma volta (posição e rotação) quando o jogador sai.")]
    [SerializeField] private bool voltarQuandoJogadorSai = true;

    [Tooltip("Se ligado, o jogador vira filho da plataforma enquanto está em cima (acompanha movimento e giro).")]
    [SerializeField] private bool carregarJogador = true;

    [SerializeField] private string tagJogador = "Player";

    private Rigidbody rb;
    private Vector3 posicaoInicial;
    private Quaternion rotacaoInicial;
    private float duracao;       // tempo (s) para ir de 0 a 1
    private float progresso;     // 0 = posição/rotação inicial, 1 = final
    private bool jogadorEmCima;
    private float contadorAtraso;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        posicaoInicial = transform.position;
        rotacaoInicial = transform.rotation;

        // Duração = o que demorar mais: deslocar ou girar (assim os dois terminam juntos)
        float tempoMovimento = velocidade > 0f ? deslocamento.magnitude / velocidade : 0f;
        float maiorAngulo = Mathf.Max(Mathf.Abs(anguloRotacao.x),
                            Mathf.Max(Mathf.Abs(anguloRotacao.y), Mathf.Abs(anguloRotacao.z)));
        float tempoRotacao = velocidadeRotacao > 0f ? maiorAngulo / velocidadeRotacao : 0f;
        duracao = Mathf.Max(tempoMovimento, tempoRotacao, 0.01f);
    }

    private void FixedUpdate()
    {
        float alvoProgresso;

        if (jogadorEmCima)
        {
            if (contadorAtraso > 0f)
            {
                contadorAtraso -= Time.fixedDeltaTime;
                return;
            }
            alvoProgresso = 1f;
        }
        else if (voltarQuandoJogadorSai)
        {
            alvoProgresso = 0f;
        }
        else
        {
            return; // fica parado onde está
        }

        progresso = Mathf.MoveTowards(progresso, alvoProgresso, Time.fixedDeltaTime / duracao);

        rb.MovePosition(posicaoInicial + deslocamento * progresso);
        rb.MoveRotation(rotacaoInicial * Quaternion.Euler(anguloRotacao * progresso));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagJogador)) return;

        jogadorEmCima = true;
        contadorAtraso = atrasoParaIniciar;

        if (carregarJogador)
            other.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(tagJogador)) return;

        jogadorEmCima = false;

        if (carregarJogador && other.transform.parent == transform)
            other.transform.SetParent(null);
    }

    // Mostra o trajeto no editor (linha amarela) e a pose final (cubo verde, já girado)
    private void OnDrawGizmosSelected()
    {
        Vector3 inicio = Application.isPlaying ? posicaoInicial : transform.position;
        Quaternion rotBase = Application.isPlaying ? rotacaoInicial : transform.rotation;
        Vector3 fim = inicio + deslocamento;
        Quaternion rotFim = rotBase * Quaternion.Euler(anguloRotacao);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(inicio, fim);

        Gizmos.color = Color.green;
        Matrix4x4 anterior = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(fim, rotFim, transform.localScale);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = anterior;
    }
}
