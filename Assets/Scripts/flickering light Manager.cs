using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Escolhe luzes aleatórias de uma lista definida no Inspector e faz
/// elas piscarem durante um intervalo de tempo. Pode repetir em loop,
/// escolhendo um novo conjunto de luzes a cada ciclo.
/// </summary>
public class FlickerLightsManager : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Adicione aqui todas as luzes que vão sofrer o efeito")]
    [SerializeField] private List<Light> _targetLights = new List<Light>();

    [Header("Quantidade de luzes por ciclo")]
    [SerializeField] private int _minLightsToFlicker = 2;
    [SerializeField] private int _maxLightsToFlicker = 5;

    [Header("Configurações do piscar")]
    [SerializeField] private float _minFlickerDuration = 2f;
    [SerializeField] private float _maxFlickerDuration = 6f;
    [SerializeField] private float _minIntensity = 0.1f;
    [SerializeField] private float _maxIntensity = 1.2f;
    [Tooltip("De quanto em quanto tempo a luz escolhe uma nova intensidade alvo")]
    [SerializeField] private float _flickerChangeInterval = 0.08f;

    [Header("Repetição")]
    [Tooltip("Se marcado, depois de cada ciclo escolhe luzes novas e repete para sempre")]
    [SerializeField] private bool _loop = true;
    [SerializeField] private float _minTimeBetweenCycles = 3f;
    [SerializeField] private float _maxTimeBetweenCycles = 10f;

    private void Start()
    {
        // Remove da lista espaços vazios (null) caso algum slot fique em branco no Inspector
        _targetLights.RemoveAll(l => l == null);

        if (_targetLights.Count == 0)
        {
            Debug.LogWarning("FlickerLightsManager: Nenhuma luz foi adicionada à lista '_targetLights'.");
            return;
        }

        StartCoroutine(FlickerCycleRoutine());
    }

    private IEnumerator FlickerCycleRoutine()
    {
        do
        {
            if (_targetLights.Count > 0)
            {
                // Define quantas luzes vão piscar neste ciclo
                int count = Random.Range(_minLightsToFlicker, _maxLightsToFlicker + 1);
                count = Mathf.Min(count, _targetLights.Count);

                List<Light> chosen = PickRandomLights(count);
                float duration = Random.Range(_minFlickerDuration, _maxFlickerDuration);

                // Inicia o efeito para cada luz escolhida
                foreach (Light light in chosen)
                {
                    StartCoroutine(FlickerSingleLight(light, duration));
                }

                // Espera a duração do piscar acabar
                yield return new WaitForSeconds(duration);
            }

            if (_loop)
            {
                // Espera um tempo antes de começar o próximo ciclo
                float wait = Random.Range(_minTimeBetweenCycles, _maxTimeBetweenCycles);
                yield return new WaitForSeconds(wait);
            }

        } while (_loop);
    }

    private List<Light> PickRandomLights(int count)
    {
        // Cria uma cópia da lista original para podermos remover os itens já sorteados
        List<Light> pool = new List<Light>(_targetLights);
        List<Light> result = new List<Light>();

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return result;
    }

    private IEnumerator FlickerSingleLight(Light light, float duration)
    {
        if (light == null) yield break;

        float originalIntensity = light.intensity;
        float elapsed = 0f;
        float timer = 0f;
        float targetIntensity = Random.Range(_minIntensity, _maxIntensity);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            timer += Time.deltaTime;

            light.intensity = Mathf.Lerp(light.intensity, targetIntensity, Time.deltaTime * 10f);

            // Troca o alvo da intensidade de tempos em tempos
            if (timer >= _flickerChangeInterval)
            {
                targetIntensity = Random.Range(_minIntensity, _maxIntensity);
                timer = 0f;
            }

            yield return null;
        }

        // Restaura a intensidade original ao final do ciclo
        light.intensity = originalIntensity;
    }
}