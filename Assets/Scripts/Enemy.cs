using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public enum EstadoInimigo { Parado, Patrulhando }

    [Header("Configurações de Movimento")]
    [SerializeField] private float raioDePatrulha = 15f;
    [SerializeField] private float tempoMinParado = 2f;
    [SerializeField] private float tempoMaxParado = 5f;

    [Header("Animação")]
    [SerializeField] private Animator animator;

    private NavMeshAgent agent;
    private EstadoInimigo estadoAtual;
    private bool estaMudandoDeEstado = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>(); // tenta pegar automaticamente se não foi arrastado no Inspector

        MudarEstado(EstadoInimigo.Patrulhando);
    }

    void Update()
    {
        AcordoComEstado();
    }

    private void AcordoComEstado()
    {
        if (estaMudandoDeEstado) return;

        switch (estadoAtual)
        {
            case EstadoInimigo.Parado:
                // Aguarda o fim do temporizador da Corrotina
                break;

            case EstadoInimigo.Patrulhando:
                // Verifica se chegou ao destino (considerando a distância de parada)
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    StartCoroutine(FicarParadoRotina());
                }
                break;
        }
    }

    private void MudarEstado(EstadoInimigo novoEstado)
    {
        estadoAtual = novoEstado;

        if (estadoAtual == EstadoInimigo.Patrulhando)
        {
            IrParaPontoAleatorio();
            AtualizarAnimacao(isWalking: true);
        }
        else if (estadoAtual == EstadoInimigo.Parado)
        {
            AtualizarAnimacao(isWalking: false);
        }
    }

    private void IrParaPontoAleatorio()
    {
        Vector3 pontoAleatorio = GerarPontoAleatorioNoNavMesh(transform.position, raioDePatrulha);
        agent.SetDestination(pontoAleatorio);

        // Sorteia o tipo de caminhada (0 ou 1) pra esse trecho de patrulha
        float tipoCaminhada = Random.Range(0, 2); // retorna 0 ou 1
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

        agent.ResetPath();

        float tempoEspera = Random.Range(tempoMinParado, tempoMaxParado);
        yield return new WaitForSeconds(tempoEspera);

        estaMudandoDeEstado = false;
        MudarEstado(EstadoInimigo.Patrulhando);
    }
}