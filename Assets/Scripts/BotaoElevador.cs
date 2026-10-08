using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class BotaoElevador : MonoBehaviour
{
    public ElevadorPlataforma elevador;
    public int indiceAndar;

    [Header("Cores de Emissão")]
    [ColorUsage(true, true)] public Color corDesligado = Color.black;
    [ColorUsage(true, true)] public Color corMirando = Color.yellow;
    [ColorUsage(true, true)] public Color corSelecionado = Color.green;

    private Renderer rend;
    private MaterialPropertyBlock propBlock;
    private bool foiClicado = false;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        MudarCor(corDesligado);
    }

    // Chamado pelo Raycast quando o jogador olha para o botão
    public void AoMirar()
    {
        // Só acende se não tiver sido clicado e se o elevador estiver parado
        if (!foiClicado && !elevador.EstaEmMovimento)
        {
            MudarCor(corMirando);
        }
    }

    // Chamado pelo Raycast quando o jogador desvia o olhar
    public void AoTirarMira()
    {
        if (!foiClicado)
        {
            MudarCor(corDesligado);
        }
    }

    // Chamado quando o jogador clica
    public void AoClicar()
    {
        // Trava o clique se o elevador já está andando ou se já estamos neste andar
        if (elevador.EstaEmMovimento || indiceAndar == elevador.AndarAtual) return;

        foiClicado = true;
        MudarCor(corSelecionado);
        elevador.IrParaAndar(indiceAndar);
    }

    // Chamado pelo elevador quando ele chega no destino
    public void ResetarBotao()
    {
        foiClicado = false;
        MudarCor(corDesligado);
    }

    private void MudarCor(Color cor)
    {
        // O MaterialPropertyBlock altera a propriedade gráfica sem duplicar o material
        rend.GetPropertyBlock(propBlock);
        propBlock.SetColor("_EmissionColor", cor);
        rend.SetPropertyBlock(propBlock);
    }
}