using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public enum EstadoInimigo { Parado, Patrulhando, Alerta, Perseguindo }

    [Header("Inspector Debug (Apenas Leitura)")]
    [SerializeField] private EstadoInimigo estadoAtual;
    [SerializeField] private bool vendoPlayerDebug;
    [SerializeField] private float distanciaAtualDebug;
    [SerializeField] private string objetoBloqueandoVisao = "Nenhum";

    [Header("Configurações de Movimento")]
    [SerializeField] private float raioDePatrulha = 15f;
    [SerializeField] private float tempoMinParado = 2f;
    [SerializeField] private float tempoMaxParado = 5f;
    [SerializeField] private float velocidadePatrulha = 3.5f;
    [SerializeField] private float velocidadePerseguicao = 6.0f;

    [Header("Visão e Detecção")]
    [SerializeField] private Transform player;
    [SerializeField] private float alcanceVisao = 12f;
    [SerializeField] private float anguloVisao = 80f;
    [SerializeField] private float alturaOlhos = 1.5f;
    [SerializeField] private float distanciaPerseguirGarantida = 4f;
    [SerializeField] private float distanciaDespistar = 16f;
    [SerializeField] private float tempoParaPerderDeVista = 2.5f;

    [Header("Configurações de Alerta")]
    [SerializeField] private float velocidadeRotacaoAlerta = 8f;

    [Header("Animação")]
    [SerializeField] private Animator animator;

    private NavMeshAgent agent;
    private bool estaMudandoDeEstado = false;
    private Coroutine rotinaEspera;
    private float temporizadorPerderVista = 0f;
    private Vector3 pontoPartidaDebug;
    private Vector3 pontoDestinoDebug;
    private Color corRaioDebug = Color.yellow;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogError("Enemy: Nenhum objeto com a Tag 'Player' foi encontrado no cenário!");
        }

        MudarEstado(EstadoInimigo.Patrulhando);
    }

    void Update()
    {
        ProcessarSensoriamento();
        AcordoComEstado();
    }

    private void ProcessarSensoriamento()
    {
        if (player == null) return;

        bool vendoPlayer = ChecarVisaoPlayer(out float distanciaPlayer);

        // Atualiza variáveis de debug visíveis no Inspector
        vendoPlayerDebug = vendoPlayer;
        distanciaAtualDebug = distanciaPlayer;

        if (vendoPlayer)
        {
            temporizadorPerderVista = 0f;

            // Transição para Perseguindo
            if (distanciaPlayer <= distanciaPerseguirGarantida || estadoAtual == EstadoInimigo.Alerta)
            {
                if (estadoAtual != EstadoInimigo.Perseguindo)
                {
                    MudarEstado(EstadoInimigo.Perseguindo);
                }
            }
            // Transição para Alerta
            else if (estadoAtual == EstadoInimigo.Parado || estadoAtual == EstadoInimigo.Patrulhando)
            {
                MudarEstado(EstadoInimigo.Alerta);
            }
        }
        else
        {
            // Lógica para perder de vista / desistir
            if (estadoAtual == EstadoInimigo.Perseguindo || estadoAtual == EstadoInimigo.Alerta)
            {
                temporizadorPerderVista += Time.deltaTime;

                if (distanciaPlayer > distanciaDespistar || temporizadorPerderVista >= tempoParaPerderDeVista)
                {
                    MudarEstado(EstadoInimigo.Patrulhando);
                }
            }
        }
    }

    private bool ChecarVisaoPlayer(out float distancia)
    {
        distancia = Vector3.Distance(transform.position, player.position);

        Vector3 origem = transform.position + Vector3.up * alturaOlhos;
        Vector3 destino = player.position + Vector3.up * alturaOlhos;
        Vector3 direcao = (destino - origem).normalized;

        pontoPartidaDebug = origem;
        pontoDestinoDebug = destino;

        // 1. Checa alcance
        if (distancia > alcanceVisao)
        {
            objetoBloqueandoVisao = "Fora do Alcance";
            corRaioDebug = Color.gray;
            return false;
        }

        // 2. Checa ângulo de visão (FOV)
        float angulo = Vector3.Angle(transform.forward, direcao);
        if (angulo > anguloVisao / 2f)
        {
            objetoBloqueandoVisao = "Fora do Ângulo de Visão";
            corRaioDebug = Color.yellow;
            return false;
        }

        // 3. Lança RaycastAll para ignorar colisões do próprio inimigo
        RaycastHit[] hits = Physics.RaycastAll(origem, direcao, alcanceVisao, ~0, QueryTriggerInteraction.Ignore);

        // Ordena colisões da mais próxima para a mais distante
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        foreach (RaycastHit hit in hits)
        {
            // Ignora colisão se for o próprio inimigo ou filhos dele
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
                continue;

            objetoBloqueandoVisao = hit.transform.name;

            // Se atingir o Player (ou filho do Player ou tag Player)
            if (hit.transform == player || hit.transform.IsChildOf(player) || hit.transform.CompareTag("Player"))
            {
                corRaioDebug = Color.green; // Visão limpa!
                return true;
            }
            else
            {
                // Atingiu um obstáculo antes de chegar no Player
                corRaioDebug = Color.red; // Bloqueado por parede/objeto
                return false;
            }
        }

        objetoBloqueandoVisao = "Nada atingido";
        corRaioDebug = Color.gray;
        return false;
    }

    private void AcordoComEstado()
    {
        switch (estadoAtual)
        {
            case EstadoInimigo.Parado:
                break;

            case EstadoInimigo.Patrulhando:
                if (!estaMudandoDeEstado && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    rotinaEspera = StartCoroutine(FicarParadoRotina());
                }
                break;

            case EstadoInimigo.Alerta:
                EncararAlvo(player.position);
                break;

            case EstadoInimigo.Perseguindo:
                if (player != null)
                {
                    agent.SetDestination(player.position);
                }
                break;
        }
    }

    private void EncararAlvo(Vector3 destino)
    {
        Vector3 direcao = (destino - transform.position).normalized;
        direcao.y = 0;

        if (direcao != Vector3.zero)
        {
            Quaternion rotacaoAlvo = Quaternion.LookRotation(direcao);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacaoAlvo, Time.deltaTime * velocidadeRotacaoAlerta);
        }
    }

    private void MudarEstado(EstadoInimigo novoEstado)
    {
        if (rotinaEspera != null)
        {
            StopCoroutine(rotinaEspera);
            rotinaEspera = null;
        }
        estaMudandoDeEstado = false;

        estadoAtual = novoEstado;

        switch (estadoAtual)
        {
            case EstadoInimigo.Patrulhando:
                agent.isStopped = false;
                agent.speed = velocidadePatrulha;
                IrParaPontoAleatorio();
                AtualizarAnimacao(isWalking: true);
                break;

            case EstadoInimigo.Parado:
                agent.isStopped = true;
                AtualizarAnimacao(isWalking: false);
                break;

            case EstadoInimigo.Alerta:
                agent.isStopped = true;
                agent.ResetPath();
                AtualizarAnimacao(isWalking: false);
                break;

            case EstadoInimigo.Perseguindo:
                agent.isStopped = false;
                agent.speed = velocidadePerseguicao;
                AtualizarAnimacao(isWalking: true);
                break;
        }
    }

    private void IrParaPontoAleatorio()
    {
        Vector3 pontoAleatorio = GerarPontoAleatorioNoNavMesh(transform.position, raioDePatrulha);
        agent.SetDestination(pontoAleatorio);

        float tipoCaminhada = Random.Range(0, 2);
        if (animator != null)
            animator.SetFloat("Walk", tipoCaminhada);
    }

    private void AtualizarAnimacao(bool isWalking)
    {
        if (animator == null) return;
        animator.SetBool("Is_Moving", isWalking);
    }

    private Vector3 GerarPontoAleatorioNoNavMesh(Vector3 origem, float distancia)
    {
        Vector3 direcaoAleatoria = Random.insideUnitSphere * distancia;
        direcaoAleatoria += origem;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(direcaoAleatoria, out hit, distancia, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return origem;
    }

    private IEnumerator FicarParadoRotina()
    {
        estaMudandoDeEstado = true;
        MudarEstado(EstadoInimigo.Parado);

        float tempoEspera = Random.Range(tempoMinParado, tempoMaxParado);
        yield return new WaitForSeconds(tempoEspera);

        estaMudandoDeEstado = false;
        MudarEstado(EstadoInimigo.Patrulhando);
    }

    private void OnDrawGizmos()
    {
        // Linha do Raycast visível na aba Scene/Game
        Gizmos.color = corRaioDebug;
        Gizmos.DrawLine(pontoPartidaDebug, pontoDestinoDebug);
        Gizmos.DrawSphere(pontoPartidaDebug, 0.1f);

        // Círculos de Alcance (quando o objeto está selecionado)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alcanceVisao);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaPerseguirGarantida);
    }
}