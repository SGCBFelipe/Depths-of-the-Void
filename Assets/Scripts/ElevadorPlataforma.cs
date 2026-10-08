using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct ConfiguracaoAndar
{
    public string nomeAndar;
    [Tooltip("Posição exata do elevador neste andar")]
    public Vector3 posicao;
    [Tooltip("Rotação em graus (X, Y, Z) do elevador neste andar")]
    public Vector3 rotacao;
}

[RequireComponent(typeof(Collider))]
public class ElevadorPlataforma : MonoBehaviour
{
    [Header("Referências")]
    public Transform jogador;
    public Transform portaEsquerda;
    public Transform portaDireita;

    [Header("Configuração de Andares")]
    public List<ConfiguracaoAndar> andares = new List<ConfiguracaoAndar>();
    public float velocidadeElevador = 3f;

    [Header("Configuração das Portas")]
    public Vector3 eixoDeAbertura = new Vector3(1.5f, 0, 0);
    public float velocidadePortas = 5f;

    private Vector3 esqFechada, dirFechada;
    private Vector3 esqAberta, dirAberta;

    // Substitua as variáveis privadas antigas por estas propriedades públicas (mas que só o elevador altera):
    public int AndarAtual { get; private set; } = 0;
    public bool EstaEmMovimento { get; private set; } = false;
    private bool portasAbertas = false;
    private Coroutine rotinaPortas;

    private void Start()
    {
        if (jogador == null)
        {
            GameObject objJogador = GameObject.FindGameObjectWithTag("Player");
            if (objJogador != null) jogador = objJogador.transform;
        }

        esqFechada = portaEsquerda.localPosition;
        dirFechada = portaDireita.localPosition;
        esqAberta = esqFechada + eixoDeAbertura;
        dirAberta = dirFechada - eixoDeAbertura;

        if (andares.Count > 0)
        {
            transform.position = andares[0].posicao;
            transform.rotation = Quaternion.Euler(andares[0].rotacao);
        }
    }

    // O uso do BoxCollider substitui a necessidade de calcular distâncias no Update
    private void Update()
    {
        // Deixei em branco caso precise colocar alguma outra lógica no futuro
    }

    // Detecta quando o jogador entra no BoxCollider
    private void OnTriggerEnter(Collider other)
    {
        // Evita abrir as portas se o elevador estiver no meio da viagem
        if (EstaEmMovimento) return;

        if (other.CompareTag("Player"))
        {
            // Abre as portas
            AbrirPortas();

            // Põe o jogador como "filho" do elevador para que ele não caia ao subir/descer
            other.transform.SetParent(transform);
        }
    }

    // Detecta quando o jogador sai totalmente do elevador
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Tira o jogador do elevador na hierarquia (ele volta a ser livre na cena)
            other.transform.SetParent(null);

            // As portas NÃO fecham aqui. Elas ficarão abertas esperando o próximo comando.
        }
    }

    public void IrParaAndar(int indiceAndar)
    {
        if (EstaEmMovimento || indiceAndar == AndarAtual) return;
        if (indiceAndar < 0 || indiceAndar >= andares.Count) return;

        StartCoroutine(RotinaMoverElevador(indiceAndar));
    }

    private IEnumerator RotinaMoverElevador(int indiceAndar)
    {
        EstaEmMovimento = true;

        // 1. Ao receber o comando de ir para o andar, a primeira coisa é FECHAR as portas
        if (rotinaPortas != null) StopCoroutine(rotinaPortas);
        yield return StartCoroutine(RotinaMoverPortas(false));

        // 2. Prepara e inicia a viagem do elevador
        ConfiguracaoAndar destino = andares[indiceAndar];
        Vector3 posInicial = transform.position;
        Quaternion rotInicial = transform.rotation;
        Quaternion rotDestino = Quaternion.Euler(destino.rotacao);

        float distancia = Vector3.Distance(posInicial, destino.posicao);
        float duracao = distancia / velocidadeElevador;
        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, tempo / duracao);

            transform.position = Vector3.Lerp(posInicial, destino.posicao, t);
            transform.rotation = Quaternion.Lerp(rotInicial, rotDestino, t);
            yield return null;
        }

        transform.position = destino.posicao;
        transform.rotation = rotDestino;
        AndarAtual = indiceAndar;

        // 3. Chegou no destino: ABRE as portas novamente
        yield return StartCoroutine(RotinaMoverPortas(true));

        EstaEmMovimento = false;

        // NOVO: Pega todos os botões que são filhos do elevador e reseta a cor deles
        BotaoElevador[] todosOsBotoes = GetComponentsInChildren<BotaoElevador>();
        foreach (BotaoElevador btn in todosOsBotoes)
        {
            btn.ResetarBotao();
        }
    }

    private void AbrirPortas()
    {
        if (portasAbertas) return;
        if (rotinaPortas != null) StopCoroutine(rotinaPortas);
        rotinaPortas = StartCoroutine(RotinaMoverPortas(true));
    }

    private IEnumerator RotinaMoverPortas(bool abrir)
    {
        portasAbertas = abrir;
        Vector3 alvoEsq = abrir ? esqAberta : esqFechada;
        Vector3 alvoDir = abrir ? dirAberta : dirFechada;

        while (Vector3.Distance(portaEsquerda.localPosition, alvoEsq) > 0.001f)
        {
            portaEsquerda.localPosition = Vector3.Lerp(portaEsquerda.localPosition, alvoEsq, Time.deltaTime * velocidadePortas);
            portaDireita.localPosition = Vector3.Lerp(portaDireita.localPosition, alvoDir, Time.deltaTime * velocidadePortas);
            yield return null;
        }

        portaEsquerda.localPosition = alvoEsq;
        portaDireita.localPosition = alvoDir;
    }
}