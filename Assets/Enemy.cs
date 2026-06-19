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

    private NavMeshAgent agent;
    private EstadoInimigo estadoAtual;
    private bool estaMudandoDeEstado = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
        }
    }

    private void IrParaPontoAleatorio()
    {
        Vector3 pontoAleatorio = GerarPontoAleatorioNoNavMesh(transform.position, raioDePatrulha);
        agent.SetDestination(pontoAleatorio);
    }

    private Vector3 GerarPontoAleatorioNoNavMesh(Vector3 origem, float distancia)
    {
        Vector3 direcaoAleatoria = Random.insideUnitSphere * distancia;
        direcaoAleatoria += origem;

        NavMeshHit hit;
        // Busca o ponto válido mais próximo no NavMesh do pacote AI Navigation
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

        // Reseta o caminho atual do agente
        agent.ResetPath();

        // Define um tempo aleatório para ficar parado
        float tempoEspera = Random.Range(tempoMinParado, tempoMaxParado);
        yield return new WaitForSeconds(tempoEspera);

        estaMudandoDeEstado = false;
        MudarEstado(EstadoInimigo.Patrulhando);
    }
}
