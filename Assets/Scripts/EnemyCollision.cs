using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class EnemyCollision : MonoBehaviour
{
    [Header("Configurações de Respawn")]
    [Tooltip("Arraste o Empty GameObject (Ponto de Spawn) do cenário aqui")]
    [SerializeField] private Transform playerSpawn;

    [Header("Efeito de Transição (Fade)")]
    [Tooltip("Arraste o componente CanvasGroup do painel da UI aqui")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    [Tooltip("Tempo (segundos) para a tela ficar totalmente preta")]
    [SerializeField] private float duracaoFadeOut = 0.8f;

    [Tooltip("Tempo que a tela permanece preta no respawn")]
    [SerializeField] private float tempoTelaPreta = 0.5f;

    [Tooltip("Tempo (segundos) para a tela voltar ao normal")]
    [SerializeField] private float duracaoFadeIn = 0.8f;

    private bool aguardandoRespawn = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !aguardandoRespawn)
        {
            StartCoroutine(ExecutarRespawnComFade(collision.gameObject));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !aguardandoRespawn)
        {
            StartCoroutine(ExecutarRespawnComFade(other.gameObject));
        }
    }

    private IEnumerator ExecutarRespawnComFade(GameObject jogador)
    {
        aguardandoRespawn = true;

        if (playerSpawn == null)
        {
            Debug.LogError("EnemyCollision: 'Player Spawn' não configurado no Inspector!");
            aguardandoRespawn = false;
            yield break;
        }

        // 1. Fade Out (Escurece a tela até ficar preta)
        if (fadeCanvasGroup != null)
        {
            yield return StartCoroutine(AnimarFade(0f, 1f, duracaoFadeOut));
        }

        // 2. Teleporta o jogador enquanto a tela está completamente escura
        // Se usar CharacterController, é necessário desativá-lo temporariamente para permitir o teleporte
        CharacterController cc = jogador.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        jogador.transform.position = playerSpawn.position;
        jogador.transform.rotation = playerSpawn.rotation;

        if (cc != null) cc.enabled = true;

        // 3. Aguarda o tempo de espera em tela preta
        yield return new WaitForSeconds(tempoTelaPreta);

        // 4. Fade In (Clareia a tela de volta ao normal)
        if (fadeCanvasGroup != null)
        {
            yield return StartCoroutine(AnimarFade(1f, 0f, duracaoFadeIn));
        }

        aguardandoRespawn = false;
    }

    private IEnumerator AnimarFade(float alphaInicial, float alphaFinal, float duracao)
    {
        float tempoDecorrido = 0f;

        while (tempoDecorrido < duracao)
        {
            tempoDecorrido += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(alphaInicial, alphaFinal, tempoDecorrido / duracao);
            yield return null;
        }

        fadeCanvasGroup.alpha = alphaFinal;
    }
}