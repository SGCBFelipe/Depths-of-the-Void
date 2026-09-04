using UnityEngine;

public class DetectorDeJogador : MonoBehaviour
{
    [Header("Estado")]
    public bool jogadorDetectado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            jogadorDetectado = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            jogadorDetectado = false;
    }
}