using UnityEngine;

public class BotaoElevador : MonoBehaviour
{
    [Tooltip("Referência ao painel principal do elevador")]
    public ElevadorPlataforma elevador;

    [Tooltip("Qual o índice do andar deste botão na lista? (Ex: 0, 1, 2)")]
    public int indiceAndar;
}