using UnityEngine;

public class PeixeBolhas : MonoBehaviour
{
    [Header("Referência")]
    [SerializeField] private ParticleSystem sistemaDeBolhas;

    [Header("Configuração de Emissão")]
    [SerializeField] private float tempoMinEntreEmissoes = 3f;
    [SerializeField] private float tempoMaxEntreEmissoes = 7f;
    [SerializeField] private int minParticulasPorEmissao = 1;
    [SerializeField] private int maxParticulasPorEmissao = 3;

    private float tempoParaProximaEmissao;

    void Start()
    {
        if (sistemaDeBolhas == null)
            sistemaDeBolhas = GetComponentInChildren<ParticleSystem>();

        AgendarProximaEmissao();
    }

    void Update()
    {
        tempoParaProximaEmissao -= Time.deltaTime;

        if (tempoParaProximaEmissao <= 0f)
        {
            EmitirBolhas();
            AgendarProximaEmissao();
        }
    }

    private void EmitirBolhas()
    {
        if (sistemaDeBolhas == null) return;

        int quantidade = Random.Range(minParticulasPorEmissao, maxParticulasPorEmissao + 1);
        sistemaDeBolhas.Emit(quantidade);
    }

    private void AgendarProximaEmissao()
    {
        tempoParaProximaEmissao = Random.Range(tempoMinEntreEmissoes, tempoMaxEntreEmissoes);
    }
}