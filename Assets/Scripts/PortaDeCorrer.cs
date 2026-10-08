using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class PortaDeCorrer : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform mesh;
    [SerializeField] private DetectorDeJogador detector;
    [SerializeField] private AudioSource audioSource;

    [Header("Deslize")]
    [SerializeField] private Vector3 direcaoDeslize = Vector3.right;
    [SerializeField] private float distanciaDeslize = 2f;
    [SerializeField] private float velocidadeAbertura = 2f;

    [Header("Sons de Abertura")]
    [SerializeField] private List<AudioClip> sonsAbrir;
    [SerializeField] private List<AudioClip> sonsAbrirRaros;

    [Header("Sons de Fechamento")]
    [SerializeField] private List<AudioClip> sonsFechar;
    [SerializeField] private List<AudioClip> sonsFecharRaros;

    [Header("Configuração de Raridade")]
    [Range(0.1f, 100f)]
    [SerializeField] private float chanceSomRaroPorcento = 2f;

    private Vector3 posicaoFechada;
    private Vector3 posicaoAberta;
    private bool estavaAberto = false;

    void Start()
    {
        if (mesh == null)
            Debug.LogWarning("[PortaDeCorrer] Arraste o objeto 'Mesh' no campo Mesh do Inspector.");

        if (detector == null)
            Debug.LogWarning("[PortaDeCorrer] Arraste o objeto com o DetectorDeJogador no campo Detector do Inspector.");

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // DIAGNÓSTICO 1: Verifica se o detector está dentro do mesh que se move
        if (detector != null && mesh != null && detector.transform.IsChildOf(mesh))
        {
            Debug.LogError("[ERRO GRAVE] O Detector é filho do Mesh da porta! Quando a porta se mover, o detector vai se mover junto e quebrar o gatilho de áudio. Mova o Detector para fora da folha da porta na Hierarquia!");
        }

        posicaoFechada = mesh.localPosition;
        Vector3 direcaoLocal = mesh.TransformDirection(direcaoDeslize.normalized);
        posicaoAberta = posicaoFechada + direcaoLocal * distanciaDeslize;
    }

    void Update()
    {
        if (mesh == null || detector == null) return;

        bool querAbrir = detector.jogadorDetectado;

        if (querAbrir != estavaAberto)
        {
            estavaAberto = querAbrir;

            if (querAbrir)
            {
                TocarSomComProbabilidade(sonsAbrir, sonsAbrirRaros);
            }
            else
            {
                TocarSomComProbabilidade(sonsFechar, sonsFecharRaros);
            }
        }

        Vector3 alvo = querAbrir ? posicaoAberta : posicaoFechada;
        mesh.localPosition = Vector3.Lerp(mesh.localPosition, alvo, velocidadeAbertura * Time.deltaTime);
    }

    private void TocarSomComProbabilidade(List<AudioClip> listaSonsComuns, List<AudioClip> listaSonsRaros)
    {
        if (audioSource == null)
        {
            Debug.LogError("[PortaDeCorrer] AudioSource não encontrado!");
            return;
        }

        AudioClip somParaTocar = null;
        float sorteio = Random.Range(0f, 100f);

        if (listaSonsRaros != null && listaSonsRaros.Count > 0 && sorteio <= chanceSomRaroPorcento)
        {
            int indiceRaroAleatorio = Random.Range(0, listaSonsRaros.Count);
            somParaTocar = listaSonsRaros[indiceRaroAleatorio];
        }
        else if (listaSonsComuns != null && listaSonsComuns.Count > 0)
        {
            int indiceComumAleatorio = Random.Range(0, listaSonsComuns.Count);
            somParaTocar = listaSonsComuns[indiceComumAleatorio];
        }

        if (somParaTocar != null)
        {
            // DIAGNÓSTICO 2: Loga no console se o áudio está sendo chamado muitas vezes seguidas
            Debug.Log($"[ÁUDIO DISPARADO] Tocando: {somParaTocar.name} | Volume: {audioSource.volume} | Mute: {audioSource.mute} | AudioSource Ativo: {audioSource.enabled}");

            // Garante que o AudioSource e o GameObject estejam ativos
            audioSource.enabled = true;

            // Usar PlayOneShot evita que chamadas consecutivas cancelem o som anterior
            audioSource.PlayOneShot(somParaTocar);
        }
        else
        {
            Debug.LogWarning("[PortaDeCorrer] Nenhum AudioClip foi selecionado. As listas estão preenchidas no Inspector?");
        }
    }
}